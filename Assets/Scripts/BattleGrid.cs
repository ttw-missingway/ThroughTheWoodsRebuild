using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class BattleGrid : MonoBehaviour
    {
        public static BattleGrid singleton;

        public Dictionary<Vector2Int, Cell> allCells = new Dictionary<Vector2Int, Cell>();
        public Dictionary<Vector2Int, Cell> playerCells = new Dictionary<Vector2Int, Cell>();

        private void Awake()
        {
            SetSingleton();
            PopulateCellDictionaries();
        }

        private void SetSingleton()
        {
            if (singleton == null)
            {
                singleton = this;
            }
            else
            {
                print("ERROR: more than one BattleGrid in scene!");
            }
        }

        private void PopulateCellDictionaries()
        {
            var listOfAllCells = FindObjectsOfType<Cell>();

            var listOfAllPlayerCells = from cell in listOfAllCells
                                       where !cell.IsEnemyCell
                                       where !cell.IsBenchCell
                                       select cell;

            foreach (var cell in listOfAllCells)
            {
                AddNewCellKey(cell.GetComponent<GridPosition>().GetGridPos(), cell);
            }

            foreach (var cell in listOfAllPlayerCells)
            {
                AddNewPlayerCellKey(cell.GetComponent<GridPosition>().GetGridPos(), cell);
            }
        }

        private void AddNewCellKey(Vector2Int key, Cell value)
        {
            allCells.Add(key, value);
        }

        private void AddNewPlayerCellKey(Vector2Int key, Cell value)
        {
            playerCells.Add(key, value);
        }
    }
}
