using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class CasterProxy : MonoBehaviour
    {
        Cooldown _cooldown;

        private void SetCooldown()
        {
            _cooldown = GetComponent<Cooldown>();
            _cooldown.OnChannelEnd += _cooldown_OnChannelEnd;
        }

        private void _cooldown_OnChannelEnd(object sender, System.EventArgs e)
        {
            Destroy(gameObject);
        }

        public void SetPacket(AttackPacket packet)
        {
            SetCooldown();
            _cooldown.StartAbilityChannel(packet, packet.Ability.chainDelayTime);
        }
    }
}