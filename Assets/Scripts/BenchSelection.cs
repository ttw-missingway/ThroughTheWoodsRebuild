using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class BenchSelection : MonoBehaviour
    {
        [SerializeField] Bench bench;
        [SerializeField] CombatUnloader unloader;

        [SerializeField] List<ActorEntity> displayedActors = new List<ActorEntity>();
        [SerializeField] List<BenchText> actorTexts = new List<BenchText>();
        BattleGrid battleGrid;
        int highlightIndex = 0;

        private void Start()
        {
            battleGrid = BattleGrid.singleton;
        }

        public void ResetIndex()
        {
            highlightIndex = 0;
        }

        public bool IsBenchOccupied()
        {
            return (unloader.ActorsOnBench().Count > 0);
        }

        public void DisplayActors()
        {
            displayedActors.Clear();

            for (int i = 0; i < unloader.ActorsOnBench().Count; i++)
            {
                displayedActors.Add(unloader.ActorsOnBench()[i]);
                actorTexts[i].AssignAbility(displayedActors[i]);
            }

            actorTexts[highlightIndex].Highlight();
        }

        public void ClearDisplay()
        {
            foreach (var text in actorTexts)
            {
                text.ClearDisplay();
            }
        }

        public ActorEntity GetHighlightedBenchActor() => unloader.BenchInPosition(highlightIndex);

        public ActorEntity SwapParty(ActorEntity partyMember, ActorEntity benchMember)
        {
            Cell spawnCell = battleGrid.playerCells[partyMember.GridPosition];

            partyMember = unloader.ConvertToOriginalPrefab(partyMember);
            unloader.SwapBenchAndParty(partyMember, benchMember, spawnCell, initial: false);

            benchMember = unloader.ConvertToManifest(benchMember);

            return benchMember;
        }

        public ActorEntity ReplaceDeadParty(ActorEntity deadParty, ActorEntity benchMember)
        {
            Cell spawnCell = battleGrid.playerCells[deadParty.GridPosition];

            unloader.RemoveCooldown(deadParty);
            deadParty.DestroyFlag = true;
            deadParty.Unsubscribe();
            Destroy(deadParty.gameObject);

            deadParty = unloader.ConvertToOriginalPrefab(deadParty);

            unloader.RemovePartyMember(deadParty);
            unloader.AddPartyMember(benchMember);
            unloader.RemoveBenchMember(benchMember);
            unloader.ManifestActor(benchMember, spawnCell, initial: false);

            benchMember = unloader.ConvertToManifest(benchMember);

            return benchMember;
        }

        public ActorEntity MoveThroughBench(DirectionTypes direction)
        {
            foreach (BenchText text in actorTexts)
            {
                text.ClearHighlight();
            }

            switch (direction)
            {
                case DirectionTypes.Left:
                    if (highlightIndex > 0)
                        highlightIndex--;
                    break;
                case DirectionTypes.Right:
                    if (highlightIndex < displayedActors.Count - 1)
                        highlightIndex++;
                    break;
                default:
                    print("ERROR, unexpected highlight direction");
                    break;
            }

            actorTexts[highlightIndex].Highlight();

            return GetHighlightedBenchActor();
        }

        public void Highlight()
        {
            bench.SetHighlight(true);
        }

        public void ResetHighlight()
        {
            bench.SetHighlight(false);
        }
    }
}
