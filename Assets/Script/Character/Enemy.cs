using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using Random = UnityEngine.Random;
public class Enemy : CharacterBase
{
    #region Inspector Fields
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Movement Settings")]
    [SerializeField] private float randomMoveInterval = 2f;

    [Header("Drop Settings")]
    [SerializeField] private GameObject itemDropPrefab;
    #endregion

    #region Private Fields
    private Player target;
    private Vector2 randomDirection;
    private float randomMoveTimer;
    private bool isInitialized;

    // Cached components
    private Transform cachedTransform;
    private static readonly WaitForSeconds dispawnDelay = new WaitForSeconds(1.5f);
    #endregion

    #region Unity Lifecycle Methods
    public override void Start()
    {
        base.Start();
        Initialize();
    }

    public override void Update()
    {
        if (!isInitialized) return;

        base.Update();
        if (!isDead) HandleAI();
    }

    private void FixedUpdate()
    {
        if (!isInitialized || isDead) return;

        HandleMovement();
    }

    private void OnDrawGizmos()
    {
        DrawDebugRanges();
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        cachedTransform = transform;
        isInitialized = true;
    }

    public void SetDataByCharacter(Character data)
    {
        if (data == null)
        {
            Debug.LogError($"[{nameof(Enemy)}] Attempted to set null character data!");
            return;
        }

        maxHP = data.hp;
        attackMin = data.attackMin;
        attackMax = data.attackMax;
        speed = data.moveSpeed;
        attackSpeed = data.attackSpeed;
    }
    #endregion

    #region AI Behavior
    private void HandleAI()
    {
        UpdateTargetStatus();
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(cachedTransform.position, target.transform.position);

        if (distanceToTarget <= detectionRange)
        {
            if (distanceToTarget <= attackRange)
            {
                HandleAttackBehavior();
            }
            else
            {
                HandleChaseBehavior();
            }
        }
        else
        {
            HandleIdleBehavior();
        }
    }

    private void UpdateTargetStatus()
    {
        if (target == null || target.isDead)
        {
            target = FindNearestPlayer();
            isChasing = false;
            isAttacking = false;
        }
    }

    private void HandleAttackBehavior()
    {
        rb.velocity = Vector2.zero;
        TryAttackPlayer();
        isChasing = true;
    }

    private void HandleChaseBehavior()
    {
        isChasing = false;
        isAttacking = false;
        MoveTowardsTarget();
    }

    private void HandleIdleBehavior()
    {
        isChasing = false;
        rb.velocity = Vector2.zero;
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        if (!isChasing && !isAttacking && target == null)
        {
            MoveRandomly();
            if (target == null)
            {
                target = FindNearestPlayer();
            }
        }
        else if (!isChasing && !isAttacking && target != null)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveRandomly()
    {
        if (randomMoveTimer <= 0f)
        {
            GenerateNewRandomDirection();
        }
        else
        {
            randomMoveTimer -= Time.deltaTime;
        }

        ApplyRandomMovement();
        UpdateFacingDirection(randomDirection.x);
    }

    private void GenerateNewRandomDirection()
    {
        randomDirection = Random.insideUnitCircle.normalized;
        randomMoveTimer = randomMoveInterval;
    }

    private void ApplyRandomMovement()
    {
        rb.velocity = randomDirection * speed;
        Debug.DrawLine(cachedTransform.position,
            (Vector2)cachedTransform.position + randomDirection * speed * Time.deltaTime,
            Color.red, 0.5f);
    }

    private void MoveTowardsTarget()
    {
        if (target == null)
        {
            MoveRandomly();
            return;
        }

        Vector2 direction = (target.transform.position - cachedTransform.position).normalized;
        rb.velocity = direction * speed;
        UpdateFacingDirection(direction.x);
    }

    private void UpdateFacingDirection(float directionX)
    {
        cachedTransform.localScale = new Vector3(directionX < 0 ? 1 : -1, 1, 1);
    }
    #endregion

    #region Combat
    private void TryAttackPlayer()
    {
        if (!isAttacking && Time.time - lastAttackTime >= 1f / attackSpeed)
        {
            isAttacking = true;
            AttackPlayer();
            lastAttackTime = Time.time;
            Invoke(nameof(EndAttack), 0.5f);
        }
    }

    private void AttackPlayer()
    {
        if (target != null)
        {
            float damage = Random.Range(attackMin, attackMax);
            target.TakeDamage(damage);
        }
        Attack();
    }

    private void EndAttack()
    {
        isAttacking = false;
    }
    #endregion

    #region Player Detection
    private Player FindNearestPlayer()
    {
        Vector3 center = boxCollider2D != null ? boxCollider2D.bounds.center : cachedTransform.position;
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(center, detectionRange, playerLayer);

        return FindClosestValidPlayer(playersInRange, center);
    }

    private Player FindClosestValidPlayer(Collider2D[] players, Vector3 center)
    {
        Player nearestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D playerCollider in players)
        {
            if (playerCollider.TryGetComponent(out Player player) && !player.isDead)
            {
                float distance = Vector2.Distance(center, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestPlayer = player;
                }
            }
        }

        return nearestPlayer;
    }
    #endregion

