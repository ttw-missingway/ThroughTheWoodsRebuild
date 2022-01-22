using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class TargetSelection : MonoBehaviour
    {
        [SerializeField] ActorEntity selectedActor;
        [SerializeField] List<Targetable> targets = new List<Targetable>();
        [SerializeField] Targetable highlightedTarget;
        [SerializeField] private Camera cam;
        Cell[] allCells;
        List<Cell> highlightedCells;

        ScreenSelectionTool screenSelector = new ScreenSelectionTool();
        TargetingTool targetingTool;

        public bool CheckTargetsAvailable() => (targets.Count() > 0);

        private void Awake()
        {
            allCells = FindObjectsOfType<Cell>();
        }

        public List<Targetable> GetSelectedTargets(TargetingType type)
        {
            if (targets.Count == 0) return targets;

            switch (type)
            {
                case TargetingType.allallies:
                case TargetingType.allfoes:
                case TargetingType.ordinal:
                    return targets;
                default:
                    return new List<Targetable> { highlightedTarget };
            }
        }

        public void HighlightTarget(DirectionTypes direction, TargetingType tType)
        {
            if (!CheckTargetsAvailable()) return;

            ClearCellHighlights();
            HighlightRangeCells();

            highlightedTarget = screenSelector.MoveThroughSelection(targets, direction, highlightedTarget);
            HighlightSelected(tType);

        }

        private void HighlightSelected(TargetingType tType)
        {
            var battlegrid = BattleGrid.singleton;

            if (tType == TargetingType.allallies || tType == TargetingType.allfoes || tType == TargetingType.ordinal)
            {
                HighlightMultipleSelections();
            }
            else
            {
                if (highlightedTarget.GetTargetClass() != TargetClass.Boss)
                {
                    battlegrid.allCells[highlightedTarget.GridPosition].HighlightSelected();
                }
            }

            HighlightSelectedBoss();

            if (tType == TargetingType.random)
            {
                HighlightRangeCells();
            }
        }

        private void HighlightMultipleSelections()
        {
            var targetedCells = from c in highlightedCells
                                from t in targets
                                where t.GridPosition == c.GetGridPos()
                                select c;

            foreach (Cell c in targetedCells)
            {
                c.HighlightSelected();
            }
        }

        private void HighlightSelectedBoss()
        {
            var bossTargets = from t in targets
                              where t.GetTargetClass() == TargetClass.Boss && t == highlightedTarget
                              select t;

            var bowBoss = from t in bossTargets
                          where t.GetComponent<GridPosition>().GetWing() == Wing.Bow
                          select t;

            var starboardBoss = from t in bossTargets
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Starboard
                                select t;

            var portBoss = from t in bossTargets
                           where t.GetComponent<GridPosition>().GetWing() == Wing.Port
                           select t;

            var bowCells = from c in allCells
                           where c.GetGridPos().y == 3
                           select c;

            var starboardCells = from c in allCells
                                 where c.GetGridPos().x == 5
                                 select c;

            var portCells = from c in allCells
                            where c.GetGridPos().x == 0
                            select c;

            if (bowBoss.Count() > 0)
            {
                foreach (Cell c in bowCells)
                {
                    c.HighlightSelected();
                }
            }
            if (starboardBoss.Count() > 0)
            {
                foreach (Cell c in starboardCells)
                {
                    c.HighlightSelected();
                }
            }
            if (portBoss.Count() > 0)
            {
                foreach (Cell c in portCells)
                {
                    c.HighlightSelected();
                }
            }
        }

        public void SetActor(ActorEntity actor)
        {
            selectedActor = actor;
        }

        public TargetingTool FilterTargetables(Ability ability)
        {
            targets.Clear();

            var allTargetables = FindObjectsOfType<Targetable>();

            Targetable caster = selectedActor.GetComponent<Targetable>();

            targetingTool = new TargetingTool(allTargetables.ToList(), ability, caster, TargetClass.Actor);

            targets = targetingTool.SortTargetables();

            if (targets.Count() > 0)
                highlightedTarget = targets[0];

            return targetingTool;
        }

        private void HighlightRangeCells()
        {
            print("highlighting cell: 1");

            highlightedCells = targetingTool.TargetableCells();

            foreach (Cell c in highlightedCells)
            {
                c.Highlight();
            }
        }

        public void ClearCellHighlights()
        {
            foreach(Cell c in allCells)
            {
                c.ClearHighlight();
            }
        }
    }
}