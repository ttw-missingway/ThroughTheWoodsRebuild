using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SWS;

namespace TTW.Combat
{
    public class AbilityUtilityTool
    {
        private AttackPacket _packet;

        public void LoadPacket(AttackPacket packet)
        {
            _packet = packet;
        }
        public void PerformUtilities()
        {
            if (_packet == null) return;

            if (_packet.Ability.enemyEntity != null)
                CreateEnemy(_packet);
            if (_packet.Ability.displacement != Displacement.none)
                Displace(_packet);
            if (_packet.Ability.portPath != null || _packet.Ability.starboardPath != null || _packet.Ability.bowPath != null)
                SetRailsMotion(_packet);
            if (_packet.Ability.useGridTargeting && _packet.Ability.obstacleCreate != null)
                ObstacleViaGrid(_packet);
            if (_packet.Ability.enemyPrefab != null)
                ChangeForm(_packet);

            CellTargeting(_packet);
        }

        private void ChangeForm(AttackPacket packet)
        {
            MonoBehaviour.Instantiate(packet.Ability.enemyPrefab);
            MonoBehaviour.Destroy(packet.Caster.gameObject);
        }

        private void CreateEnemy(AttackPacket packet)
        {
            if (packet.Ability.enemyEntity.Length > 0)
            {
                int randomIndex = Random.Range(0, packet.Ability.enemyEntity.Length - 1);
                CreateEnemyEntity(packet.Ability.enemyEntity[randomIndex], wingsOnly: packet.Ability.wingsOnly);
            }
                
        }

        private void Displace(AttackPacket packet)
        {
            var orderedTargets = from Targetable t in packet.GetTargets()
                                 select t;

            switch (packet.Ability.globalDirection)
            {
                case DirectionTypes.Left:
                    orderedTargets = from Targetable t in packet.GetTargets()
                                     orderby t.GridPosition.x
                                     select t;
                    break;
                case DirectionTypes.Right:
                    orderedTargets = from Targetable t in packet.GetTargets()
                                     orderby t.GridPosition.x descending
                                     select t;
                    break;
                case DirectionTypes.Up:
                    orderedTargets = from Targetable t in packet.GetTargets()
                                     orderby t.GridPosition.y descending
                                     select t;
                    break;
                case DirectionTypes.Down:
                    orderedTargets = from Targetable t in packet.GetTargets()
                                     orderby t.GridPosition.y
                                     select t;
                    break;
            }

            foreach(Targetable t in orderedTargets)
            {
                DisplaceByDirection(packet, t);
            }
        }

        private void CellTargeting(AttackPacket packet)
        {
            foreach (var t in from Targetable t in packet.GetTargets()
                              where t.GetComponent<Cell>() != null
                              select t)
            {
                if (packet.Ability.trapAbility != null)
                    Trap(packet, t);

                if (packet.Ability.obstacleCreate && !packet.Ability.useGridTargeting)
                    ObstacleViaCell(packet, t);

                if (packet.Ability.displacement == Displacement.leap)
                    Leap(packet, t);
            }
        }

        private static void Leap(AttackPacket packet, Targetable t)
        {
            packet.Caster.GetComponent<GridMover>().Displace(t.GetComponent<Cell>());
        }

        private void Trap(AttackPacket packet, Targetable t)
        {
            t.GetComponent<Cell>().AssembleTrap(packet.Ability.trapShell, packet.Ability.trapAbility, packet.Caster);
        }

