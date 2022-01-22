using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class CombatUnloader : MonoBehaviour
    {
        [SerializeField] List<ActorEntity> actorsInParty = new List<ActorEntity>();
        [SerializeField] List<ActorEntity> actorsOnBench = new List<ActorEntity>();
        [SerializeField] List<CooldownTimer> cds = new List<CooldownTimer>();
        

        public ActorEntity PartyInPosition(int position) => ConvertToManifest(actorsInParty[position]);

        public int PositionOfParty(ActorEntity actor)
        {
            return actorsInParty.IndexOf(ConvertToOriginalPrefab(actor));
        }

        public int PartyCount() => actorsInParty.Count;

        //Index out of range error occuring here
        public ActorEntity BenchInPosition(int position) => actorsOnBench[position];
        public List<ActorEntity> ActorsOnBench() => actorsOnBench;


        public ActorEntity ConvertToOriginalPrefab(ActorEntity clone)
        {
            foreach (ActorEntity a in actorsInParty)
            {
                if (a.Actor == clone.Actor)
                {
                    return a;
                }
            }

            return null;
        }

        public ActorEntity ConvertToManifest(ActorEntity prefab)
        {
            ActorEntity[] allActors = FindObjectsOfType<ActorEntity>();

            foreach (ActorEntity a in allActors)
            {
                if (a.Actor == prefab.Actor)
                {
                    return a;
                }
            }

            return null;
        }

        public bool CheckBenchVacant()
        {
            if (actorsOnBench.Count == 0) return true;
            else return false;
        }

        public void AddPartyMember(ActorEntity actor)
        {
            actorsInParty.Add(actor);
        }

        public void AddBenchMember(ActorEntity actor)
        {
            actorsOnBench.Add(actor);
        }

        public void RemovePartyMember(ActorEntity actor)
        {
            if (!actorsInParty.Contains(actor)) return;

            actorsInParty.Remove(actor);
        }

        public void RemoveBenchMember(ActorEntity actor)
        {
            if (!actorsOnBench.Contains(actor)) return;

            actorsOnBench.Remove(actor);
        }

        public void SwapBenchAndParty(ActorEntity partyActor, ActorEntity benchActor, Cell spawnPoint, bool initial)
        {
            actorsInParty.Remove(partyActor);
            actorsOnBench.Remove(benchActor);
            actorsInParty.Add(benchActor);
            actorsOnBench.Add(partyActor);
            RemoveManifestActor(partyActor);
            ManifestActor(benchActor, spawnPoint, initial);
        }

        public void ManifestActor(ActorEntity actor, Cell spawnCell, bool initial)
        {
            var manifestActor = Instantiate(actor, spawnCell.transform.position, Quaternion.identity);
            manifestActor.GetComponent<StatsHandler>().SetActor(actor.Actor.actorType);
            manifestActor.GetComponent<StatsHandler>().PullStats(initial);
            manifestActor.GetComponent<StatusHandler>().PullStatus();
            AssignCooldown(manifestActor);
        }

        private void AssignCooldown(ActorEntity manifestActor)
        {
            foreach (CooldownTimer c in cds)
            {
                if (c.TargetAssigned() == null)
                {
                    c.SetTarget(manifestActor.GetComponent<Cooldown>());
                    manifestActor.GetComponent<Cooldown>().AssignUITimer(c);
                    break;
                }
            }
        }

        public void RemoveCooldown(ActorEntity manifestActor)
        {
            foreach (CooldownTimer c in cds)
            {
                if (c.TargetAssigned() == manifestActor.GetComponent<Cooldown>())
                {
                    c.RemoveTarget();
                    manifestActor.GetComponent<Cooldown>().RemoveUITimer();
                }
            }
        }

        public void RemoveManifestActor(ActorEntity actor)
        {
            ActorEntity manifestActor = null;
            ActorEntity[] allClones = FindObjectsOfType<ActorEntity>();

            foreach(ActorEntity a in allClones)
            {
                if (actor.Actor == a.Actor)
                {
                    manifestActor = a;
                    break;
                }
            }

            if (manifestActor != null)
            {
                manifestActor.GetComponent<StatsHandler>().PushStats();
                manifestActor.GetComponent<StatusHandler>().PushStatus();
                manifestActor.DestroyFlag = true;
                manifestActor.Unsubscribe();
                RemoveCooldown(manifestActor);
                Destroy(manifestActor.gameObject);
            }
        }
    }
}
