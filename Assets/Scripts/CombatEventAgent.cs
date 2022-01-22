using System;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public enum CombatEventType
    {
        DestroyStructure,
        TiltLeft,
        TiltRight,
        //Add more as needed
    }

    public class CombatEventAgent : MonoBehaviour
    {
        [SerializeField] bool sequenceEvent = false;
        [SerializeField] List<CombatEventType> eventTypes = new List<CombatEventType>();
        EventController eventController;

        public delegate void PerformEvent();
        private PerformEvent performEvent;

        private void Start()
        {
            eventController = EventController.singleton;
            eventController.OnCombatEvent += AnimationController_OnCombatEvent;
        }

        private void AssignEvent(CombatEventType eventType)
        {
            switch (eventType)
            {
                case CombatEventType.DestroyStructure:
                    performEvent += () => Destroy(gameObject); 
                    break;
                case CombatEventType.TiltLeft:
                    performEvent += TiltLeft;
                    break;
                case CombatEventType.TiltRight:
                    performEvent += TiltRight;
                    break;
            }
        }

        private void TiltLeft()
        {
            if (GetComponent<EventTransformer>() == null) return;

            EventTransformer et = GetComponent<EventTransformer>();
            et.StartTilt(DirectionTypes.Left);
        }

        private void TiltRight()
        {
            if (GetComponent<EventTransformer>() == null) return;

            EventTransformer et = GetComponent<EventTransformer>();
            et.StartTilt(DirectionTypes.Right);
        }

        private void AnimationController_OnCombatEvent(object sender, System.EventArgs e)
        {
            CombatEventType eventType = eventController.GetEventSequence();

            if (eventTypes.Contains(eventType))
            {
                AssignEvent(eventType);
                performEvent();
            }
        }
    }
}