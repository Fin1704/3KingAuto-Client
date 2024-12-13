using System;
using Spine.Unity;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterBase : MonoBehaviour
{
    public string attackName="sword_attack";
    public string runName="run";
    public string idleName="idle_1";
    public string deathName="dead";
    public string moveGoHomeName="walk2";
    public string recoveryName="skid";
    public SkeletonAnimation skeletonAnimation;
    protected Canvas UIOnMap;
    public string characterName;
    public float maxHP = 100f;
    public float currentHP;
    public float attackMin = 5f;
    public float attackMax = 10f;

    public float speed = 3f;
    public float defense = 1f;
    public float attackSpeed = 1f;
    public bool isDead;
    public bool isChasing = false;
    public bool isAttacking = false;
    public TMP_Text damageText;
        public TMP_Text recoveryText;

    public float randomDameTextRange = 0.5f;
    public HealthBar healthBar;
    public BoxCollider2D boxCollider2D;
    private string currentAnimation = "";
    protected Rigidbody2D rb;
    public bool isReviving = false;
    protected float lastAttackTime = 0f;

    public virtual void Start()
    {
        currentHP = maxHP;
        isDead = false;
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        GameObject canvasObject = GameObject.Find("UIOnMap");
        UIOnMap=canvasObject.GetComponent<Canvas>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHP);
        }
    }

    public virtual void Update()
    {
        if (!boxCollider2D)
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
        }
        healthBar.SetByXDirection(transform.localScale.x);
        if (isDead)
        {
            if(!isReviving){
                rb.velocity = Vector2.zero;
            }
            if (boxCollider2D.enabled)
            {

                boxCollider2D.enabled = false;
            }

            return;
        }

        if (!isChasing)
        {
            if (!boxCollider2D.enabled)
            {
                boxCollider2D.enabled = true;
            }

            if (rb.velocity.magnitude > 0.3f)
            {
                PlayAnimation(runName, true);
            }
            else
            {
                PlayAnimation(idleName, true);
            }
        }

    }

    public void Attack()
    {
        if (boxCollider2D.enabled)
        {
            boxCollider2D.enabled = false;
        }
        PlayAnimation(attackName, true);
    }


    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP -= damage;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHP);
            ShowDamage(damage);
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {

        PlayAnimation(deathName, false);
        isDead = true;
        isChasing = false;
        isAttacking = false;
    }

    protected void PlayAnimation(string animationName, bool loop)
    {
        if (isDead && isReviving == false) return;
        if (currentAnimation != animationName || (currentAnimation == animationName && !loop))
        {
            skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
            currentAnimation = animationName;
        }
    }
    private void ShowDamage(float damage)
    {
        if (damageText != null)
        {
            Vector3 center = transform.position;
            if (boxCollider2D != null)
            {
                center = boxCollider2D.bounds.center;
            }

            Vector3 randomOffset = new Vector3(
                Random.Range(-randomDameTextRange * 0.5f, randomDameTextRange * 0.5f),
                Random.Range(-randomDameTextRange * 0.5f, randomDameTextRange * 0.5f),
                0
            );
            TMP_Text instance = Instantiate(damageText, center + randomOffset, Quaternion.identity, UIOnMap.transform);
            instance.text = "-" + damage.ToString("F0");
            instance.gameObject.SetActive(true);

            float moveDistance = 0.5f;
            float duration = 0.5f;
            Vector3 targetPosition = instance.transform.position + new Vector3(0, moveDistance, 0);

            LeanTween.move(instance.gameObject, targetPosition, duration).setEase(LeanTweenType.easeOutQuad);
            Destroy(instance.gameObject, duration);
        }
    }
    protected void ShowRecovery(float value)
    {
        if (recoveryText != null)
        {
            Vector3 center = transform.position;
            if (boxCollider2D != null)
            {
                center = boxCollider2D.bounds.center;
            }

            Vector3 randomOffset = new Vector3(
                Random.Range(-randomDameTextRange * 0.5f, randomDameTextRange * 0.5f),
                Random.Range(-randomDameTextRange * 0.5f, randomDameTextRange * 0.5f),
                0
            );
            TMP_Text instance = Instantiate(recoveryText, center + randomOffset, Quaternion.identity, UIOnMap.transform);
            instance.text = "+" + value.ToString("F0");
            instance.gameObject.SetActive(true);

            float moveDistance = 0.5f;
            float duration = 0.5f;
            Vector3 targetPosition = instance.transform.position + new Vector3(0, moveDistance, 0);

            LeanTween.move(instance.gameObject, targetPosition, duration).setEase(LeanTweenType.easeOutQuad);
            Destroy(instance.gameObject, duration);
        }
    }

}
