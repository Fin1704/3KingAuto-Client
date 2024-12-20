using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static Dictionary<string, Action<object[]>> eventDictionary = new Dictionary<string, Action<object[]>>();

    public static void FireEvent(string eventName, params object[] parameters)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName]?.Invoke(parameters);
        }
    }

    public static void StartListening(string eventName, Action<object[]> listener)
    {
        if (!eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] = listener;
        }
        else
        {
            eventDictionary[eventName] += listener;
        }
    }

    public static void StopListening(string eventName, Action<object[]> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }
}
