using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class AttackReceiver : MonoBehaviour
    {
        //attack receiver determines:
        //damage intake calculated using targets stats
        //ability effects that affect the target
        //communicates with stats handler and status handler
        //offloads to pushable effects
        //does NOT interact with attack executor outside of receiving attack packet
        //does intake attack packets from other sources

        StatusHandler status;
        StatsHandler stats;
        Cooldown cooldown;
        StatModifierTool statModifierTool;
        PublicAbilities publicAbilities;

        VFX missVFX;
        VFX armorBreakVFX;
        VFX criticalVFX;

        private void Awake()
        {
            statModifierTool = new StatModifierTool(FindObjectOfType<PublicValues>());
            status = GetComponent<StatusHandler>();
            stats = GetComponent<StatsHandler>();
            cooldown = GetComponent<Cooldown>();
        }

        private void Start()
        {
            publicAbilities = PublicAbilities.singleton;
            criticalVFX = publicAbilities.critFX;
            missVFX = publicAbilities.missFX;
            armorBreakVFX = publicAbilities.armorBreakFX;
        }

        public void ReceiveAttackPacket(AttackPacket packet)
        {
            var ability = packet.Ability;
            var caster = packet.Caster.GetComponent<Targetable>();
            var target = GetComponent<Targetable>();

            if (BlackShield(packet)) return;

            if (!AccuracyCheck(ability, caster, target)) return;

            var damage = packet.Damage;
            string messageType = "damage";

            if (status.Bubble)
            {
                BubbleDamage(damage);
                return;
            }

            if (status.State == NeutralState.counter && packet.DamageType == DamageType.physical)
            {
                if (Counter(packet))
                    return;
            }

            if (status.Mirror && (packet.DamageType == DamageType.magical || packet.DamageType == DamageType.healing))
            {
                if (MagicMirror(packet))
                    return;
            }

            if (status.State == NeutralState.protection && GetComponent<StatusHandler>().Protector != null)
            {
                if (Protection(packet))
                    return;
            }

            if (status.State == NeutralState.guardian)
            {
                if (Guardian(packet))
                    return;
            }

            if (ability.neutralState == NeutralState.protection || ability.neutralState == NeutralState.guardian)
                SetProtection(packet);

            ProcessDamage(packet, ref damage, ref messageType);

            GetComponent<StatsHandler>().ChangeHealth(damage);

            if (packet.Crit)
            {
                Instantiate(criticalVFX, transform.position, Quaternion.identity);
            }

            if (packet.Ability.canRevive)
                GetComponent<StatsHandler>().Revive(damage);

            if (packet.Dispel)
                GetComponent<StatsHandler>().Dispel();

            status.BreakState();

            print(this.name + " takes " + damage + " " + messageType + " from " + packet.Ability.name + " cast by " + packet.Caster);

            if (packet.NeutralState != NeutralState.none)
                status.ChangeState(packet.NeutralState);

            if (packet.Stat != Stats.None)
                stats.ModifyStat(packet.Stat, packet.StatChange);

            if (packet.Status != StatusEffectType.none)
            {
                StatusSnapShot snapShot = new StatusSnapShot(packet.Caster, packet.Status, packet.StatusTimer);
                status.CreateNewStatus(snapShot, reactivate: false);
            }

            if (packet.Ability.attackVariant == AttackVariant.lifesteal)
                Vampirism(packet);

            if (packet.Ability.attackVariant == AttackVariant.splash)
                Splash(packet);

            if (packet.BewilderTimer > 0f)
                cooldown.AddToCooldown(packet.BewilderTimer);

            if (packet.Enchantment != null)
                status.SetEnchantmentSlot(packet.Enchantment);

            if (packet.Ability.refresh)
                cooldown.Refresh();
        }

        private void ProcessDamage(AttackPacket packet, ref float damage, ref string messageType)
        {
            switch (packet.DamageType)
            {
                case DamageType.physical:
                    damage = ProcessPhysicalDamage(packet);
                    break;
                case DamageType.magical:
                    damage = ProcessMagicalDamage(damage);
                    break;
                case DamageType.healing:
                    damage = ProcessHealing(damage);
                    messageType = "healing";
                    break;
            }
        }

        private bool BlackShield(AttackPacket packet)
        {

            if (GetComponent<BossEntity>() != null)
            {
                if (GetComponent<BossEntity>().BlackShield == null) return false;

                if (GetComponent<BossEntity>().BlackShield.GetActivated)
                {
                    if (packet.Ability.damageType == DamageType.magical && packet.Ability.magicType == MagicType.electric)
                    {
                        GetComponent<BossEntity>().BlackShield.ElectricLightUsed();
                    }

                    return true;
                }
            }

            return false;
        }

        private float GuardReduction(float damage)
        {
            return damage *= statModifierTool.GuardAdjustment(GetComponent<StatsHandler>());
        }

        private void BubbleDamage(float finalDamage)
        {
            var allChildrenStatusEffects = GetComponentsInChildren<StatusEffect>();
            var allBubbles = from StatusEffect s in allChildrenStatusEffects
                             where s.GetEffectType() == StatusEffectType.bubble
                             select s;
            allBubbles.ToList();
            var targetBubble = allBubbles.ElementAt(0);

            targetBubble.BubbleDamage(finalDamage);
        }

        private float ProcessPhysicalDamage(AttackPacket packet)
        {
            var rawDamage = packet.Damage;

            if (packet.AttackVariant == AttackVariant.armorBreak)
                rawDamage = ArmorBreak(packet.Caster.GetComponent<Targetable>(), rawDamage, GetComponent<Targetable>());

            if (status.State == NeutralState.guard)
            {
                GuardCooldownPunishment(packet);
                rawDamage = GuardReduction(rawDamage);
            }
                

            if (status.State == NeutralState.invulnerable && !packet.Ability.neverMiss)
                rawDamage = 0f;

            var def = statModifierTool.DefenseAdjustment(GetComponent<StatsHandler>());
            var att = rawDamage;

            var finalDamage = -1 * (att * att / (att + def));

            return finalDamage;
        }

        private void GuardCooldownPunishment(AttackPacket packet)
        {
            packet.Caster.GetComponent<Cooldown>().AddToCooldown(10f);
            //no magic numbers 
        }

        private float ProcessMagicalDamage(float rawDamage)
        {
            //var damageReduction = 0f;

            //damageReduction = rawDamage - rawDamage * (100 / (100 + (GetComponent<StatsHandler>().Spirit * publicValues.SpiritForMagicResistance)));

            //print(this.name + " reduced damage by " + damageReduction);

            var def = statModifierTool.MagicResAdjustment(GetComponent<StatsHandler>());
            var att = rawDamage;

            var finalDamage = -1 * (att * att / (att + def));

            return finalDamage;
        }

        private float ProcessHealing(float rawHealing)
        {
            //GetComponent<StatsHandler>().ChangeHealth(rawHealing);
            return rawHealing * statModifierTool.ReceivedHealingAdjustment(GetComponent<StatsHandler>());
        }

        private bool AccuracyCheck(Ability ability, Targetable caster, Targetable target)
        {
            //print("accuracy adjustment: " + AccuracyAdjustment(caster.Stats));
            //print("evasion adjustment: " + EvasionAdjustment(target.Stats));

            if (ability.damageType == DamageType.healing) return true;
            if (ability.targetingType == TargetingType.allallies) return true;
            if (ability.targetingType == TargetingType.cardinalAlly) return true;
            if (ability.targetingType == TargetingType.self) return true;
            if (ability.targetingType == TargetingType.support) return true;
            if (ability.targetingType == TargetingType.supportAdjacent) return true;
            if (ability.targetingType == TargetingType.supportNotSelf) return true;
            if (ability.neverMiss) return true;


            var abilityAccuracy = ability.abilityAccuracy * statModifierTool.AccuracyAdjustment(caster.Stats);
            var randomValue = UnityEngine.Random.Range(0f, 100f);
            var targetEvasion = randomValue * statModifierTool.EvasionAdjustment(target.Stats);

            if (caster.GetComponent<StatusHandler>().Blind)
            {
                abilityAccuracy *= 0.25f;
            } 

            if (targetEvasion > abilityAccuracy)
            {
                Instantiate(missVFX, transform.position, Quaternion.identity);
                print(ability.name + " MISSED " + target.name + "!");
                return false;
            }

            return true;
        }

        private float ArmorBreak(Targetable caster, float damage, Targetable target)
        {
            List<NeutralState> listOfGuardStates = new List<NeutralState> { NeutralState.cloak, NeutralState.counter, NeutralState.guard, NeutralState.guardian };

            if (listOfGuardStates.Contains(target.GetComponent<StatusHandler>().State))
            {
                target.GetComponent<StatusHandler>().ChangeState(NeutralState.none);
                damage *= statModifierTool.ArmorBreakAdjustment(caster.Stats);
                Instantiate(armorBreakVFX, transform.position, Quaternion.identity);
            }

            return damage;
        }

        private bool Counter(AttackPacket packet)
        {
            if (packet.EventChain >= 1) return false;

            GetComponent<StatusHandler>().BreakState();

            float randomRoll = Random.Range(0f, 100f);
            float counterThreshold = GetComponent<StatusHandler>().GetCounterAbility().counterSuccessRate + statModifierTool.CounterAdjustment(GetComponent<StatsHandler>());

            if (randomRoll > counterThreshold)
            {
                print("COUNTER UNSUCCESSFUL!");
                return false;
            }

            print(packet.Ability + " COUNTERED! by " + this);

            var ability = GetComponent<StatusHandler>().GetCounterAbility();

            Targetable[] allTargetables = FindObjectsOfType<Targetable>();
            TargetingTool tTool = new TargetingTool(allTargetables.ToList(), ability, GetComponent<Targetable>(), GetComponent<Targetable>().GetTargetClass());
            List<Targetable> allTargets = tTool.SortTargetables();
            List<Targetable> finalTargets = new List<Targetable>();

            if (ability.targetingType == TargetingType.allallies||
                ability.targetingType == TargetingType.allfoes||
                ability.targetingType == TargetingType.ordinal||
                ability.targetingType == TargetingType.gridGlobal||
                ability.targetingType == TargetingType.gridRelative)
            {
                foreach (Targetable t in allTargets)
                {
                    finalTargets.Add(t);
                }
            }
            else if (allTargets.Contains(packet.Caster.GetComponent<Targetable>()))
            {
                finalTargets.Add(packet.Caster.GetComponent<Targetable>());
            }
            else
            {
                int randomIndex = Random.Range(0, allTargets.Count() - 1);
                finalTargets.Add(allTargets[randomIndex]);
            }

            AttackPacket counterAttack = new AttackPacket(ability, GetComponent<AttackExecutor>(), finalTargets);
            counterAttack.EventChain = packet.EventChain;
            counterAttack.Counter = true;
            counterAttack.EventChain++;

            GetComponent<AttackExecutor>().ReceiveAttackPacket(counterAttack);

            return true;
        }

        private bool MagicMirror(AttackPacket packet)
        {   
            if (packet.EventChain >= 1) return false;

            print(packet.Ability + " MIRRORED! by " + this);

            var originalCaster = packet.Caster.GetComponent<Targetable>();

            List<Targetable> newTargets = new List<Targetable>();
            newTargets.Add(originalCaster);

            packet.WriteNewTargets(newTargets);
            packet.EventChain++;

            GetComponent<AttackExecutor>().ReceiveAttackPacket(packet);

            return true;
        }

        private void SetProtection(AttackPacket packet)
        {
            //target.GetComponent<StatusHandler>().ChangeState(ability.neutralState);
            GetComponent<StatusHandler>().SetProtector(packet.Caster.Targetable);
        }

        private bool Protection(AttackPacket packet)
        {
            if (packet.EventChain >= 1) return false;

            var target = GetComponent<StatusHandler>().Protector;

            print(this + " PROTECTED! from " + packet.Ability + " by " + target);

            
            GetComponent<StatusHandler>().BreakState();

            packet.ClearAllTargets();
            packet.AddNewTarget(target);

            packet.EventChain++;

            GetComponent<AttackExecutor>().ReceiveAttackPacket(packet);

            return true;
        }

        private bool Guardian(AttackPacket packet)
        {
            if (packet.EventChain >= 1) return false;

            GetComponent<StatusHandler>().BreakState();

            BattleGrid battleGrid = BattleGrid.singleton;
            Directions directions = new Directions();
            Cell testCell;

            bool didLeap = false;

            foreach (Vector2Int d in directions.AllDirections)
            {
                if (battleGrid.playerCells.ContainsKey(GetComponent<Targetable>().GridPosition + d))
                {
                    testCell = battleGrid.playerCells[GetComponent<Targetable>().GridPosition + d];
                }
                else
                {
                    continue;
                }

                if (!testCell.IsOccupied)
                {
                    didLeap = true;
                    GetComponent<StatusHandler>().Protector.GetComponent<GridMover>().Displace(testCell);
                    break;
                }
            }

            if (!didLeap) return false;

            if (GetComponent<StatusHandler>().Protector != null && packet.Ability.damageType == DamageType.physical)
            {
                var caster = GetComponent<StatusHandler>().Protector.GetComponent<AttackExecutor>();
                var target = packet.Caster.Targetable;

                packet.ClearAllTargets();
                packet.AddNewTarget(target);

                packet.Caster = caster;
                packet.EventChain++;

                GetComponent<AttackExecutor>().ReceiveAttackPacket(packet);
            }

            print(this + " GUARDIAN'D! from " + packet.Ability + " by " + GetComponent<StatusHandler>().Protector);
            return true;
        }

        private bool Vampirism(AttackPacket packet)
        {
            if (packet.EventChain >= 1) return false;
            PublicAbilities pa = PublicAbilities.singleton;
            Ability vamp = pa.vampire;

            AttackPacket vampirePacket = new AttackPacket(vamp, GetComponent<AttackExecutor>(), packet.GetTargets());

            vampirePacket.ClearAllTargets();
            vampirePacket.AddNewTarget(packet.Caster.Targetable);
            vampirePacket.Caster = packet.Caster;
            vampirePacket.EventChain = packet.EventChain;
            vampirePacket.EventChain++;
            vampirePacket.Damage = packet.Damage;

            var healing = vampirePacket.Damage * statModifierTool.VampirismAdjustment(vampirePacket.Caster.GetComponent<StatsHandler>());
            vampirePacket.Damage = healing;

            GetComponent<AttackExecutor>().ReceiveAttackPacket(vampirePacket);
            return true;
        }

        private bool Splash(AttackPacket packet)
        {
            if (packet.EventChain >= 1) return false;

            PublicAbilities pa = PublicAbilities.singleton;
            Ability splash = pa.splash;

            AttackPacket splashPacket = new AttackPacket(splash, GetComponent<AttackExecutor>(), packet.GetTargets())
            {
                Caster = GetComponent<AttackExecutor>(),
                EventChain = packet.EventChain
            };
            splashPacket.EventChain++;
            splashPacket.Damage = packet.Damage;

            var damage = splashPacket.Damage * statModifierTool.SplashAdjustment(packet.Caster.GetComponent<StatsHandler>());
            splashPacket.Damage = damage;

            GetComponent<AttackExecutor>().ReceiveAttackPacket(splashPacket);
            return true;
        }
    }
}
