using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class AttackPacket
    {
        public Ability Ability { get; set; }
        public float Damage { get; set; }
        public DamageType DamageType { get; set; }
        public NeutralState NeutralState { get; set; }
        public Stats Stat { get; set; }
        public int StatChange { get; set; }
        public StatusEffectType Status { get; set;}
        public AttackVariant AttackVariant { get; set; }
        public float StatusTimer { get; set; }
        public Ability Enchantment { get; set; }
        public float BewilderTimer { get; set; }
        public AttackExecutor Caster { get; set; }
        List<Targetable> _targets = new List<Targetable>();
        public bool Dispel { get; set; }
        public int EventChain { get; set; }
        public bool Counter { get; set; }
        public bool Crit { get; set; }
        public bool AttackCancelled { get; set; }


        public AttackPacket(Ability ability, AttackExecutor caster, List<Targetable> targets)
        {
            Ability = ability;
            Caster = caster;
            Damage = ability.damageFlat;
            DamageType = ability.damageType;
            NeutralState = ability.neutralState;
            AttackVariant = ability.attackVariant;
            Stat = ability.statToChange;
            StatChange = ability.statChangeValue;
            Status = ability.statusEffect;
            StatusTimer = ability.statusEffectTimer;
            BewilderTimer = 0f;
            Dispel = (ability.attackVariant == AttackVariant.dispel);
            EventChain = 0;
            Counter = false;
            Crit = false;
            AttackCancelled = false;

            foreach(Targetable t in targets)
            {
                _targets.Add(t);
            }
        }

        public AttackPacket(Ability ability, AttackExecutor caster, Targetable target)
        {
            Ability = ability;
            Caster = caster;
            Damage = ability.damageFlat;
            DamageType = ability.damageType;
            NeutralState = ability.neutralState;
            AttackVariant = ability.attackVariant;
            Stat = ability.statToChange;
            StatChange = ability.statChangeValue;
            Status = ability.statusEffect;
            StatusTimer = ability.statusEffectTimer;
            BewilderTimer = 0f;
            _targets.Add(target);
            Dispel = (ability.attackVariant == AttackVariant.dispel);
            EventChain = 0;
            Crit = false;
            AttackCancelled = false;
        }

        public AttackPacket(Ability ability, AttackExecutor caster)
        {
            Ability = ability;
            Caster = caster;
            Damage = ability.damageFlat;
            DamageType = ability.damageType;
            NeutralState = ability.neutralState;
            AttackVariant = ability.attackVariant;
            Stat = ability.statToChange;
            StatChange = ability.statChangeValue;
            Status = ability.statusEffect;
            StatusTimer = ability.statusEffectTimer;
            BewilderTimer = 0f;
            Dispel = (ability.attackVariant == AttackVariant.dispel);
            EventChain = 0;
            Crit = false;
            AttackCancelled = false;
        }

        public void WriteNewTargets(List<Targetable> targets)
        {
            _targets.Clear();
            foreach(Targetable t in targets)
            {
                _targets.Add(t);
            }
        }

        public Targetable FirstTargetable()
        {
            return _targets[0];
        }

        public void AddNewTarget(Targetable target)
        {
            _targets.Add(target);
        }

        public void ClearAllTargets()
        {
            _targets.Clear();
        }

        public int TargetsCount() => _targets.Count;

        public List<Targetable> GetTargets() => _targets;

        public void NeutralSettings()
        {
            NeutralState = NeutralState.none;
            Stat = Stats.None;
            StatChange = 0;
            Status = StatusEffectType.none;
            StatusTimer = 0f;
        }

        public void CancelAttack()
        {
            AttackCancelled = true;
            ClearAllTargets();
        }
    }
}
