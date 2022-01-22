using System;
using System.Collections;
using System.Collections.Generic;
using TTW.Persistent;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class StatusHandler : MonoBehaviour
    {
        [SerializeField] NeutralState _state = NeutralState.none;
        [SerializeField] List<StatusEffect> statusEffects = new List<StatusEffect>();
        [SerializeField] bool blind;
        [SerializeField] bool burn;
        [SerializeField] bool shock;
        [SerializeField] bool madness;
        [SerializeField] bool regen;
        [SerializeField] bool rebirth;
        [SerializeField] bool bubble;
        [SerializeField] bool mirror;
        [SerializeField] StatusEffect statusEffectShell;
        [SerializeField] Targetable protector;

        [SerializeField] Ability counterAbility;
        [SerializeField] Ability enchantment;
        [SerializeField] Ability enchantmentSlot;

        public bool Blind => blind;
        public bool Burn => burn;
        public bool Shock => shock;
        public bool Madness => madness;
        public bool Regen => regen;
        public bool Rebirth => rebirth;
        public bool Bubble => bubble;
        public bool Mirror => mirror;
        public Targetable Protector => protector;

        public Ability GetCounterAbility() => counterAbility;
        public Ability GetEnchantedAbility() => enchantment;
        public Ability GetEnchantmentSlot() => enchantmentSlot;

        public void PushStatus()
        {
            if (GetComponent<ActorEntity>() == null) return;

            PartyManager partyManager = FindObjectOfType<PartyManager>();

            List<StatusEffect> activeStatusEffects = new List<StatusEffect>(GetComponentsInChildren<StatusEffect>());
            List<StatusSnapShot> statusSnapShots = new List<StatusSnapShot>();

            foreach(StatusEffect s in activeStatusEffects)
            {
                var snap = new StatusSnapShot(s.Belligerent, s.GetEffectType(), s.RemainingTime);
                statusSnapShots.Add(snap);
            }

            partyManager.SaveStatusSnapshot(GetComponent<ActorEntity>().Actor.actorType, statusSnapShots);

        }

        public void PullStatus()
        {
            if (GetComponent<ActorEntity>() == null) return;

            PartyManager partyManager = FindObjectOfType<PartyManager>();

            if (partyManager.LoadStatusSnapshot(GetComponent<ActorEntity>().Actor.actorType) == null) return;

            foreach(StatusSnapShot s in partyManager.LoadStatusSnapshot(GetComponent<ActorEntity>().Actor.actorType))
            {
                CreateNewStatus(s, reactivate: true);
            }
        }

        public void ClearEnchantmentSlot()
        {
            enchantmentSlot = null;
        }

        public void SetEnchantmentSlot(Ability ability)
        {
            if (ability.damageType == DamageType.magical)
                enchantmentSlot = ability;
            else
                print("Not able to enchant with non-magical attacks");
        }

        public void ChangeState(NeutralState newState)
        {
            _state = newState;
        }

        public void BreakState()
        {
            _state = NeutralState.none;
        }

        public void SetProtector(Targetable newProtector)
        {
            protector = newProtector;
        }

        public void CreateNewStatus(StatusSnapShot snapShot, bool reactivate)
        {
            if (CheckRedundantStatus(snapShot.Type)) return;

            if (bubble) return;

            var newStatus = Instantiate(statusEffectShell, transform);
            
            newStatus.ActivateStatusEffect(snapShot.Type, snapShot.Timer, this, snapShot.Attacker, reactivate);
            UpdateStatus();
        }

        private bool CheckRedundantStatus(StatusEffectType type)
        {
            switch (type)
            {
                case StatusEffectType.blind:
                    if (blind) return true;
                    break;
                case StatusEffectType.burn:
                    if (burn) return true;
                    break;
                case StatusEffectType.madness:
                    if (madness) return true;
                    break;
                case StatusEffectType.shock:
                    if (shock) return true;
                    break;
                case StatusEffectType.regen:
                    if (regen) return true;
                    break;
                case StatusEffectType.rebirth:
                    if (rebirth) return true;
                    break;
                case StatusEffectType.bubble:
                    if (bubble) return true;
                    break;
                case StatusEffectType.mirror:
                    if (mirror) return true;
                    break;
            }

            return false;
        }

        public void UpdateStatus()
        {
            statusEffects.Clear();
            var allStatusEffects = FindObjectsOfType<StatusEffect>();

            var myStatusEffects = from StatusEffect s in allStatusEffects
                                  where GetComponentsInChildren<StatusEffect>().Contains(s)
                                  select s;

            blind = false;
            burn = false;
            shock = false;
            madness = false;
            regen = false;
            rebirth = false;
            bubble = false;
            mirror = false;

            foreach(StatusEffect s in myStatusEffects)
            {
                statusEffects.Add(s);

                if (s.GetEffectType() == StatusEffectType.blind)
                    blind = true;
                if (s.GetEffectType() == StatusEffectType.burn)
                    burn = true;
                if (s.GetEffectType() == StatusEffectType.shock)
                    shock = true;
                if (s.GetEffectType() == StatusEffectType.madness)
                    madness = true;
                if (s.GetEffectType() == StatusEffectType.regen)
                    regen = true;
                if (s.GetEffectType() == StatusEffectType.rebirth)
                    rebirth = true;
                if (s.GetEffectType() == StatusEffectType.bubble)
                    bubble = true;
                if (s.GetEffectType() == StatusEffectType.mirror)
                    mirror = true;
            }
        }

        public NeutralState State => _state;
    }
}
