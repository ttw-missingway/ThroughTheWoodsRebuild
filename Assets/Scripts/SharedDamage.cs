using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class SharedDamage : MonoBehaviour
    {
        [SerializeField] StatsHandler damageCore;

        private void Awake()
        {
            damageCore = GameObject.FindGameObjectWithTag("DamageCore").GetComponent<StatsHandler>();
        }

        public void SendDamageToCore(float damage)
        {
            damageCore.ChangeHealth(damage);
        }

        public void SendBuffsToCore(Stats stat, int amount)
        {
            damageCore.ModifyStat(stat, amount);
        }

        public StatsHandler DamageCore => damageCore;
    }
}
