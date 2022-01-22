using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class StatusEffect : MonoBehaviour
    {
        [SerializeField] StatusEffectType _type = StatusEffectType.none;
        [SerializeField] float _timer = 0f;
        [SerializeField] StatusHandler _parent = null;
        [SerializeField] AttackExecutor _belligerent = null;
        [SerializeField] float _bubbleHealth = 100f;

        public float RemainingTime => _timer;
        public StatusEffectType GetEffectType() => _type;
        public AttackExecutor Belligerent => _belligerent;

        StatModifierTool statModifierTool;

        private void Awake()
        {
            statModifierTool = new StatModifierTool(FindObjectOfType<PublicValues>());
        }

        public void ActivateStatusEffect(StatusEffectType type, float timer, StatusHandler parent, AttackExecutor belligerent, bool reactivate)
        {
            _type = type;

            if (reactivate)
            {
                _timer = timer;
            }
            else
            {
                _timer = timer * statModifierTool.StatusAdjustment(belligerent.GetComponent<StatsHandler>());
            }
            
            _parent = parent;
            _parent.GetComponent<StatsHandler>().OnRebirth += StatsHandler_OnRebirth;
            _belligerent = belligerent;
            StartCoroutine(EffectTimer());
        }

        private void StatsHandler_OnRebirth(object sender, EventArgs e)
        {
            Destroy(gameObject);
        }

        private IEnumerator EffectTimer()
        {
            while(_timer > 0f)
            {
                _timer--;
                if (_type == StatusEffectType.burn) Burn();
                if (_type == StatusEffectType.regen) Regen();

                yield return new WaitForSeconds(1f);
            }
            Destroy(gameObject);
        }

        public void Burn()
        {
            if (_belligerent == null) return;
            if (_parent == null) return;

            var baseDamage = 2f;
            var burnDamage = baseDamage * statModifierTool.BurnAdjustment(_belligerent.GetComponent<StatsHandler>());
            print("burning for: " + burnDamage);
            _parent.GetComponent<StatsHandler>().ChangeHealth(-burnDamage);
        }

        private void Regen()
        {
            var baseHealing = 2f;
            var regenHeal = baseHealing * statModifierTool.BaseHealingAdjustment(_belligerent.GetComponent<StatsHandler>());
            print("healing for: " + regenHeal);
            _parent.GetComponent<StatsHandler>().ChangeHealth(regenHeal);
        }

        public void BubbleDamage(float damage)
        {
            _bubbleHealth -= damage;

            if(_bubbleHealth <= 0f)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _parent.UpdateStatus();
        }
    }
}