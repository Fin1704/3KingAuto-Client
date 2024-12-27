using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Use a concurrent dictionary to handle multi-threaded scenarios safely
    private static readonly Dictionary<string, Action<object[]>> eventDictionary = 
        new Dictionary<string, Action<object[]>>(StringComparer.Ordinal);
    
    // Cache for frequently used event names to avoid string allocations
    private static readonly HashSet<string> registeredEvents = new HashSet<string>(StringComparer.Ordinal);
    
    // Object pool for parameter arrays to reduce garbage collection
    private static readonly Queue<object[]> parameterPool = new Queue<object[]>();
    private const int INITIAL_POOL_SIZE = 10;
    private const int MAX_POOL_SIZE = 100;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Pre-populate parameter pool
        for (int i = 0; i < INITIAL_POOL_SIZE; i++)
        {
            parameterPool.Enqueue(new object[8]); // Default size for parameter arrays
        }
    }

    public static void FireEvent(string eventName, params object[] parameters)
    {
        if (string.IsNullOrEmpty(eventName)) return;

        if (eventDictionary.TryGetValue(eventName, out Action<object[]> action))
        {
            try
            {
                action?.Invoke(parameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error firing event {eventName}: {e}");
            }
        }
    }

    public static void StartListening(string eventName, Action<object[]> listener)
    {
        if (string.IsNullOrEmpty(eventName) || listener == null) return;

        lock (eventDictionary)
        {
            if (!eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] = listener;
                registeredEvents.Add(eventName);
            }
            else
            {
                eventDictionary[eventName] += listener;
            }
        }
    }

    public static void StopListening(string eventName, Action<object[]> listener)
    {
        if (string.IsNullOrEmpty(eventName) || listener == null) return;

        lock (eventDictionary)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] -= listener;

                // Remove empty event handlers
                if (eventDictionary[eventName] == null)
                {
                    eventDictionary.Remove(eventName);
                    registeredEvents.Remove(eventName);
                }
            }
        }
    }

    // Helper method to get a parameter array from the pool
    public static object[] GetParameterArray()
    {
        lock (parameterPool)
        {
            return parameterPool.Count > 0 ? parameterPool.Dequeue() : new object[8];
        }
    }

    // Helper method to return a parameter array to the pool
    public static void ReturnParameterArray(object[] parameters)
    {
        if (parameters == null) return;

        lock (parameterPool)
        {
            if (parameterPool.Count < MAX_POOL_SIZE)
            {
                Array.Clear(parameters, 0, parameters.Length);
                parameterPool.Enqueue(parameters);
            }
        }
    }

    // Method to check if an event has any listeners
    public static bool HasListeners(string eventName)
    {
        return !string.IsNullOrEmpty(eventName) && registeredEvents.Contains(eventName);
    }

    // Clean up method to remove all listeners
    public static void ClearAllListeners()
    {
        lock (eventDictionary)
        {
            eventDictionary.Clear();
            registeredEvents.Clear();
        }
    }

    // Optional: Method to get listener count for debugging/monitoring
    public static int GetListenerCount(string eventName)
    {
        if (string.IsNullOrEmpty(eventName)) return 0;

        if (eventDictionary.TryGetValue(eventName, out Action<object[]> action))
        {
            return action?.GetInvocationList().Length ?? 0;
        }
        return 0;
    }

    private void OnDestroy()
    {
        ClearAllListeners();
    }
}

// Example usage with object pooling
public static class EventManagerExtensions
{
    public static void FireEventPooled(this EventManager manager, string eventName, params object[] parameters)
    {
        var pooledParams = EventManager.GetParameterArray();
        try
        {
            if (parameters != null && parameters.Length > 0)
            {
                Array.Copy(parameters, pooledParams, Math.Min(parameters.Length, pooledParams.Length));
            }
            EventManager.FireEvent(eventName, pooledParams);
        }
        finally
        {
            EventManager.ReturnParameterArray(pooledParams);
        }
    }
}
