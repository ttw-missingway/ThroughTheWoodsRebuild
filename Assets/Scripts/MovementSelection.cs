using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class MovementSelection : MonoBehaviour
    {
        BattleGrid battleGrid;
        AnimationController animationController;
        [SerializeField] Cell highlightedCell;
        [SerializeField] Vector2Int pivotPosition;
        [SerializeField] Vector2Int highlightedCellPosition;
        [SerializeField] PathDrawer pathDrawer;
        [SerializeField] ActorEntity savedActor;
        [SerializeField] bool invalidPath;

        public Vector2Int getHighlightedCellPosition => highlightedCell.GetGridPos();

        private void Start()
        {
            animationController = AnimationController.singleton;
            battleGrid = BattleGrid.singleton;

            animationController.OnAnimationFreezeEnd += AnimationController_OnAnimationFreezeEnd;
        }

        private void AnimationController_OnAnimationFreezeEnd(object sender, System.EventArgs e)
        {
            UpdateSelection(savedActor);
        }

        public void Move(ActorEntity actor)
        {
            if (CheckShock(actor.GetComponent<StatusHandler>())) return;

            actor.GetComponent<StatusHandler>().BreakState();

            savedActor = actor;
            pathDrawer.ErasePath();
            actor.GetComponent<GridMover>().Pathfind(highlightedCell);
        }

        private bool CheckShock(StatusHandler status)
        {
            if (status.Shock)
            {
                print(status.name + " IS SHOCKED AND UNABLE TO MOVE!");
                return true;
            } 

            return false;
        }

        public bool ActorUnderPointer(ActorEntity actor)
        {
            if (actor.GridPosition == highlightedCell.GetGridPos())
            {
                return true;
            }

            return false;
        }

        public Cell HighlightCellByDirection(DirectionTypes direction, bool performHighlight)
        {
            Vector2Int destinationPosition = GetDestinationPosition(direction);

            if (destinationPosition.y != -1) //only returns player cells unless player is trying to access bench
            {
                if (!battleGrid.playerCells.ContainsKey(destinationPosition)) return highlightedCell;
            }

            Cell destinationCell = battleGrid.allCells[destinationPosition];

            if (performHighlight)
                HighlightCell(destinationCell);

            return destinationCell;
        }

        public Cell HighlightCell(Cell destinationCell)
        {
            if (battleGrid.playerCells.ContainsKey(destinationCell.GetGridPos()))
            {
                SetHighlightedPosition(destinationCell.GetGridPos());
                DrawPath(pivotPosition, destinationCell.GetGridPos());
            }

            return destinationCell;
        }

        public void ClearAllPathData()
        {
            savedActor = null;
            highlightedCell = null;
            pathDrawer.ErasePath();
        }

        private void DrawPath(Vector2Int startPosition, Vector2Int endPosition)
        {
            Cell startCell = battleGrid.playerCells[startPosition];
            Cell endCell = battleGrid.playerCells[endPosition];

            pathDrawer.ClearPath();
            pathDrawer.DrawPath(startCell, endCell);
            invalidPath = pathDrawer.CheckPath(startCell, endCell);
        }

        private Vector2Int GetDestinationPosition(DirectionTypes direction)
        {
            switch (direction)
            {
                case DirectionTypes.Up:
                    return highlightedCellPosition + Vector2Int.up;
                case DirectionTypes.Down:
                    return highlightedCellPosition + Vector2Int.down;
                case DirectionTypes.Left:
                    return highlightedCellPosition + Vector2Int.left;
                case DirectionTypes.Right:
                    return highlightedCellPosition + Vector2Int.right;
                default:
                    return highlightedCellPosition;
            }
        }

        private void SetHighlightedPosition(Vector2Int newPosition)
        {
            highlightedCellPosition = newPosition;
            highlightedCell = battleGrid.playerCells[highlightedCellPosition];
        }

        public void UpdateSelection(ActorEntity actorEntity)
        {
            if (actorEntity == null) return;

            SetHighlightedPosition(actorEntity.GridPosition);
            pivotPosition = highlightedCellPosition;
        }

        public bool InvalidPath() => invalidPath;

        public Cell GetHighlightedCell() => highlightedCell;
    }
}
