using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class EventController : MonoBehaviour
    {
        public static EventController singleton;
        [SerializeField] CombatEventType type;

        public event EventHandler OnCombatEvent;

        private void Awake()
        {
            SetSingleton();
        }

        public CombatEventType GetEventSequence() => type;

        public void BroadcastNewEventType(CombatEventType newType)
        {
            type = newType;
            BroadcastToEventAgents();
        }

        private void SetSingleton()
        {
            if (singleton == null)
            {
                singleton = this;
            }
            else
            {
                print("ERROR: more than one AnimationFreeze in scene!");
            }
        }

        public void CombatEvent(AbilityAnimationController eventAnimator, Targetable target)
        {
            //eventSequence++;
            //read through array from scriptable object for event sequences;

            var newAnimator = Instantiate(eventAnimator);
            newAnimator.SetAbilityCam();
            newAnimator.AddTarget(target);
        }

        public void BroadcastToEventAgents()
        {
            OnCombatEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
