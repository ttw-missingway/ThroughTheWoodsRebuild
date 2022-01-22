using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public enum TargetClass
    {
        Actor,
        Enemy,
        Obstacle,
        Cell,
        Boss,
        None
    }

    public class Targetable : MonoBehaviour
    {
        [SerializeField] TargetClass targetClass;

        bool _isTargetable = false;
        int _proximityScore = 0;
        int _directionScore = 0;
        int _totalScore = 0;

        private void Start()
        {
            targetClass = GetTargetClass();
        }

        public void SetTargetable(bool targetableStatus)
        {
            _isTargetable = targetableStatus;
        }

        public bool GetTargetableStatus() => _isTargetable;

        public void SetProximityScore(int score)
        {
            _proximityScore = score;
        }

        public void SetTotalScore(int score)
        {
            _totalScore = score;
        }

        public void SetDirectionScore(int score)
        {
            _directionScore = score;
        }

        public AttackReceiver GetAttackReceiver() => GetComponent<AttackReceiver>();

        public int ProximityScore => _proximityScore;

        public int DirectionScore => _directionScore;

        public int TotalScore => _totalScore;

        public void ClearScore()
        {
            _proximityScore = 0;
            _directionScore = 0;
            _totalScore = 0;
        }

        public TargetClass GetTargetClass()
        {
            if (GetComponent<ActorEntity>() != null) return TargetClass.Actor;
            else if (GetComponent<EnemyEntity>() != null) return TargetClass.Enemy;
            else if (GetComponent<Obstacle>() != null) return TargetClass.Obstacle;
            else if (GetComponent<Cell>() != null) return TargetClass.Cell;
            else if (GetComponent<BossEntity>() != null) return TargetClass.Boss;
            else return TargetClass.None;
        }

        public ActorEntity GetActorEntity()
        {
            if (GetComponent<ActorEntity>() != null)
            {
                return GetComponent<ActorEntity>();
            }
            else
            {
                print("ERROR: trying to access ActorEntity where no ActorEntity exists");
                return null;
            }
        }

        public BossEntity GetBossEntity()
        {
            if (GetComponent<BossEntity>() != null)
            {
                return GetComponent<BossEntity>();
            }
            else
            {
                print("ERROR: trying to access BossEntity where no BossEntity exists");
                return null;
            }
        }

        public EnemyEntity GetEnemyEntity()
        {
            if (GetComponent<EnemyEntity>() != null) return GetComponent<EnemyEntity>();
            else
            {
                print("ERROR: trying to access EnemyEntity where no EnemyEntity exists");
                return null;
            }
        }

        public Cell GetCell()
        {
            if (GetComponent<Cell>() != null) return GetComponent<Cell>();
            else
            {
                print("ERROR: trying to access Cell where no Cell exists");
                return null;
            }
        }

        public StatsHandler Stats => GetComponent<StatsHandler>();

        public Vector2Int GridPosition => GetComponent<GridPosition>().GetGridPos();

        public AttackExecutor AttackExecutor => GetComponent<AttackExecutor>();

        public AttackReceiver AttackReceiver => GetComponent<AttackReceiver>();
    }
}
