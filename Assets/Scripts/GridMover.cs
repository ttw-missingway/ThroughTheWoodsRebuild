using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;

namespace TTW.Combat
{
    [RequireComponent(typeof(GridPosition))]
    public class GridMover : MonoBehaviour
    {
        List<Cell> _path;
        [SerializeField] float _speed = 9.8f;
        [SerializeField] float _updatePathSpeed = 1f;
        [SerializeField] Vector2Int _testCell;
        [SerializeField] bool _beingPushed;
        private bool _isMovingToTarget = false;

        Pathfinder pathfinder;
        BattleGrid battleGrid;
        AnimationController animationController;
        GridPosition gridPosition;
        splineMove spline;
        ICombatAnimation animManager;

        [SerializeField] Cell _finalCell;
        [SerializeField] Cell _currentCell;
        [SerializeField] Cell _target;

        private void Start()
        {
            if (GetComponent<ICombatAnimation>() != null)
            {
                animManager = GetComponent<ICombatAnimation>();
            }

            gridPosition = GetComponent<GridPosition>();
            pathfinder = GetComponent<Pathfinder>();
            battleGrid = BattleGrid.singleton;
            animationController = AnimationController.singleton;
            GetCurrentCell();
            SetCurrentCellOccupied();

            if (GetComponent<splineMove>() != null)
                SetUpSplineListener();

        }

        private void SetUpSplineListener()
        {
            spline = GetComponent<splineMove>();
            spline.onPathEnd += Spline_onPathEnd;
        }

        private void SetCurrentCellOccupied()
        {
            _currentCell.SetOccupied(true);
        }

        private void Update()
        {
            CheckForTraps();
            UpdateCurrentCell();

            if (!_isMovingToTarget) return;

            MoveToTarget(_target);
            
        }

        private void UpdateCurrentCell()
        {
            _currentCell = GetCurrentCell();
        }

        private void CheckForTraps()
        {
            if (_currentCell.HasTrap)
            {
                if (GetComponent<AttackReceiver>() != null)
                {
                    _currentCell.TriggerTrap(GetComponent<AttackReceiver>());
                }
            }
        }

        private Cell GetCurrentCell()
        {
            var gridPos = gridPosition.GetGridPos();
            _currentCell = battleGrid.allCells[gridPos];

            return _currentCell;
        }

        public void Displace(Cell endingCell)
        {
            _currentCell.SetOccupied(false);
            transform.position = endingCell.transform.position;
            _currentCell = endingCell;
            _currentCell.SetOccupied(true);
        }

        public void Swap(GridMover swapee)
        {
            var oldPosition = transform.position;

            transform.position = swapee.transform.position;
            swapee.transform.position = oldPosition;

            GetCurrentCell();
            swapee.GetCurrentCell();
        }

        public void SetBeingPushed()
        {
            _beingPushed = true;
        }

        public void Pathfind(Cell endingCell)
        {
            ClearPath(pathfinder);

            _finalCell = endingCell;
            _finalCell.SetFlagForArrival(true);
            _currentCell.SetOccupied(false);
            _path = pathfinder.GetFinalPath(_currentCell, endingCell);

            if (_path.Count > 1)
            {
                StartCoroutine(MoveOnPath(_path));
                //GameEvents.current.AnimationStart();
                //GetComponent<AttackReceiver>().BreakNeutralState(true);
            }
            else
            {
                _finalCell.SetFlagForArrival(false);
                _currentCell.SetOccupied(true);
                _beingPushed = false;
            }
            ClearPath(pathfinder);
        }

        private void ClearPath(Pathfinder pathfinder)
        {
            Cell[] cells = FindObjectsOfType<Cell>();

            foreach (Cell cell in cells)
            {
                cell.SetExplored(false);
                cell.SetExploredFrom(null);
            }
        }

        IEnumerator MoveOnPath(List<Cell> path)
        {
            AnimateEntityOnPath();

            foreach (Cell coordinate in path)
            {
                if (coordinate == path[0]) continue;

                if (GetComponent<ActorEntity>() != null)
                {
                    AnimateActors(coordinate);
                }

                _isMovingToTarget = true;
                _target = coordinate;

                yield return new WaitForSeconds(_updatePathSpeed);
            }
        }

        private void AnimateActors(Cell coordinate)
        {
            if (_beingPushed) return;

            ActorEntity actor = GetComponent<ActorEntity>();

            if (coordinate.GetGridPos().x > gridPosition.GetGridPos().x)
            {
                actor.ChangeFacingDirection(DirectionTypes.Left);
            }
            else if (coordinate.GetGridPos().x < gridPosition.GetGridPos().x)
            {
                actor.ChangeFacingDirection(DirectionTypes.Right);
            }
            else if (coordinate.GetGridPos().y < gridPosition.GetGridPos().y)
            {
                actor.ChangeFacingDirection(DirectionTypes.Down);
            }
            else
            {
                actor.ChangeFacingDirection(DirectionTypes.Up);
            }

            animManager.ChangeAnimationState(CombatAnimStates.Run, actor.GetFacingDirection());
        }

        private void AnimateEntityOnPath()
        {
            animationController.StartAnimationFreeze();
        }

        private void MoveToTarget(Cell target)
        {
            float finalDistance = Vector3.Distance(transform.position, target.transform.position);
            float step = _speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);

            if (target == _finalCell && finalDistance < 1f)
            {
                EndOfMovementPath(target);
            }
        }

        private void EndOfMovementPath(Cell target)
        {
            _isMovingToTarget = false;
            _beingPushed = false;
            target.SetOccupied(true);
            target.SetFlagForArrival(false);
            GetCurrentCell();

            animationController.EndAnimationFreeze();

            if (GetComponent<ActorEntity>() != null)
            {
                animManager.ChangeAnimationState(CombatAnimStates.Idle, GetComponent<ActorEntity>().GetFacingDirection());
            }
        }

        public void StartRailsMovement(splineMove spline, PathManager path, Cell destination)
        {
            animationController.StartAnimationFreeze();
            spline.pathContainer = path;
            spline.StartMove();
            _currentCell.SetOccupied(false);
            destination.SetOccupied(true);
        }

        private void Spline_onPathEnd(object sender, EventArgs e)
        {
            animManager.ChangeAnimationState(CombatAnimStates.Idle, DirectionTypes.None);
            animationController.EndAnimationFreeze();
        }
    }
}
