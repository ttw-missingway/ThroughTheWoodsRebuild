using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class StatusSnapShot 
    {
        AttackExecutor _attacker;
        StatusEffectType _type;
        float _timer;

        public StatusSnapShot(AttackExecutor attacker, StatusEffectType type, float timer)
        {
            _timer = timer;
            _type = type;
            _attacker = attacker;
        }

        public AttackExecutor Attacker => _attacker;
        public StatusEffectType Type => _type;
        public float Timer => _timer;
    }
}