        private static void ObstacleViaGrid(AttackPacket packet)
        {
            int obstacleMax = 4;

            var allObstacles = Object.FindObjectsOfType<Obstacle>();

            if (allObstacles.Count() < obstacleMax)
            {
                var targetedCells = new List<Cell>();
                var tTool = new TargetingTool(null, null, null, TargetClass.Actor);
                var grid = BattleGrid.singleton;

                if (packet.Ability.targetingType == TargetingType.gridRelative)
                {
                    foreach (Vector2Int v in packet.Ability.gridTargetingCoordinates)
                    {
                        var newCoord = tTool.TranslateCoordinates(v, packet.Caster.Targetable.GridPosition, packet.Caster.GetComponent<GridPosition>().GetWing());
                        targetedCells.Add(grid.playerCells[newCoord]);
                    }
                }
                else if (packet.Ability.targetingType == TargetingType.gridGlobal)
                {
                    foreach (Vector2Int v in packet.Ability.gridTargetingCoordinates)
                    {
                        targetedCells.Add(grid.playerCells[v]);
                    }
                }
                else
                {
                    return;
                }

                foreach (Cell c in targetedCells)
                {
                    if (!c.IsOccupied)
                    {
                        c.CreateObstacle(packet.Ability.obstacleCreate);
                    }
                }
            }   
        }

        private static void ObstacleViaCell(AttackPacket packet, Targetable t)
        {
            int obstacleMax = 4;
            var allObstacles = Object.FindObjectsOfType<Obstacle>();

            if (allObstacles.Count() < obstacleMax)
            {
                t.GetComponent<Cell>().CreateObstacle(packet.Ability.obstacleCreate);
            }
        }

        private void SetRailsMotion(AttackPacket packet)
        {
            if (packet.Ability.bowPath != null && packet.Caster.GetComponent<GridPosition>().GetWing() == Wing.Bow)
                PerformRails(packet.Ability.bowPath, packet.Caster.Targetable, packet.Ability);
            else if (packet.Ability.starboardPath != null && packet.Caster.GetComponent<GridPosition>().GetWing() == Wing.Starboard)
                PerformRails(packet.Ability.starboardPath, packet.Caster.Targetable, packet.Ability);
            else if (packet.Ability.portPath != null && packet.Caster.GetComponent<GridPosition>().GetWing() == Wing.Port)
                PerformRails(packet.Ability.portPath, packet.Caster.Targetable, packet.Ability);
        }

        private void CreateEnemyEntity(EnemyEntity enemyEntity, bool wingsOnly)
        {
            Vector3 yOffset = new Vector3(0f, -5f, -4f);

            var allCells = Object.FindObjectsOfType<Cell>();

            var allAvailableCells = from Cell c in allCells
                                    where c.IsEnemyCell
                                    where !c.IsOccupied
                                    select c;

            var cellsList = allAvailableCells.ToList();

            if (wingsOnly)
                cellsList.RemoveAll(c => c.GetGridPos().y == 3);

            if (cellsList.Count() == 0) return;

            int randomIndex = Random.Range(0, cellsList.Count() - 1);

            EnemyEntity newEnemy = Object.Instantiate(enemyEntity, cellsList.ElementAt(randomIndex).transform.position + yOffset, Quaternion.identity);

            if (_packet.Ability.summonFX != null)
            {
                VFX summonVFX = Object.Instantiate(_packet.Ability.summonFX, cellsList.ElementAt(randomIndex).transform.position + yOffset, Quaternion.identity);
            }
        }

        private void PerformRails(PathManager path, Targetable caster, Ability ability)
        {
            BattleGrid bg = BattleGrid.singleton;
            Cell endingCell;
            splineMove spline = caster.GetComponent<splineMove>();
            GridMover gridMover = caster.GetComponent<GridMover>();

            if (!ability.relative)
            {
                endingCell = bg.allCells[ability.endingCell];
            }
            else
            {
                endingCell = bg.allCells[caster.GetComponent<AI>().TranslateCoordinates(ability.endingCell, caster.GetComponent<GridPosition>().GetWing())];
            }

            if (!ability.relative && ability.startingCell != caster.GridPosition) return;
            if (!ability.relative && bg.allCells[ability.endingCell].IsOccupied == true) return;
            if (ability.relative && endingCell.IsOccupied == true) return;

            gridMover.StartRailsMovement(spline, path, endingCell);
        }

