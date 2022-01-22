using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class Obstacle : MonoBehaviour
    {
        BattleGrid battleGrid;
        Cell _currentCell;
        Targetable _parent;
        Vector2Int _gridPos;
        public bool _flaggedToDestroy = false;

        [SerializeField] bool destroyable;
        [SerializeField] Ability ability;

        private void Start()
        {
            _gridPos = GetComponent<GridPosition>().GetGridPos();
            battleGrid = BattleGrid.singleton;
            _currentCell = battleGrid.allCells[_gridPos];
            _currentCell.SetOccupied(true);
        }

        public void ReceiveAttackPacket(AttackPacket packet)
        {
            _parent = packet.Caster.Targetable;
            
            if (packet.Ability.damageType == DamageType.physical)
            {
                if (ability != null)
                {
                    CastObstacleAbility();
                }

                if (destroyable)
                {
                    _flaggedToDestroy = true;
                    _currentCell.SetOccupied(false);
                    Destroy(gameObject);
                }
            }
        }

        private void CastObstacleAbility()
        {
            Directions directions = new Directions();
            var allTargetables = FindObjectsOfType<Targetable>();
            AttackPacket obstaclePacket = new AttackPacket(ability, _parent.GetComponent<AttackExecutor>());

            foreach (var t in from Targetable t in allTargetables
                              from Vector2Int d in directions.AllOrdinalDirections
                              where _gridPos + d == t.GridPosition
                              select t)
            {
                if (t.GetTargetClass() == TargetClass.Cell) continue;
                if (t == GetComponent<Targetable>()) continue;

                if (t.GetTargetClass() == TargetClass.Obstacle)
                {
                    if (!t.GetComponent<Obstacle>()._flaggedToDestroy)
                    {
                        _flaggedToDestroy = true;
                        t.GetComponent<Obstacle>().ReceiveAttackPacket(obstaclePacket);
                        continue;
                    }
                }
                else
                {
                    t.GetComponent<AttackReceiver>().ReceiveAttackPacket(obstaclePacket);
                }
            }
        }
    }
}
