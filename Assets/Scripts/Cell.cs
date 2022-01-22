using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TTW.Combat
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] bool _isEnemyCell = false;
        [SerializeField] bool _isBenchCell = false;

        [SerializeField] private bool _isOccupied;
        [SerializeField] private bool _isExplored;
        [SerializeField] private bool _hasTrap;
        [SerializeField] private bool _isFlaggedForArrival;
        [SerializeField] Trap _trap;
        [SerializeField] MeshRenderer highlight;
        [SerializeField] Material highlightMaterial;
        [SerializeField] Material selectedMaterial;
        private Cell _exploredFrom;

        public bool IsOccupied => _isOccupied;
        public bool IsExplored => _isExplored;
        public Cell ExploredFrom => _exploredFrom;
        public bool IsEnemyCell => _isEnemyCell;
        public bool IsBenchCell => _isBenchCell;
        public bool HasTrap => _hasTrap;
        public bool IsFlaggedForArrival => _isFlaggedForArrival;

        GridPosition gridPosition;

        const int gridSize = 10;

        private void Awake()
        {
            gridPosition = GetComponent<GridPosition>();
        }

        public void SetTrap(Trap trap)
        {
            if (_trap != null) return;

            _hasTrap = true;
            _trap = trap;
        }

        public void CreateObstacle(Obstacle obstacle)
        {
            Vector3 yOffset = new Vector3(0f, 10f, 0f);
            Instantiate(obstacle, transform.position + yOffset, Quaternion.identity);
        }

        public void AssembleTrap(Trap trapShell, Ability trapAbility, AttackExecutor attacker)
        {
            var newTrap = Instantiate(trapShell);
            newTrap.SetParent(attacker);
            newTrap.SetAbility(trapAbility);
            SetTrap(newTrap);
            newTrap.Materialize(this);
        }

        public void TriggerTrap(AttackReceiver attackReceiver)
        {
            _trap.Activate(attackReceiver);
            _trap = null;
            _hasTrap = false;
        }

        public void SetFlagForArrival(bool flag) => _isFlaggedForArrival = flag;

        public void SetOccupied(bool occupiedStatus)
        {
            _isOccupied = occupiedStatus;
        }

        public void SetExplored(bool exploredStatus)
        {
            _isExplored = exploredStatus;
        }

        public void SetExploredFrom(Cell cell)
        {
            _exploredFrom = cell;
        }

        public Vector2Int GetGridPos()
        {
            return gridPosition.GetGridPos();
        }

        public int GetGridSize()
        {
            return gridSize;
        }

        public void Highlight()
        {
            highlight.material = highlightMaterial;
            highlight.enabled = true;
        }

        public void HighlightSelected()
        {
            highlight.material = selectedMaterial;
        }

        public void ClearHighlight()
        {
            if (IsBenchCell) return;

            highlight.enabled = false;
        }
    }
}
