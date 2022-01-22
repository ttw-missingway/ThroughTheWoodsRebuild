using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SWS;

namespace TTW.Combat
{
    public class AttackExecutor : MonoBehaviour
    {
        StatModifierTool statModifierTool;
        [SerializeField] float criticalMultiplier = 3f;
        [SerializeField] float residualMultiplier = 0.5f;

        float rawDamage = 0f;

        private void Awake()
        {
            statModifierTool = new StatModifierTool(FindObjectOfType<PublicValues>());
        }

        public Targetable Targetable => GetComponent<Targetable>();

        public void ReceiveAttackPacket(AttackPacket packet)
        {
            FinalizeTargets(ref packet);

            if (packet.Caster.GetComponent<StatusHandler>().Madness)
            {
                var newTarget = ApplyMadness();
                packet.ClearAllTargets();
                packet.AddNewTarget(newTarget);
            }

            if (packet.TargetsCount() == 0)
            {
                print(packet.Ability + " hit NO targets!");
            }

            AbilityUtilityTool utilityTool = new AbilityUtilityTool();

            PerformAbility(ref utilityTool, ref packet);
            ExecuteAttackPacket(utilityTool, packet);

            if (packet.Ability.isChainedAbility) return;
            if (packet.Counter) return;

            SetCasterCooldown(packet.Caster.GetComponent<Cooldown>(), packet.Ability.attackCD);
        }

        private void FinalizeTargets(ref AttackPacket packet)
        {

            if (packet.TargetsCount() == 0 && !packet.Ability.isChainedAbility && !packet.Ability.usesRails) return;
            //if (packet.Counter) return;

            var allTargetables = FindObjectsOfType<Targetable>();
            TargetingTool targetingTool = new TargetingTool(allTargetables.ToList(), packet.Ability, packet.Caster.Targetable, packet.Caster.Targetable.GetTargetClass());
            BattleGrid battleGrid = BattleGrid.singleton;

            var targetablesInPosition = targetingTool.SortTargetables();

            if (CancelRails(ref packet, targetingTool, battleGrid)) return;

            if (packet.Ability.targetingType == TargetingType.allallies ||
                packet.Ability.targetingType == TargetingType.allfoes ||
                packet.Ability.targetingType == TargetingType.gridGlobal ||
                packet.Ability.targetingType == TargetingType.gridRelative ||
                packet.Ability.targetingType == TargetingType.ordinal)
            {
                packet.WriteNewTargets(targetablesInPosition);
            }
            else
            {
                if (targetablesInPosition.Contains(packet.FirstTargetable()))
                {
                    var addTarget = packet.FirstTargetable();
                    packet.ClearAllTargets();
                    packet.AddNewTarget(addTarget);
                }
                else if (packet.Ability.isChainedAbility && targetablesInPosition.Count > 0)
                {
                    packet.ClearAllTargets();
                    packet.AddNewTarget(targetablesInPosition[0]);
                }
                else
                {
                    CancelAttack(ref packet);
                }
            }
        }