        private void DisplaceByDirection(AttackPacket packet, Targetable target)
        {
            var caster = packet.Caster.Targetable;

            Pushable casterPushable = caster.GetComponent<Pushable>();
            Pushable targetPushable = target.GetComponent<Pushable>();

            switch (packet.Ability.displacement)
            {
                case Displacement.dash:
                    if (packet.Ability.targetingType != TargetingType.beeline) return;

                    if (target.GetTargetClass() == TargetClass.Enemy)
                    {
                        if (target.GridPosition.y == caster.GridPosition.y && target.GridPosition.x < caster.GridPosition.x)
                            casterPushable.Push(DirectionTypes.Left, 4, anim: true);
                        else if (target.GridPosition.y == caster.GridPosition.y && target.GridPosition.x > caster.GridPosition.x)
                            casterPushable.Push(DirectionTypes.Right, 4, anim: true);
                        else if (target.GridPosition.x == caster.GridPosition.x)
                            casterPushable.Push(DirectionTypes.Up, 4, anim: true);
                    }

                    if (target.GetTargetClass() == TargetClass.Boss)
                    {
                        if (target.GetBossEntity().GridWing == Wing.Bow)
                            casterPushable.Push(DirectionTypes.Up, 4, anim: true);
                        else if (target.GetBossEntity().GridWing == Wing.Port)
                            casterPushable.Push(DirectionTypes.Left, 4, anim: true);
                        else if (target.GetBossEntity().GridWing == Wing.Starboard)
                            casterPushable.Push(DirectionTypes.Left, 4, anim: true);
                    }
                    break;

                case Displacement.push:
                    if (target.GridPosition.y == caster.GridPosition.y && target.GridPosition.x < caster.GridPosition.x)
                        targetPushable.Push(DirectionTypes.Left, 4, anim: false);
                    else if (target.GridPosition.y == caster.GridPosition.y && target.GridPosition.x > caster.GridPosition.x)
                        targetPushable.Push(DirectionTypes.Right, 4, anim: false);
                    else if (target.GridPosition.x == caster.GridPosition.x && target.GridPosition.y > caster.GridPosition.y)
                        targetPushable.Push(DirectionTypes.Up, 4, anim: false);
                    else if (target.GridPosition.x == caster.GridPosition.x && target.GridPosition.y < caster.GridPosition.y)
                        targetPushable.Push(DirectionTypes.Down, 4, anim: false);
                    break;

                case Displacement.random:
                    var randomDirection = Random.Range(0, 4);
                    targetPushable.Push((DirectionTypes)randomDirection, 4, anim: false);
                    break;

                case Displacement.pull:
                    if (target.GridPosition.y == caster.GridPosition.y && target.GridPosition.x < caster.GridPosition.x)
                        targetPushable.Push(DirectionTypes.Right, 4, anim: false);
                    else if (target.GridPosition.y == caster.GridPosition.y && target.GridPosition.x > caster.GridPosition.x)
                        targetPushable.Push(DirectionTypes.Left, 4, anim: false);
                    else if (target.GridPosition.x == caster.GridPosition.x && target.GridPosition.y > caster.GridPosition.y)
                        targetPushable.Push(DirectionTypes.Down, 4, anim: false);
                    else if (target.GridPosition.x == caster.GridPosition.x && target.GridPosition.y < caster.GridPosition.y)
                        targetPushable.Push(DirectionTypes.Up, 4, anim: false);
                    break;

                case Displacement.swap:
                    caster.GetComponent<GridMover>().Swap(target.GetComponent<GridMover>());
                    break;

                case Displacement.warp:
                    var allCells = MonoBehaviour.FindObjectsOfType<Cell>();
                    var availableEnemyCells = from Cell c in allCells
                                     where c.IsEnemyCell
                                     where !c.IsOccupied
                                     select c;

                    var listOfCells = availableEnemyCells.ToList();
                    var randomIndex = UnityEngine.Random.Range(0, listOfCells.Count - 1);
                    var randomCell = listOfCells[randomIndex];

                    caster.GetComponent<GridMover>().Displace(randomCell);
                    break;

                case Displacement.global:
                    targetPushable.Push(packet.Ability.globalDirection, 4, anim: false);
                    break;
            }
        }
    }
}
