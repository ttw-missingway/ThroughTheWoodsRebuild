using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class ScreenSelectionTool
    {
        readonly int coefficient = 2;

        public Targetable MoveThroughSelection(List<Targetable> targetables, DirectionTypes direction, Targetable pivot)
        {
            Targetable currentSelection = targetables[0];

            foreach(Targetable t in targetables)
            {
                t.SetDirectionScore(CalculateDirectionScore(pivot, t, direction));
                t.SetProximityScore(CalculateProximityScore(pivot, t));
                t.SetTotalScore(t.DirectionScore * coefficient + t.ProximityScore);
            }

            foreach (Targetable t in targetables)
            {
                if (t == pivot) continue;

                if (t.TotalScore < currentSelection.TotalScore)
                {
                    currentSelection = t;
                }
            }

            if (currentSelection.TotalScore >= 100)
            {
                currentSelection = pivot;
            }

            foreach(Targetable t in targetables)
            {
                t.ClearScore();
            }

            return currentSelection;
        }

        public Targetable PassToAvailable(List<Targetable> targetables, Targetable pivot)
        {
            Targetable currentSelection = targetables[0];

            pivot.SetTotalScore(100);

            foreach (Targetable t in targetables)
            {
                t.SetProximityScore(CalculateProximityScore(pivot, t));
                t.SetTotalScore(t.DirectionScore * coefficient + t.ProximityScore);
            }

            foreach (Targetable t in targetables)
            {
                if (t == pivot) continue;

                if (t.TotalScore < currentSelection.TotalScore)
                {
                    currentSelection = t;
                }
            }

            foreach (Targetable t in targetables)
            {
                t.ClearScore();
            }

            return currentSelection;
        }

        private int CalculateProximityScore(Targetable pivot, Targetable compare)
        {
            int proximityScore =
                Mathf.Abs(pivot.GridPosition.x - compare.GridPosition.x + (pivot.GridPosition.y - compare.GridPosition.y));

            return proximityScore;
        }

        private int CalculateDirectionScore(Targetable pivotTarget, Targetable compareTarget, DirectionTypes direction)
        {
            int directionScore = 100;

            switch (direction)
            {
                case DirectionTypes.Down:
                    if (compareTarget.GridPosition.y < pivotTarget.GridPosition.y)
                    {
                        directionScore = Mathf.Abs(pivotTarget.GridPosition.x - compareTarget.GridPosition.x);
                    }
                    break;
                case DirectionTypes.Up:
                    if (compareTarget.GridPosition.y > pivotTarget.GridPosition.y)
                    {
                        directionScore = Mathf.Abs(pivotTarget.GridPosition.x - compareTarget.GridPosition.x);
                    }
                    break;
                case DirectionTypes.Left:
                    if (compareTarget.GridPosition.x < pivotTarget.GridPosition.x)
                    {
                        directionScore = Mathf.Abs(pivotTarget.GridPosition.y - compareTarget.GridPosition.y);
                    }
                    break;
                case DirectionTypes.Right:
                    if (compareTarget.GridPosition.x > pivotTarget.GridPosition.x)
                    {
                        directionScore = Mathf.Abs(pivotTarget.GridPosition.y - compareTarget.GridPosition.y);
                    }
                    break;
                case DirectionTypes.None:
                    directionScore = 0;
                    break;
            }

            return directionScore;
        }
    }
}