        private bool CancelRails(ref AttackPacket packet, TargetingTool targetingTool, BattleGrid battleGrid)
        {
            if (packet.Ability.usesRails)
            {
                if (!packet.Ability.relative)
                {
                    if (battleGrid.allCells[packet.Ability.endingCell].IsOccupied)
                    {
                        CancelAttack(ref packet);
                        return true;
                    }
                }
                else
                {
                    if (GetComponent<GridPosition>().GetWing() == Wing.Bow) return true;

                    print(battleGrid.allCells[targetingTool.TranslateCoordinates(packet.Ability.endingCell, GetComponent<GridPosition>().GetGridPos(), GetComponent<GridPosition>().GetWing())]);

                    if (battleGrid.allCells[targetingTool.TranslateCoordinates(packet.Ability.endingCell, GetComponent<GridPosition>().GetGridPos(), GetComponent<GridPosition>().GetWing())].IsOccupied)
                    {
                        CancelAttack(ref packet);
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        private void CancelAttack(ref AttackPacket packet)
        {
            packet.CancelAttack(); 
        }

        private void PerformAbility(ref AbilityUtilityTool utilityTool, ref AttackPacket packet)
        {
            var caster = packet.Caster.GetComponent<Targetable>();
            var casterStats = packet.Caster.GetComponent<StatsHandler>();
            var ability = packet.Ability;
            var targets = packet.GetTargets();

            if (GetComponent<SharedDamage>() != null)
                casterStats = GetComponent<SharedDamage>().DamageCore;

            rawDamage = ability.damageFlat;

            if (ability.crowdScaled)
                rawDamage = ability.damageFlat / targets.Count();

            caster.GetComponent<StatusHandler>().BreakState();

            switch (packet.Ability.damageType)
            {
                case DamageType.healing:
                    ProcessHealing(ref packet, casterStats);
                    break;
                case DamageType.physical:
                case DamageType.magical:
                    ProcessDamage(ref packet, casterStats);
                    break;
            }

            if (packet.Ability.enchantment)
                Enchant(ref packet);

            if (packet.Ability.attackVariant == AttackVariant.bewilder)
                Bewilder(ref packet);

            if (packet.Ability.legendary)
                packet.Caster.GetComponent<AbilitySlots>().UseLegendary();

            if (packet.Ability.recoil)
                Recoil(packet);

            if (GetComponent<StatusHandler>().GetEnchantmentSlot() != null)
                if (packet.Ability.damageType == DamageType.physical)
                    PerformEnchantment(caster, targets);

            if (packet.Ability.chainedAbility != null)
                PerformChainedAbility(caster, targets, packet.Ability.chainedAbility);

            utilityTool.LoadPacket(packet);
        }

        private void ExecuteAttackPacket(AbilityUtilityTool utilityTool, AttackPacket packet)
        {
            var animationHandler = Instantiate(packet.Ability.abilityAnimation);
            animationHandler.LoadUtilityTool(utilityTool);
            LoadVFX(packet, animationHandler);
            animationHandler.SetAbilityCam();
            animationHandler.AddCaster(packet.Caster.GetComponent<Targetable>());
            animationHandler.ReceiveAttackPacket(packet);
        }

        private static void LoadVFX(AttackPacket packet, AbilityAnimationController animationHandler)
        {
            if (packet.Ability.targetEffect != null)
                animationHandler.LoadVFX(packet.Ability.targetEffect, VFXType.target);
            if (packet.Ability.casterEffect != null)
                animationHandler.LoadVFX(packet.Ability.casterEffect, VFXType.caster);
            if (packet.Ability.stageEffect != null)
                animationHandler.LoadVFX(packet.Ability.stageEffect, VFXType.stage);
            if (packet.Ability.projectile != null)
                animationHandler.LoadVFX(packet.Ability.projectile, VFXType.projectile);
            if (packet.Ability.cellEffect != null)
                animationHandler.LoadVFX(packet.Ability.cellEffect, VFXType.cell);
        }

        private Targetable ApplyMadness()
        {
            var allTargetables = FindObjectsOfType<Targetable>();
            var nonCellTargets = from Targetable t in allTargetables
                                 where t.GetComponent<Cell>() == null
                                 select t;

            nonCellTargets.ToList();

            var random = new System.Random();
            int index = random.Next(nonCellTargets.Count());
            var target = nonCellTargets.ElementAt(index);

            return target;
        }

        private void PerformEnchantment(Targetable caster, List<Targetable> targets)
        {
            var enchantment = caster.GetComponent<StatusHandler>().GetEnchantmentSlot();
            caster.GetComponent<StatusHandler>().ClearEnchantmentSlot();

            AttackPacket enchantmentPacket = new AttackPacket(enchantment, this, targets);

            ReceiveAttackPacket(enchantmentPacket);
        }

        private void PerformChainedAbility(Targetable caster, List<Targetable> targets, Ability chainedAbility)
        {
            AttackPacket chainPacket = new AttackPacket(chainedAbility, this, targets);

            var casterProxy = Instantiate(chainedAbility.casterProxy);
            casterProxy.SetPacket(chainPacket);
        }

        private void Enchant(ref AttackPacket packet)
        {
            packet.Enchantment = packet.Caster.GetComponent<StatusHandler>().GetEnchantedAbility();
        }

        private void Bewilder(ref AttackPacket packet)
        {
            packet.BewilderTimer = statModifierTool.BewilderAdjustment(packet.Caster.Targetable.Stats);
        }

        private float ProcessMagic(Ability ability, StatsHandler casterStats)
        {
            float damage = rawDamage;

            switch (ability.magicType)
            {
                case MagicType.sun:
                    damage *= statModifierTool.SunLightAdjustment(casterStats);
                    break;
                case MagicType.moon:
                    damage *= statModifierTool.MoonLightAdjustment(casterStats);
                    break;
                case MagicType.lamp:
                    damage *= statModifierTool.LampLightAdjustment(casterStats);
                    break;
                case MagicType.electric:
                    damage *= statModifierTool.ElectricLightAdjustment(casterStats);
                    break;
                case MagicType.umbra:
                    damage *= statModifierTool.UmbraAdjustment(casterStats);
                    break;
            }

            return damage;
        }

        private void ProcessHealing(ref AttackPacket packet, StatsHandler casterStats)
        {
            float rawHealing = packet.Ability.damageFlat;
            float healing = rawHealing;
            float finalHealing = healing;

            finalHealing *= statModifierTool.BaseHealingAdjustment(casterStats);

            packet.Damage = finalHealing;
        }

        private void ProcessDamage(ref AttackPacket packet, StatsHandler casterStats)
        {
            float damage = rawDamage;
            float finalDamage;

            if (packet.Ability.damageType == DamageType.magical)
            {
                damage = ProcessMagic(packet.Ability, casterStats);
            }
            else if (packet.Ability.damageType == DamageType.physical)
            {
                damage *= statModifierTool.BaseDamageAdjustment(casterStats);
            }

            if (packet.Ability.targetingType == TargetingType.melee)
                damage *= statModifierTool.MeleeAdjustment(casterStats);

            if (packet.Ability.targetingType == TargetingType.beeline)
                damage *= statModifierTool.BeelineAdjustment(casterStats);

            if (packet.Ability.attackVariant == AttackVariant.critical) damage = CriticalRoll(casterStats, damage, ref packet);

            finalDamage = damage;

            packet.Damage = finalDamage;
        }

        private void Recoil(AttackPacket packet)
        {
            if (packet.EventChain > 0) return;

            AttackPacket recoilPacket = new AttackPacket(packet.Ability, this, Targetable);
            recoilPacket.NeutralSettings();

            recoilPacket.EventChain++;

            recoilPacket.Damage = packet.Damage * (packet.Ability.recoilPercent * 0.01f) * statModifierTool.RecoilAdjustment(packet.Caster.Targetable.Stats) ;

            ReceiveAttackPacket(recoilPacket);
        }

        private float CriticalRoll(StatsHandler casterStats, float damage, ref AttackPacket packet)
        {
            float rollThreshold = statModifierTool.CritChanceAdjustment(casterStats);
            float randomRoll = UnityEngine.Random.Range(0, 100);

            if (randomRoll < rollThreshold)
            {
                print("CRITICAL HIT!");
                packet.Crit = true;
                return damage * statModifierTool.CritDamageAdjustment(casterStats);
            } 
            else
                return damage;
        }

        private void SetCasterCooldown(Cooldown caster, float cdTime)
        {
            if (caster.IsOnCooldown) return;

            caster.SetCooldown(cdTime);
        }
    }
}