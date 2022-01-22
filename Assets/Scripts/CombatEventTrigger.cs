using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class CombatEventTrigger : MonoBehaviour
    {
        [SerializeField] float[] percentageTriggers = new float[0];
        [SerializeField] int eventSequence = 0;
        [SerializeField] AbilityAnimationController aac;
        [SerializeField] Targetable cameraTarget;
        StatsHandler stats;
        EventController eventController;

        private void Start()
        {
            if (GetComponent<StatsHandler>() != null)
            {
                stats = GetComponent<StatsHandler>();
                stats.OnDamageTaken += Stats_OnDamageTaken;
            }
            else
            {
                Debug.LogError("EVENT AGENT MUST HAVE ACCESS TO A STATS HANDLER");
            }

            eventController = EventController.singleton;
        }

        private void Stats_OnDamageTaken(object sender, System.EventArgs e)
        {
            CheckTriggers();
        }

        public void CheckTriggers()
        {
            print("checking event trigger");

            float currentRatio = stats.Health / stats.MaxHealth;

            if (currentRatio <= percentageTriggers[eventSequence])
            {
                print("event triggered, starting event sequence");
                eventSequence++;
                eventController.CombatEvent(aac, cameraTarget);
            }
        }
    }
}
