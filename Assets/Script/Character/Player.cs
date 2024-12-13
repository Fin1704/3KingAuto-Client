using System.Collections;
using UnityEngine;

public class Player : CharacterBase
{
    public Vector3 homePosition;
    private Vector3 targetPosition;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public Enemy target;
    public LayerMask enemyLayer;
    private Vector2 randomDirection;
    private float randomMoveTimer = 0f;
    private float randomMoveInterval = 2f;
    private Coroutine movementCoroutine;
    private Coroutine regenCoroutine;
    public void SetDataByCharacter(Character data){
        maxHP=data.hp;
        attackMin=data.attackMin;
        attackMax=data.attackMax;
        speed=data.moveSpeed;
        attackSpeed=data.attackSpeed;
    }
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
        StartCoroutine(WaitToGoHome());
    }

    private IEnumerator WaitToGoHome()
    {
        yield return new WaitForSeconds(2f);
        MoveToHome();
    }

    void FixedUpdate()
    {
        if (!isChasing && !isAttacking && !isDead && target == null)
        {
            MoveRandomly();
        }
        else if (!isChasing && !isAttacking && !isDead && target != null)
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
            target = FindNearestEnemy();
        }

        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance <= detectionRange)
        {
            if (distance <= attackRange)
            {
                rb.velocity = Vector2.zero;
                TryAttackTarget();
                isChasing = true;
            }
            else
            {
                isChasing=false;
                isAttacking=false;
                MoveTowardsTarget();
            }
        }
        else
        {
            isChasing = false;
            rb.velocity = Vector2.zero;
        }
    }
    private Enemy FindNearestEnemy()
    {
        Vector3 center = transform.position;
        Collider2D boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            center = boxCollider2D.bounds.center;
        }

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(center, detectionRange, enemyLayer);

        Enemy nearestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !enemy.isDead)
            {
                // Tính toán khoảng cách chỉ khi cần thiết
                float distance = Vector2.Distance(center, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        return nearestEnemy;
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

        Vector2 velocity = randomDirection * speed;

        if (velocity.magnitude > speed)
        {
            velocity = velocity.normalized * speed;
        }

        rb.velocity = velocity;

        transform.localScale = new Vector3(randomDirection.x > 0 ? 1 : -1, 1, 1);
    }

    private void MoveTowardsTarget()
    {
         target = FindNearestEnemy();
        PlayAnimation(runName, true);
        if (target != null)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;

            // Giới hạn tốc độ di chuyển
            Vector2 velocity = direction * speed;
            if (velocity.magnitude > speed)
            {
                velocity = velocity.normalized * speed;
            }

            rb.velocity = velocity;

            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        }
        else
        {
            MoveRandomly();
        }
    }

    private void TryAttackTarget()
    {
        if (Time.time - lastAttackTime >= 1f / attackSpeed)
        {
            isAttacking = true;
            AttackTarget();
            lastAttackTime = Time.time;
            Invoke(nameof(EndAttack), 0.5f);
        }
    }

    private void AttackTarget()
    {
        if (target != null)
        {
            Vector3 direction = target.transform.position - transform.position;
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
            Attack();
            target.TakeDamage(Random.Range(attackMin, attackMax));
            if(target.isDead){
                target=FindNearestEnemy();
            }
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

   public void MoveToHome()
{
    // Xác định phạm vi ngẫu nhiên xung quanh homePosition
    float randomOffsetX = Random.Range(-1f, 1f); // Phạm vi ngẫu nhiên theo trục X
    float randomOffsetY = Random.Range(-1f, 1f); // Phạm vi ngẫu nhiên theo trục Y

    // Tính toán vị trí mới (2D chỉ cần X và Y)
    Vector2 randomPosition = new Vector2(
        homePosition.x + randomOffsetX,
        homePosition.y + randomOffsetY
    );

    // Gán vị trí mục tiêu
    targetPosition = new Vector3(randomPosition.x, randomPosition.y, 0); // Đảm bảo Z = 0

    // Kiểm tra và dừng Coroutine nếu đang chạy
    if (movementCoroutine != null)
    {
        StopCoroutine(movementCoroutine);
    }

    isReviving = true;
    PlayAnimation(moveGoHomeName, true);

    // Bắt đầu Coroutine di chuyển tới vị trí mới
    movementCoroutine = StartCoroutine(MoveTowards(targetPosition));
}


    private IEnumerator MoveTowards(Vector3 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;

            // Giới hạn tốc độ di chuyển
            Vector2 velocity = direction * speed;
            if (velocity.magnitude > speed)
            {
                velocity = velocity.normalized * speed;
            }

            rb.velocity = velocity;

            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);

            yield return null;
        }

        if (regenCoroutine == null)
        {
            PlayAnimation(recoveryName, true);
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }

        rb.velocity = Vector2.zero;
    }

    private IEnumerator RegenerateHealth()
    {
        while (currentHP < maxHP)
        {
            currentHP += maxHP * 0.1f;
            currentHP = Mathf.Min(currentHP, maxHP);
            ShowRecovery(maxHP * 0.1f);
            healthBar.SetHealth(currentHP);
            yield return new WaitForSeconds(1f);
        }

        target = FindNearestEnemy();
        regenCoroutine = null;
        isDead = false;
        isReviving = false;
        boxCollider2D.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            randomDirection = -randomDirection;
        }
        else if (collision.gameObject.CompareTag("Enity"))
        {
           rb.velocity = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(homePosition, new Vector3(0.1f, 0.1f, 0));
    }
}
