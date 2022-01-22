using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace TTW.Combat
{
    public class ActorSelection : MonoBehaviour
    {
        [SerializeField] ActorEntity highlightedActor;
        [SerializeField] CombatUnloader unloader;

        ScreenSelectionTool screenSelector = new ScreenSelectionTool();
        public event EventHandler onActorAvailable;

        public void Initialize()
        {
            var allActors = FindObjectsOfType<ActorEntity>();

            foreach(ActorEntity a in allActors)
            {
                if (a.Actor == unloader.PartyInPosition(0).Actor)
                {
                    highlightedActor = a;
                    break;
                }
            }
        }

        public ActorEntity HighlightActor(DirectionTypes direction)
        {
            if (GetTargetableActors().Count() == 0) return null;

            List<Targetable> targetableActors = GetTargetableActors();

            var highlightedTarget = screenSelector.MoveThroughSelection(targetableActors, direction, highlightedActor.GetComponent<Targetable>());
            highlightedActor = highlightedTarget.GetActorEntity();

            return highlightedActor;
        }

        public List<Targetable> GetTargetableActors()
        {
            var allActors = FindObjectsOfType<ActorEntity>();

            var allTargetable = (from ActorEntity a in allActors
                    where a.GetComponent<Cooldown>().IsOnCooldown == false
                    where a.GetComponent<Cooldown>().IsChanneling == false
                    where a.DestroyFlag == false
                    select a.GetComponent<Targetable>()).ToList();

            return allTargetable;
        }

        public List<Targetable> GetTargetableActors(Targetable excludeTarget)
        {
            var allActors = FindObjectsOfType<ActorEntity>();

            var allTargetable = (from ActorEntity a in allActors
                                 where a.GetComponent<Cooldown>().IsOnCooldown == false
                                 where a.GetComponent<Cooldown>().IsChanneling == false
                                 where a.DestroyFlag == false
                                 select a.GetComponent<Targetable>()).ToList();

            if (allTargetable.Contains(excludeTarget))
            {
                allTargetable.Remove(excludeTarget);
            }

            return allTargetable;
        }

        public void OffCDAlert()
        {
            if (GetTargetableActors().Count() == 1)
            {
                onActorAvailable?.Invoke(this, EventArgs.Empty);
            }
        }

        public ActorEntity SelectNextActor(DirectionTypes dir)
        {
            if (GetTargetableActors().Count() == 0)
            {
                highlightedActor = null;
                return null;
            }

            if (GetTargetableActors().Count() == 1)
            {
                highlightedActor = GetTargetableActors()[0].GetActorEntity();
                return highlightedActor;
            }

            int nextIndex = unloader.PositionOfParty(highlightedActor);

            while (!GetTargetableActors(excludeTarget: highlightedActor.GetComponent<Targetable>()).Contains(unloader.PartyInPosition(nextIndex).GetComponent<Targetable>()))
            {
                if (dir == DirectionTypes.Right)
                {
                    if (nextIndex < unloader.PartyCount() - 1)
                    {
                        nextIndex++;
                    }
                    else
                    {
                        nextIndex = 0;
                    }
                }
                else
                {
                    if (nextIndex > 0)
                    {
                        nextIndex--;
                    }
                    else
                    {
                        nextIndex = unloader.PartyCount() - 1;
                    }
                }
            }

            highlightedActor = unloader.PartyInPosition(nextIndex);

            return highlightedActor;
        }
    }
}