    #region Death and Item Drop
 protected override void Die()
 {
     base.Die();
     StartCoroutine(DispawnRoutine());
 }
 
 private IEnumerator DispawnRoutine()
 {
    
 
     using (UnityWebRequest request = CreateKillMonsterRequest())
     {
         yield return request.SendWebRequest();
 
         if (request.result == UnityWebRequest.Result.Success)
         {
             HandleDropItem(request.downloadHandler.text);
         }
         else
         {
             Debug.LogError($"Kill monster request failed: {request.error}");
         }
     }
      yield return dispawnDelay;
 }
 
 private UnityWebRequest CreateKillMonsterRequest()
 {
     string url = $"{DataManager.Instance.SERVER_URL}/api/game/kill-monster";
     UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
     
     request.uploadHandler = new UploadHandlerRaw(new byte[0]);
     request.downloadHandler = new DownloadHandlerBuffer();
     request.SetRequestHeader("Content-Type", "application/json");
     request.SetRequestHeader("Authorization", $"Bearer {DataManager.Instance.Get<string>("token")}");
     
     return request;
 }
 
 [System.Serializable]
 public class RewardResponse
 {
     public bool success;
     public Rewards rewards;
     public NewTotals newTotals;
     public string message;
 }
 
 [System.Serializable]
 public class Rewards
 {
     public int gems;
 }
 
 [System.Serializable]
 public class NewTotals
 {
     public int gems;
 }
 
 private void HandleDropItem(string responseData)
 {
     try
     {
         RewardResponse rewardResponse = JsonConvert.DeserializeObject<RewardResponse>(responseData);
         if (rewardResponse?.success == true)
         {
             DropItem();
             PlayerDataManager.Instance.playerData.SetGold(rewardResponse.newTotals.gems);
             Destroy(gameObject);
         }
         else
         {
             Debug.LogWarning($"Reward response unsuccessful: {rewardResponse?.message}");
         }
     }
     catch (JsonException ex)
     {
         Debug.LogError($"Failed to parse reward response: {ex.Message}");
     }
 }
 
 private void DropItem()
 {
     if (itemDropPrefab == null) return;
     
     Vector3 dropPosition = cachedTransform.position;
     Instantiate(itemDropPrefab, dropPosition, Quaternion.identity, UIOnMap.transform);
 }
 

    #endregion

    #region Debug
    private void DrawDebugRanges()
    {
        Vector3 center = boxCollider2D != null ? boxCollider2D.bounds.center : cachedTransform.position;

        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectionRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);

        // Damage text range
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(center, randomDameTextRange);
    }
    #endregion

    #region Collision Handling
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            randomDirection = -randomDirection;
        }
    }
    #endregion
}
