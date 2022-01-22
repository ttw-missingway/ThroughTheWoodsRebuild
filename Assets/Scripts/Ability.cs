using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;

namespace TTW.Combat
{
    public enum TargetingType       { none, melee, beeline, volley, support, self, cell, allallies, allfoes, supportNotSelf, cardinalAlly, supportAdjacent, gridRelative, gridGlobal, obstacle, random, ordinal };
    public enum DamageType          { nonDamage, physical, magical, healing };
    public enum MagicType           { none, sun, moon, lamp, electric, umbra }
    public enum NeutralState        { none, guard, protection, counter, cloak, invulnerable, guardian };
    public enum Displacement        { none, dash, swap, pull, push, leap, global, warp, random };
    public enum StatusEffectType    { none, blind, burn, madness, shock, regen, rebirth, bubble, mirror }
    public enum AttackVariant       { none, lifesteal, critical, splash, armorBreak, bewilder, dispel };


    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]

    public class Ability : ScriptableObject
    {
        [Header("Name and Description")]
        public new string name;
        public string description;
        public Sprite artwork;

        [Header("Animation Handler")]
        public AbilityAnimationController abilityAnimation;
        public CombatAnimStates animationState;
        public VFX targetEffect;
        public VFX casterEffect;
        public VFX projectile;
        public VFX stageEffect;
        public VFX cellEffect;
        public Vector2Int[] fxCells;
        public bool fxCellsSameAsTargetCells;
        public bool lookAtTarget;

        [Header("Targeting")]
        public TargetingType targetingType;
        public Vector2Int[] gridTargetingCoordinates;
        public bool canTargetObstacles;

        [Header("Timers")]
        public float attackChannelTime;
        public float attackCD;
        public float aiWaitTime;

        [Header("Damage and Healing")]
        public DamageType damageType;
        public MagicType magicType;
        public float damageFlat;
        public bool canRevive;

        [Header("Accuracy")]
        [Range(0f, 100f)] public float abilityAccuracy;

        [Header("Neutral State")]
        public NeutralState neutralState;

        [Header("Status Effects")]
        public StatusEffectType statusEffect;
        public float statusEffectTimer;

        [Header("Buffing and Nerfing")]
        public Stats statToChange;
        [Range(-3, 3)]public int statChangeValue;

        [Header("Attack Variants")]
        public AttackVariant attackVariant;

        [Header("Displacement")]
        public Displacement displacement;
        public DirectionTypes globalDirection;

        [Header("Crowd Scaling")]
        public bool crowdScaled;

        [Header("Object Creation")]
        public Ability trapAbility;
        public Trap trapShell;
        public bool useGridTargeting;
        public Obstacle obstacleCreate;

        [Header("Chained Ability")]
        public bool isChainedAbility;
        public Ability chainedAbility;
        public float chainDelayTime;
        public CasterProxy casterProxy;

        [Header("Recoil")]
        public bool recoil;
        [Range(0, 100)] public float recoilPercent;

        [Header("Success Rates")]
        [Range(0, 100)] public float counterSuccessRate;
        [Range(0, 100)] public float trapSuccessRate;

        [Header("Special Rules")]
        public bool enchantment;
        public bool refresh;
        public bool legendary;
        public bool neverMiss;

        [Header("Summon Enemy Entity")]

        [Header("Enemy Ability Section")]
        public EnemyEntity[] enemyEntity;
        public int entityMax;
        public bool wingsOnly;
        public VFX summonFX;

        [Header("Change Form")]
        public Enemy enemyChange;
        public GameObject enemyPrefab;

        [Header("Rails")]
        public bool usesRails;
        public bool relative;
        public Vector2Int startingCell;
        public Vector2Int endingCell;
        public PathManager starboardPath;
        public PathManager portPath;
        public PathManager bowPath;

        [Header("AI tags")]
        public bool support;
    }
}