using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterBase : MonoBehaviour
{


    [SerializedDictionary("Animation", "Value")]
    public SerializedDictionary<string, string> animationList;
    public SkeletonAnimation skeletonAnimation;
    protected Canvas UIOnMap;
    public string characterName;
    public float maxHP = 100f;
    public float currentHP;
    public float attackMin = 5f;
    public float attackMax = 10f;
    public Vector2 boxColliderCenter;
    public float speed = 3f;
    public float defense = 1f;
    public float attackSpeed = 1f;
    public bool isDead = false;
    public bool isUseSkill = false;

    public bool isChasing = false;
    public bool isAttacking = false;
    public TMP_Text damageText;
    public TMP_Text recoveryText;

    public float randomDameTextRange = 0.5f;
    public HealthBar healthBar;
    protected BoxCollider2D boxCollider2D;
    private string currentAnimation = "";
    protected Rigidbody2D rb;
    protected bool isReviving = false;
    protected float lastAttackTime = 0f;
    public List<Skill> skills;
    private Dictionary<string, float> skillCooldownTimers = new Dictionary<string, float>();
private int baseSortingOrder = 0; 
    public virtual void Start()
    {

        currentHP = maxHP;
        isDead = false;
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        GameObject canvasObject = GameObject.Find("UIOnMap");
        UIOnMap = canvasObject.GetComponent<Canvas>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHP);
        }
    }
    public void UseSkill(Skill skill, CharacterBase target)
    {
        if ((!isUseSkill) && (!skillCooldownTimers.ContainsKey(skill.skillName) || skillCooldownTimers[skill.skillName] <= 0f))
        {
            if (Random.value <= skill.chanceToTrigger)
            {
                isUseSkill = true;
                isAttacking=false;
                isChasing=false;
                       skeletonAnimation.AnimationState.SetAnimation(0, skill.skillAnimation, false);


                StartCoroutine(TriggerSkillEffect(skill, target));
                skillCooldownTimers[skill.skillName] = skill.cooldownTime;
            }

        }

    }

    // Coroutine to trigger the skill effect and apply damage to the target
    private IEnumerator TriggerSkillEffect(Skill skill, CharacterBase target)
    {
        if (target == null)
        {
            Debug.LogError("Target is null. Cannot proceed.");
            yield break;
        }

        // Get the target's SkeletonAnimation component
        SkeletonAnimation targetAnimation = target.GetComponent<SkeletonAnimation>();
       
        Vector2 spawnPosition = target.boxColliderCenter + new Vector2(0.5f, 0); ;
        GameObject effectInstance = Instantiate(skill.skillEffectPrefab, spawnPosition, Quaternion.identity);

        SkeletonAnimation skeletonAnimation_skill = effectInstance.GetComponent<SkeletonAnimation>();
        skeletonAnimation_skill.GetComponent<MeshRenderer>().sortingOrder = 20000-Mathf.RoundToInt(effectInstance.transform.position.y*10)+ 1;


        skeletonAnimation_skill.AnimationState.SetAnimation(0, "animation", false);
        

        float animationDuration_skill = skeletonAnimation_skill.Skeleton.Data.FindAnimation("animation").Duration;
        yield return new WaitForSeconds(animationDuration_skill);
        

        Destroy(effectInstance);


        // Play the target's hit animation
        // if (targetAnimation != null)
        // {
        //     targetAnimation.AnimationState.SetAnimation(0, skill.targetHitAnimation, false);
        // }

        // Apply damage to the target
        target.TakeDamage(skill.effectValue);
        isUseSkill = false;

    }
    public virtual void Update()
    {
        skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 20000-Mathf.RoundToInt(transform.position.y*10)+ baseSortingOrder;
        if (!boxCollider2D)
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
        }
        else
        {

            boxColliderCenter = boxCollider2D.bounds.center;

        }
        List<string> keys = new List<string>(skillCooldownTimers.Keys);
        foreach (var key in keys)
        {
            if (skillCooldownTimers[key] > 0f)
            {
                skillCooldownTimers[key] -= Time.deltaTime;
            }
        }
        healthBar.SetByXDirection(transform.localScale.x);
        if (isDead)
        {
            if (!isReviving)
            {
                rb.velocity = Vector2.zero;
                PlayAnimation(animationList["idle"], true);
            }
            if (boxCollider2D.enabled)
            {

                boxCollider2D.enabled = false;
            }

            return;
        }

        if (!isChasing && !isUseSkill)
        {
            if (!boxCollider2D.enabled)
            {
                boxCollider2D.enabled = true;
            }

            if (rb.velocity.magnitude > 0.3f)
            {
                PlayAnimation(animationList["run"], true);
            }
            else
            {
                PlayAnimation(animationList["idle"], true);
            }
        }

    }

    public void Attack()
    {
        if (boxCollider2D.enabled)
        {
            boxCollider2D.enabled = false;
        }
        PlayAnimation(animationList["attack"], true);
    }


    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;
        damage -= Random.Range(0, defense);
        if (damage<0)    {
            damage=0;
        }
        currentHP -= damage;
        if (currentHP<0){
            currentHP=0;
        }
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

        PlayAnimation(animationList["death"], false);
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
