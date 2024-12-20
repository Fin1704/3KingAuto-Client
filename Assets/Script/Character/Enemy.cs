using System.Collections;
using UnityEngine;


public class Enemy : CharacterBase
{
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public Player target;
    public LayerMask playerLayer;
    private Vector2 randomDirection;
    private float randomMoveTimer = 0f;
    private float randomMoveInterval = 2f;
    public GameObject itemDropPrefab;
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        if (isDead) return;

        HandleAI();
    }
    protected override void Die()
    {
        base.Die();
        StartCoroutine(Dispawn());
    }
    public void SetDataByCharacter(Character data)
    {
        maxHP = data.hp;
        attackMin = data.attackMin;
        attackMax = data.attackMax;
        speed = data.moveSpeed;
        attackSpeed = data.attackSpeed;
    }
    private IEnumerator Dispawn()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
        if (itemDropPrefab != null)
        {

            GameObject item = Instantiate(itemDropPrefab, transform.position, Quaternion.identity, UIOnMap.transform);
            // ItemDrop itemDrop = item.GetComponent<ItemDrop>();
        }
    }
    void FixedUpdate()
    {

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
        if (target == null || target.isDead)
        {
            target = FindNearestPlayer();
        }

        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance <= detectionRange)
        {
            if (distance <= attackRange)
            {
                rb.velocity = Vector2.zero;
                TryAttackPlayer();
                isChasing = true;
            }
            else
            {
                isChasing = false;
                isAttacking = false;
                MoveTowardsTarget();
            }
        }
        else
        {
            isChasing = false;
            rb.velocity = Vector2.zero;
        }
    }

    private Player FindNearestPlayer()
    {
        Vector3 center = transform.position;
        if (boxCollider2D != null)
        {
            center = boxCollider2D.bounds.center;
        }

        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(center, detectionRange, playerLayer);

        Player nearestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D playerCollider in playersInRange)
        {
            Player player = playerCollider.GetComponent<Player>();
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

        if (randomDirection.x > 0)
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
        if (target != null)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            rb.velocity = direction * speed;
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
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
            target.TakeDamage(Random.Range(attackMin, attackMax));
        }
        Attack();
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
}
