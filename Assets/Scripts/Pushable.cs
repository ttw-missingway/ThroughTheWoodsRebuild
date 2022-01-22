using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class Pushable : MonoBehaviour
    {
        GridMover gridMover;
        GridPosition gridPosition;
        BattleGrid battleGrid;

        private void Start()
        {
            gridMover = GetComponent<GridMover>();
            gridPosition = GetComponent<GridPosition>();
            battleGrid = BattleGrid.singleton;
        }

        public void Push(DirectionTypes direction, int force, bool anim)
        {
            if (!anim)
                gridMover.SetBeingPushed();

            Cell targetDestination = battleGrid.playerCells[gridPosition.GetGridPos()];
            Vector2Int currentPosition = gridPosition.GetGridPos();
            Vector2Int directionChanger;
            var allCells = FindObjectsOfType<Cell>();
            var cellPath = from Cell c in allCells
                           select c;

            switch (direction)
            {
                case DirectionTypes.Left:
                    if (currentPosition.x - force < 1)
                        force = currentPosition.x - 1;
                    cellPath = from Cell c in allCells
                                   where c.GetGridPos().y == currentPosition.y
                                   where c.GetGridPos().x < currentPosition.x
                                   where c.GetGridPos().x >= currentPosition.x - force
                                   where battleGrid.playerCells.ContainsValue(c)
                                   orderby c.GetGridPos().x descending
                                   select c;
                    cellPath.ToList();
                    break;

                case DirectionTypes.Right:
                    if (currentPosition.x + force > 4)
                        force = 4 - currentPosition.x;
                    cellPath = from Cell c in allCells
                                   where c.GetGridPos().y == currentPosition.y
                                   where c.GetGridPos().x > currentPosition.x
                                   where c.GetGridPos().x <= currentPosition.x + force
                                   where battleGrid.playerCells.ContainsValue(c)
                                   orderby c.GetGridPos().x
                                   select c;
                    break;

                case DirectionTypes.Up:
                    if (currentPosition.y + force > 2)
                        force = 2 - currentPosition.y;
                    cellPath = from Cell c in allCells
                               where c.GetGridPos().x == currentPosition.x
                               where c.GetGridPos().y > currentPosition.y
                               where c.GetGridPos().y <= currentPosition.y + force
                               where battleGrid.playerCells.ContainsValue(c)
                               orderby c.GetGridPos().y
                               select c;
                    break;
                case DirectionTypes.Down:
                    if (currentPosition.y - force > 0)
                        force = currentPosition.y;
                    cellPath = from Cell c in allCells
                               where c.GetGridPos().x == currentPosition.x
                               where c.GetGridPos().y < currentPosition.y
                               where c.GetGridPos().y >= currentPosition.y - force
                               where battleGrid.playerCells.ContainsValue(c)
                               orderby c.GetGridPos().y descending
                               select c;
                    break;
                default:
                    directionChanger = new Vector2Int(0, 0);
                    break;
            }

            //print("path order: ");

            //foreach(Cell c in cellPath)
            //{
            //    print(c);
            //}

            foreach(Cell c in cellPath)
            {
                if (!c.IsOccupied)
                {
                    //print("cell: " + c + "is not occupied");
                    if (!c.IsFlaggedForArrival)
                    {
                        //print("cell: " + c + "is not flagged");
                        targetDestination = c;
                        continue;
                    }
                }

                //print("cell: " + c + "is not accessible");

                break;
            }

            //print("name: " + this.name + "destination: " + targetDestination);

            gridMover.Pathfind(targetDestination);
        }
    }
}
