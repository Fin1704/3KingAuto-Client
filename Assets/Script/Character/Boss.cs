using System.Collections;
using System.Text;
using Newtonsoft.Json;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Networking;


public class Boss : CharacterBase
{
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public Player target;
    public LayerMask playerLayer;
    private Vector2 randomDirection;
    private float randomMoveTimer = 0f;
    private float randomMoveInterval = 2f;
    public GameObject itemDropPrefab;
    public SkeletonAnimation effect_appear;
    private MeshRenderer meshRenderer;
    protected bool canMove = false;
    private Transform targetTransform;
    private Transform selfTransform;

    public override void Start()
    {
        healthBar.slider.gameObject.SetActive(false);
        base.Start();
        meshRenderer = GetComponent<MeshRenderer>();
        selfTransform = transform;
        if (target != null)
            targetTransform = target.transform;
    }

    // Update target transform when target changes
    private void UpdateTargetReference(Player newTarget)
    {
        target = newTarget;
        targetTransform = newTarget?.transform;
    }
    public void PlayAppearEffect()
    {
        SetDataByCharacter(GetRandomCharacterData());
        if (effect_appear != null)
        {
            effect_appear.AnimationState.SetAnimation(0, "animation_0", false);
            StartCoroutine(DestroyEffectAfterDelay(effect_appear.gameObject, 1f));
        }
    }
    private IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        meshRenderer.enabled = true;
        Destroy(effect);
        skeletonAnimation.AnimationState.SetAnimation(0, "show", false);
                healthBar.slider.gameObject.SetActive(true);

        StartCoroutine(StartBoss(1.5f));


    }

    private IEnumerator StartBoss(float delay)
    {
        yield return new WaitForSeconds(delay);
        canMove = true;
    }

    public override void Update()
    {
        base.Update();
        if (!canMove) return;
        if (isUseSkill)
        {
            return;
        };
        if (isDead) return;

        HandleAI();
    }
    protected override void Die()
    {
        base.Die();
        StartCoroutine(Dispawn());
        StartCoroutine(KillBossRequest());
    }
 private Character GetRandomCharacterData()
    {
        return new Character
        {
            hp = Random.Range(1, 50)*100,
            attackMin = Random.Range(5, 10),
            attackMax = Random.Range(2, 10)*15,
            defense= Random.Range(0, 10),
            moveSpeed = Random.Range(1, 2),
            attackSpeed = Random.Range(1, 3)
        };
    }

    private IEnumerator Dispawn()
    {
        yield return new WaitForSeconds(1.5f);
        if (itemDropPrefab != null)
        {
            ObjectPool.SpawnFromPool(itemDropPrefab, transform.position, Quaternion.identity, UIOnMap.transform);
        }
        gameObject.SetActive(false); // Instead of destroying, return to pool
        ObjectPool.ReturnToPool(gameObject);
    }

    public void SetDataByCharacter(Character data)
    {
        maxHP = data.hp;
        attackMin = data.attackMin;
        attackMax = data.attackMax;
        speed = data.moveSpeed;
        attackSpeed = data.attackSpeed;
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (!canMove) return;
        if (isUseSkill)
        {
            rb.velocity = Vector2.zero;
            return;
        };
        if (!isChasing && !isAttacking && !isDead && target == null)
        {

            MoveRandomly();
            if (target == null)
            {
                target = FindNearestPlayer();
            }
        }
        if (!isChasing && !isAttacking && !isDead && target != null)
        {
            MoveTowardsTarget();
        }
        if (target == null || target.isDead)
        {
            isChasing = false;
            isAttacking = false;
        }
    }

    private void HandleAI()
    {
        if (isDead) return;
        if (target == null || target.isDead)
        {
            target = FindNearestPlayer();
        }

        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance <= detectionRange)
        {
            if (distance <= attackRange && !isUseSkill)
            {
                if (Random.value <= 0.8f)
                {
                    rb.velocity = Vector2.zero;
                    TryAttackPlayer();
                    isChasing = true;
                }
                else
                {
                    TryUseRandomSkill();
                }
            }
            else
            {
                isChasing = false;
                isAttacking = false;
                MoveTowardsTarget();
                if (!isChasing)
                {
                    TryUseRandomSkill();
                }




            }
        }
    }
    private void TryUseRandomSkill()
    {
        if (skills.Count > 0 && !isAttacking)
        {
            Skill randomSkill = skills[Random.Range(0, skills.Count)];


            UseSkill(randomSkill, target);


        }
    }

    private float nextPhysicsCheck;
    private const float PHYSICS_CHECK_INTERVAL = 0.2f; // Check every 0.2 seconds instead of every frame

    private Player FindNearestPlayer()
    {
        if (Time.time < nextPhysicsCheck) return target;

        nextPhysicsCheck = Time.time + PHYSICS_CHECK_INTERVAL;
        Vector3 center = boxCollider2D != null ? boxCollider2D.bounds.center : selfTransform.position;

        Collider2D[] playersInRange = new Collider2D[10]; // Preallocate array
        int hitCount = Physics2D.OverlapCircleNonAlloc(center, detectionRange, playersInRange, playerLayer);

        Player nearestPlayer = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < hitCount; i++)
        {
            Player player = playersInRange[i].GetComponent<Player>();
            if (player != null && !player.isDead)
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


    private void MoveRandomly()
    {
        if (randomMoveTimer <= 0f)
        {
            randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            randomMoveTimer = randomMoveInterval;
        }
        else
        {
            randomMoveTimer -= Time.deltaTime;
        }

        rb.velocity = randomDirection * speed;

        Debug.DrawLine(transform.position, (Vector2)transform.position + randomDirection * speed * Time.deltaTime, Color.red, 0.5f);

        if (randomDirection.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

    }


    private void MoveTowardsTarget()
    {
        if (targetTransform != null)
        {
            Vector2 currentPos = selfTransform.position;
            Vector2 targetPos = targetTransform.position;
            Vector2 direction = (targetPos - currentPos);
            float distance = direction.magnitude;

            if (distance > 0.01f) // Avoid normalization if too close
            {
                direction /= distance; // Normalize
                rb.velocity = direction * speed;
                selfTransform.localScale = new Vector3(direction.x < 0 ? 1 : -1, 1, 1);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            MoveRandomly();
        }
    }

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
            Attack();
            target.TakeDamage(Random.Range(attackMin, attackMax));
        }

    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            randomDirection = -randomDirection;
        }
    }


    private void OnDrawGizmos()
    {
        Vector3 center = transform.position;

        if (boxCollider2D != null)
        {
            center = boxCollider2D.bounds.center;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(center, randomDameTextRange);
    }
     [System.Serializable]
    public class KillBossRq
    {
        public string bossCode;
    }
   public class KillBossRs
    {
        public string success;
        public Rune newRune;
        public string message;
    }
    private IEnumerator KillBossRequest()
    {
        KillBossRq killBossData = new KillBossRq
        {
            bossCode = DataManager.Instance.Get<string>("bossCode"),
        };
        string jsonData = JsonUtility.ToJson(killBossData);
        Debug.Log(jsonData);
        using (UnityWebRequest request = new UnityWebRequest(DataManager.Instance.SERVER_URL + "/api/game/kill-boss", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.SetRequestHeader("Authorization", $"Bearer {DataManager.Instance.Get<string>("token")}");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                KillBossRs killBossRs = JsonConvert.DeserializeObject<KillBossRs>(request.downloadHandler.text);
                PlayerDataManager.Instance.playerData.AddRune(killBossRs.newRune);
                        EventManager.FireEvent("OnBossManagerEnd",killBossRs.newRune.id);

            }

        }
    }
}
