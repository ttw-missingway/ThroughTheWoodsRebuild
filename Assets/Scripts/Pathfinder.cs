using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class Pathfinder : MonoBehaviour
    {
        Cell startingCell, endingCell;
        bool isRunning = true;
        Cell searchCenter;
        private bool LoadCellsOnce = false;

        Vector2Int[] directions =
        {
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.down,
        Vector2Int.right
        };

        BattleGrid battleGrid;
        public Queue<Cell> pathQueue = new Queue<Cell>();
        public List<Cell> finalPath = new List<Cell>();

        private void Start()
        {
            battleGrid = BattleGrid.singleton;
        }

        public List<Cell> GetFinalPath(Cell start, Cell end)
        {
            isRunning = true;
            finalPath = new List<Cell>();
            pathQueue = new Queue<Cell>();
            startingCell = start;
            endingCell = end;
            Pathfind();
            return finalPath;
        }

        private void Pathfind()
        {
            pathQueue.Enqueue(startingCell);

            while (pathQueue.Count > 0 && isRunning)
            {
                searchCenter = pathQueue.Dequeue();
                searchCenter.SetExplored(true);
                CheckIfPathIsComplete();
                ExploreNeighbors();
            }
        }

        private void CheckIfPathIsComplete()
        {
            if (searchCenter == endingCell && isRunning)
            {
                Cell currentCell = endingCell;
                isRunning = false;
                finalPath.Add(endingCell);
                while (currentCell != startingCell)
                {
                    finalPath.Add(currentCell.ExploredFrom);
                    currentCell = currentCell.ExploredFrom;
                }
                finalPath.Reverse();
            }
        }

        private void ExploreNeighbors()
        {
            if (!isRunning)  return; 

            foreach (Vector2Int direction in directions)
            {
                if (battleGrid.allCells.ContainsKey(searchCenter.GetGridPos() + direction))
                {
                    QueueNewNeighbors(direction);
                }
            }
        }

        private void QueueNewNeighbors(Vector2Int direction)
        {
            Cell neighbor = battleGrid.allCells[(searchCenter.GetGridPos() + direction)];

            if (neighbor.IsExplored && !pathQueue.Contains(neighbor))
                return; 

            if (neighbor.IsOccupied)
                return;

            if (neighbor.IsEnemyCell)
                return;

            if (neighbor.IsBenchCell)
                return;

            pathQueue.Enqueue(neighbor);
            neighbor.SetExploredFrom(searchCenter);
        }

        private bool CheckCellAvailability(Vector2Int direction)
        {
            Cell neighbor = battleGrid.allCells[(searchCenter.GetGridPos() + direction)];

            if (neighbor.IsExplored || !pathQueue.Contains(neighbor) || neighbor.IsOccupied || neighbor.IsEnemyCell)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void NoAvailablePath()
        {
            isRunning = false;
            finalPath.Clear();
            finalPath.Add(startingCell);
        }
    }
}
