using System.Collections;
using UnityEngine;
using Spine.Unity;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/Skill Data")]
public class Skill : ScriptableObject
{
    public string skillName;
    public float cooldownTime;
    public float chanceToTrigger;
    public string skillAnimation; // Animation name for the skill effect
    public string targetHitAnimation; // Animation name for the target's reaction
    public float effectValue;
    public GameObject skillEffectPrefab; // Visual effect to instantiate at the target
}
