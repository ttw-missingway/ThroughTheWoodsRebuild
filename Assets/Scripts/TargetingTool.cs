using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class TargetingTool
    {
        List<Targetable> _allTargetables;
        List<Targetable> _availableTargetables = new List<Targetable>();
        List<Targetable> _selectedTargetables = new List<Targetable>();
        List<Targetable> _allCells = new List<Targetable>();
        IEnumerable<Targetable> _allAllies;
        IEnumerable<Targetable> _allFoes;
        IEnumerable<Targetable> _allBosses;
        Ability _ability;
        Targetable _caster;
        TargetClass _tClass;
        Directions directions = new Directions();
        List<Cell> _targetableCells = new List<Cell>();

        public TargetingTool(List<Targetable> allTargetables, Ability ability, Targetable caster, TargetClass tClass)
        {
            _allTargetables = allTargetables;
            _ability = ability;
            _caster = caster;
            _tClass = tClass;
        }

        public List<Cell> TargetableCells()
        {
            MonoBehaviour.print("targetableCell Count: " + _targetableCells.Count);

            return _targetableCells;
        }

        private void GlobalGridTargeting(Vector2Int[] coordinates)
        {
            List<Targetable> targetables = new List<Targetable>();

            var targetsOnCoordinates = from Targetable t in _availableTargetables
                                       where coordinates.Contains(t.GridPosition)
                                       select t;

            _selectedTargetables.AddRange(from t in targetsOnCoordinates
                                 select t);

            _targetableCells.Clear();
            _targetableCells = FindGlobalGridCells(coordinates);
        }

        private List<Cell> FindGlobalGridCells(Vector2Int[] coordinates)
        {
            var globalGridCells = from Targetable c in _allCells
                                  where coordinates.Contains(c.GridPosition)
                                  select c.GetComponent<Cell>();

            return globalGridCells.ToList();
        }

        private void RelativeGridTargeting(Vector2Int[] coordinates, Targetable caster, Wing wing)
        {
            Vector2Int[] newCoordinates = new Vector2Int[coordinates.Length];

            for (int i = 0; i < coordinates.Length; i++)
            {
                newCoordinates[i] = TranslateCoordinates(coordinates[i], caster.GridPosition, wing);
            }

            GlobalGridTargeting(newCoordinates);

            _targetableCells.Clear();
            _targetableCells = FindGlobalGridCells(newCoordinates);
        }

        private void SelectTargetablesSupportAdjacent(Vector2Int casterPos)
        {
            Directions directions = new Directions();

            if (_tClass == TargetClass.Actor)
            {
                _allAllies = from t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Actor
                                select t;
            }
            else if (_tClass == TargetClass.Enemy)
            {
                _allAllies = from t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Enemy
                                select t;
            }
            else
            {
                AccessError();
                return;
            }

            _selectedTargetables.AddRange(from a in _allAllies
                                          from d in directions.AllDirections
                                          where casterPos + d == a.GridPosition
                                          select a);

            _targetableCells.Clear();
            _targetableCells = ShowAdjacentCells(casterPos);
        }

        private List<Cell> ShowAdjacentCells(Vector2Int casterPos)
        {
            var _adjacentCells = from Targetable c in _allCells
                                 from d in directions.AllDirections
                                 where casterPos + d == c.GridPosition
                                 select c.GetComponent<Cell>();

            return _adjacentCells.ToList();
        }

        private List<Cell> ShowOrdinalCells(Vector2Int casterPos)
        {
            var _ordinalCells = from Targetable c in _allCells
                                 from d in directions.AllOrdinalDirections
                                 where casterPos + d == c.GridPosition
                                 select c.GetComponent<Cell>();

            return _ordinalCells.ToList();
        }

        private void SelectTargetablesSupport()
        {
            if (_tClass == TargetClass.Actor)
            {
                _allAllies = from t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Actor
                                select t;
            }
            else if (_tClass == TargetClass.Enemy || _tClass == TargetClass.Boss)
            {
                _allAllies = from t in _availableTargetables
                             where t.GetTargetClass() == TargetClass.Enemy || t.GetTargetClass() == TargetClass.Boss
                             select t;
            }

            _selectedTargetables.AddRange(from a in _allAllies
                             select a);

            _targetableCells.Clear();
            _targetableCells = ShowSupportCells(_allAllies);
        }

        private List<Cell> ShowSupportCells(IEnumerable<Targetable> allAllies)
        {
            List<Cell> fullList = new List<Cell>();

            var _bossTargets = from a in allAllies
                               where a.GetTargetClass() == TargetClass.Boss
                               select a;

            var _supportCells = from Targetable c in _allCells
                                from a in allAllies
                                where c.GridPosition == a.GridPosition && a.GetTargetClass() != TargetClass.Boss
                                select c.GetComponent<Cell>();

            fullList.AddRange(from c in _supportCells
                              select c);

            var _portCells = from b in _bossTargets
                             from c in _allCells
                             where b.GetComponent<GridPosition>().GetWing() == Wing.Port && c.GridPosition.x == 0
                             select c;

            fullList.AddRange(from c in _portCells
                              select c.GetComponent<Cell>());

            var _starboardCells = from b in _bossTargets
                                  from c in _allCells
                                  where b.GetComponent<GridPosition>().GetWing() == Wing.Starboard && c.GridPosition.x == 5
                                  select c;

            fullList.AddRange(from c in _starboardCells
                              select c.GetComponent<Cell>());

            var _bowCells = from b in _bossTargets
                                  from c in _allCells
                                  where b.GetComponent<GridPosition>().GetWing() == Wing.Bow && c.GridPosition.y == 3
                                  select c;

            fullList.AddRange(from c in _bowCells
                              select c.GetComponent<Cell>());



            return fullList;
        }

        private void SelectTargetablesSelf(Targetable caster)
        {
            _selectedTargetables.Add(caster);

            _targetableCells.Clear();
            _targetableCells = ShowSelfCell(caster);
        }

        private List<Cell> ShowSelfCell(Targetable caster)
        {
            var _selfCell = from Targetable c in _allCells
                            where c.GridPosition == caster.GridPosition
                            select c.GetComponent<Cell>();

            return _selfCell.ToList();
        }

        private void SelectTargetablesSupportNotSelf(Targetable caster)
        {
            if (_tClass == TargetClass.Actor)
            {
                _allAllies = from t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Actor
                                select t;
            }
            else if (_tClass == TargetClass.Enemy || _tClass == TargetClass.Boss)
            {
                _allAllies = from t in _availableTargetables
                             where t.GetTargetClass() == TargetClass.Enemy || t.GetTargetClass() == TargetClass.Boss
                             select t;
            }
            else
            {
                AccessError();
                return;
            }
            
            var alliesNotSelf =
                            from a in _allAllies
                            where a != caster
                            select a;

            _selectedTargetables.AddRange(from a in alliesNotSelf
                             select a);

            _targetableCells.Clear();
            _targetableCells = ShowSupportCells(alliesNotSelf);
        }

        private void SelectTargetablesCell()
        {
            var availableCells =
                            from c in _allCells
                            where !c.GetComponent<Cell>().IsOccupied
                            select c;

            _selectedTargetables.AddRange(from c in availableCells
                             select c);

            _targetableCells.Clear();
            _targetableCells = ShowSupportCells(availableCells);
        }

        private void SelectTargetablesRandom()
        {
            if (_tClass == TargetClass.Actor)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Enemy || t.GetTargetClass() == TargetClass.Boss || t.GetTargetClass() == TargetClass.Obstacle
                            select t;
            }
            else if (_tClass == TargetClass.Enemy || _tClass == TargetClass.Boss)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Actor || t.GetTargetClass() == TargetClass.Obstacle
                            select t;
            }
            else
            {
                AccessError();
                return;
            }

            int randomIndex = Random.Range(0, _allFoes.Count());
            _selectedTargetables.Add(_allFoes.ElementAt(randomIndex));

            _targetableCells.Clear();
            _targetableCells = ShowSupportCells(_allFoes);
        }

        private void SelectTargetablesBeeline(Vector2Int casterPos)
        {
            MonoBehaviour.print("highlighting beelineCells");

            //come back to this
            if (_tClass == TargetClass.Enemy)
            {
                var allTargets =
                              from t in _availableTargetables
                              where t.GetTargetClass() == TargetClass.Actor || t.GetTargetClass() == TargetClass.Obstacle
                              select t;

                var allTargetsInLeftView =
                                from t in allTargets
                                where t.GridPosition.y == casterPos.y && t.GridPosition.x < casterPos.x
                                select t;

                var allTargetsInRightView =
                                from t in allTargets
                                where t.GridPosition.y == casterPos.y && t.GridPosition.x > casterPos.x
                                select t;

                var allTargetsInFrontView =
                                from t in allTargets
                                where t.GridPosition.x == casterPos.x
                                select t;

                var actorsSortedRightMost =
                                from t in allTargetsInRightView
                                orderby t.GridPosition.x
                                select t;

                var actorsSortedLeftMost =
                                from t in allTargetsInLeftView
                                orderby t.GridPosition.x descending
                                select t;

                var actorsSortedTopMost =
                                from t in allTargetsInFrontView
                                orderby t.GridPosition.y descending
                                select t;

                if (actorsSortedRightMost.Count() > 0)
                    _selectedTargetables.Add(actorsSortedRightMost.ToList()[0]);

                if (actorsSortedLeftMost.Count() > 0)
                    _selectedTargetables.Add(actorsSortedLeftMost.ToList()[0]);

                if (actorsSortedTopMost.Count() > 0)
                    _selectedTargetables.Add(actorsSortedTopMost.ToList()[0]);
            }
            else if (_tClass == TargetClass.Actor)
            {
                var allEnemies =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Enemy || t.GetTargetClass() == TargetClass.Obstacle
                            select t;

                var allActors =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Actor
                            select t;

                var allBosses =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Boss
                            select t;

                var actorsInLeftView =
                            from t in allActors
                            where t.GridPosition.y == casterPos.y && t.GridPosition.x < casterPos.x
                            select t;

                var actorsInRightView =
                            from t in allActors
                            where t.GridPosition.y == casterPos.y && t.GridPosition.x > casterPos.x
                            select t;

                var actorsInFrontView =
                                from t in allActors
                                where t.GridPosition.x == casterPos.x && t.GridPosition.y > casterPos.y
                                select t;

                var actorsInBottomView =
                                from t in allActors
                                where t.GridPosition.x == casterPos.x && t.GridPosition.y < casterPos.y
                                select t;

                var allEnemiesInLeftView =
                                from t in allEnemies
                                where t.GridPosition.y == casterPos.y && t.GridPosition.x < casterPos.x
                                select t;

                var allEnemiesInRightView =
                                from t in allEnemies
                                where t.GridPosition.y == casterPos.y && t.GridPosition.x > casterPos.x
                                select t;

                var allEnemiesInFrontView =
                                from t in allEnemies
                                where t.GridPosition.x == casterPos.x && t.GridPosition.y > casterPos.y
                                select t;

                var allEnemiesInBottomView =
                                from t in allEnemies
                                where t.GridPosition.x == casterPos.x && t.GridPosition.y < casterPos.y
                                select t;

                var allBossesInFrontView =
                                from t in allBosses
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Bow
                                select t;

                var allBossesInLeftView =
                                from t in allBosses
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Port
                                select t;

                var allBossesInRightView =
                                from t in allBosses
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Starboard
                                select t;

                var enemiesSortedRightMost =
                                from t in allEnemiesInRightView
                                orderby t.GridPosition.x
                                select t;

                var enemiesSortedLeftMost =
                                from t in allEnemiesInLeftView
                                orderby t.GridPosition.x descending
                                select t;

                var enemiesSortedTopMost =
                                from t in allEnemiesInFrontView
                                orderby t.GridPosition.y
                                select t;

                var enemiesSortedBottomMost =
                                from t in allEnemiesInBottomView
                                orderby t.GridPosition.y descending
                                select t;

                if (enemiesSortedRightMost.Count() > 0 && actorsInRightView.Count() == 0)
                    _selectedTargetables.Add(enemiesSortedRightMost.ToList()[0]);

                if (enemiesSortedLeftMost.Count() > 0 && actorsInLeftView.Count() == 0)
                    _selectedTargetables.Add(enemiesSortedLeftMost.ToList()[0]);

                if (enemiesSortedTopMost.Count() > 0 && actorsInFrontView.Count() == 0)
                    _selectedTargetables.Add(enemiesSortedTopMost.ToList()[0]);

                if (enemiesSortedBottomMost.Count() > 0 && actorsInBottomView.Count() == 0)
                    _selectedTargetables.Add(enemiesSortedBottomMost.ToList()[0]);

                if (allBossesInFrontView.Count() > 0 && actorsInFrontView.Count() == 0 && allEnemiesInFrontView.Count() == 0)
                    _selectedTargetables.Add(allBossesInFrontView.ToList()[0]);

                if (allBossesInLeftView.Count() > 0 && actorsInLeftView.Count() == 0 && allEnemiesInLeftView.Count() == 0)
                    _selectedTargetables.Add(allBossesInLeftView.ToList()[0]);

                if (allBossesInRightView.Count() > 0 && actorsInRightView.Count() == 0 && allEnemiesInRightView.Count() == 0)
                    _selectedTargetables.Add(allBossesInRightView.ToList()[0]);

                _targetableCells.Clear();
                _targetableCells = ShowBeelineCells(casterPos);
            }
        }

        private List<Cell> ShowBeelineCells(Vector2Int casterPos)
        {
            List<Cell> beelineCells = new List<Cell>();

            var actualTargetCells = from c in _allCells
                               from t in _selectedTargetables
                               where c.GridPosition == t.GridPosition
                               select c;

            foreach (Targetable t in _selectedTargetables)
            {
                if (t.GetTargetClass() == TargetClass.Boss)
                {
                    if (t.GetComponent<GridPosition>().GetWing() == Wing.Port)
                    {
                        var portBossCells = from c in _allCells
                                            where c.GridPosition.x == 0 && c.GridPosition.y == casterPos.y
                                            select c;

                        beelineCells.AddRange(from c in portBossCells
                                              select c.GetComponent<Cell>());
                    }

                    if (t.GetComponent<GridPosition>().GetWing() == Wing.Starboard)
                    {
                        var starboardBossCells = from c in _allCells
                                            where c.GridPosition.x == 5 && c.GridPosition.y == casterPos.y
                                            select c;

                        beelineCells.AddRange(from c in starboardBossCells
                                              select c.GetComponent<Cell>());
                    }

                    if (t.GetComponent<GridPosition>().GetWing() == Wing.Bow)
                    {
                        var bowBossCells = from c in _allCells
                                            where c.GridPosition.y == 3 && c.GridPosition.x == casterPos.x
                                            select c;

                        beelineCells.AddRange(from c in bowBossCells
                                              select c.GetComponent<Cell>());
                    }
                }
            }

            beelineCells.AddRange(from a in actualTargetCells
                                  select a.GetComponent<Cell>());

            var targetsPort = from c in beelineCells
                              where c.GetGridPos().x < casterPos.x
                              select c;

            var targetsStarboard = from c in beelineCells
                                   where c.GetGridPos().x > casterPos.x
                                   select c;

            var targetsBow = from c in beelineCells
                             where c.GetGridPos().y > casterPos.y
                             select c;

            var targetsStern = from c in beelineCells
                               where c.GetGridPos().y < casterPos.y
                               select c;

            var horizontalCells = from c in _allCells
                                  where c.GridPosition.y == casterPos.y
                                  select c;

            var verticalCells = from c in _allCells
                                where c.GridPosition.x == casterPos.x
                                select c;

            if (targetsPort.ToList().Count > 0)
            {
                var portsidefoe = targetsPort.ToList()[0];

                var portLine = from c in horizontalCells
                               where c.GridPosition.x < casterPos.x && c.GridPosition.x > portsidefoe.GetGridPos().x
                               select c;

                beelineCells.AddRange(from t in portLine
                                      select t.GetComponent<Cell>());
            }

            if (targetsStarboard.ToList().Count > 0)
            {
                var starboardsidefoe = targetsStarboard.ToList()[0];

                var starboardLine = from c in horizontalCells
                               where c.GridPosition.x > casterPos.x && c.GridPosition.x < starboardsidefoe.GetGridPos().x
                               select c;

                beelineCells.AddRange(from t in starboardLine
                                      select t.GetComponent<Cell>());
            }

            if (targetsBow.ToList().Count > 0)
            {
                var bowsidefoe = targetsBow.ToList()[0];

                var bowLine = from c in verticalCells
                              where c.GridPosition.y > casterPos.y && c.GridPosition.y < bowsidefoe.GetGridPos().y
                              select c;

                beelineCells.AddRange(from t in bowLine
                                      select t.GetComponent<Cell>());
            }

            if (targetsStern.ToList().Count > 0)
            {
                var sternsidefoe = targetsStern.ToList()[0];

                var sternLine = from c in verticalCells
                              where c.GridPosition.y < casterPos.y && c.GridPosition.y > sternsidefoe.GetGridPos().y
                              select c;

                beelineCells.AddRange(from t in sternLine
                                      select t.GetComponent<Cell>());
            }

            return beelineCells;
        }

        private void SelectTargetablesVolley()
        {
            if (_tClass == TargetClass.Actor)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Enemy || t.GetTargetClass() == TargetClass.Obstacle
                            select t;

                _allBosses =
                                from t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Boss
                                select t;

                _selectedTargetables.AddRange(from Targetable t in _allBosses
                                              select t);
            }
            else if (_tClass == TargetClass.Enemy || _tClass == TargetClass.Boss)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Actor
                            select t;
            }
            else
            {
                AccessError();
                return;
            }

            _selectedTargetables.AddRange(from Targetable t in _allFoes
                             select t);

            _targetableCells.Clear();
            _targetableCells = ShowSupportCells(_selectedTargetables);
        }

        private void SelectTargetablesMelee(Vector2Int casterPos)
        {
            if (_tClass == TargetClass.Actor)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Enemy || t.GetTargetClass() == TargetClass.Obstacle
                            select t;

                _allBosses =
                                from t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Boss 
                                where t.GetBossEntity().Distant == false
                                select t;

                var _allPortFoes =
                                from t in _allFoes
                                from Vector2Int d in directions.AllDirections
                                where casterPos + d == t.GridPosition
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Port
                                select t;

                var _allStarboardFoes =
                                from t in _allFoes
                                from Vector2Int d in directions.AllDirections
                                where casterPos + d == t.GridPosition
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Starboard
                                select t;

                var _allBowFoes =
                                from t in _allFoes
                                from Vector2Int d in directions.AllDirections
                                where casterPos + d == t.GridPosition
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Bow
                                select t;

                if (_allPortFoes.Count() == 0)
                {
                    _selectedTargetables.AddRange(from Targetable t in _allBosses
                                                  where casterPos.x == 1
                                                  where t.GetBossEntity().GridWing == Wing.Port
                                                  select t);
                }

                if (_allStarboardFoes.Count() == 0)
                {
                    _selectedTargetables.AddRange(from Targetable t in _allBosses
                                                  where casterPos.x == 4
                                                  where t.GetBossEntity().GridWing == Wing.Starboard
                                                  select t);
                }

                if (_allBowFoes.Count() == 0)
                {
                    _selectedTargetables.AddRange(from Targetable t in _allBosses
                                                  where casterPos.y == 2
                                                  where t.GetBossEntity().GridWing == Wing.Bow
                                                  select t);
                }
            }
            else if (_tClass == TargetClass.Enemy)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Actor
                            select t;
            }
            else
            {
                AccessError();
                return;
            }

            _selectedTargetables.AddRange(from Targetable t in _allFoes
                             from Vector2Int d in directions.AllDirections
                             where casterPos + d == t.GridPosition
                             select t);

            _targetableCells.Clear();
            _targetableCells = ShowAdjacentCells(casterPos);
        }

        private void SelectTargetablesOrdinal(Vector2Int casterPos)
        {
            if (_tClass == TargetClass.Actor)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Enemy || t.GetTargetClass() == TargetClass.Obstacle
                            select t;

                _allBosses =
                                from t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Boss
                                where t.GetBossEntity().Distant == false
                                select t;

                var _allPortFoes =
                                from t in _allFoes
                                from Vector2Int d in directions.AllOrdinalDirections
                                where casterPos + d == t.GridPosition
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Port
                                select t;

                var _allStarboardFoes =
                                from t in _allFoes
                                from Vector2Int d in directions.AllOrdinalDirections
                                where casterPos + d == t.GridPosition
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Starboard
                                select t;

                var _allBowFoes =
                                from t in _allFoes
                                from Vector2Int d in directions.AllOrdinalDirections
                                where casterPos + d == t.GridPosition
                                where t.GetComponent<GridPosition>().GetWing() == Wing.Bow
                                select t;

                if (_allPortFoes.Count() == 0)
                {
                    _selectedTargetables.AddRange(from Targetable t in _allBosses
                                                  where casterPos.x == 1
                                                  where t.GetBossEntity().GridWing == Wing.Port
                                                  select t);
                }

                if (_allStarboardFoes.Count() == 0)
                {
                    _selectedTargetables.AddRange(from Targetable t in _allBosses
                                                  where casterPos.x == 4
                                                  where t.GetBossEntity().GridWing == Wing.Starboard
                                                  select t);
                }

                if (_allBowFoes.Count() == 0)
                {
                    _selectedTargetables.AddRange(from Targetable t in _allBosses
                                                  where casterPos.y == 2
                                                  where t.GetBossEntity().GridWing == Wing.Bow
                                                  select t);
                }
            }
            else if (_tClass == TargetClass.Enemy)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Actor
                            select t;
            }
            else
            {
                AccessError();
                return;
            }

            _selectedTargetables.AddRange(from Targetable t in _allFoes
                                          from Vector2Int d in directions.AllOrdinalDirections
                                          where casterPos + d == t.GridPosition
                                          select t);

            _targetableCells.Clear();
            _targetableCells = ShowOrdinalCells(casterPos);
        }

        private void SelectTargetablesCardinalAlly(Vector2Int casterPos)
        {
            if (_tClass != TargetClass.Actor)
            {
                AccessError();
                return;
            }

            _allAllies = from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Actor
                            select t;

            var alliesLeftView =
                            from a in _allAllies
                            where a.GridPosition.y == casterPos.y && a.GridPosition.x < casterPos.x
                            select a;

            var alliesRightView =
                            from a in _allAllies
                            where a.GridPosition.y == casterPos.y && a.GridPosition.x > casterPos.x
                            select a;

            var alliesFrontView =
                            from a in _allAllies
                            where a.GridPosition.x == casterPos.x && a.GridPosition.y > casterPos.y
                            select a;

            var alliesBackView =
                            from a in _allAllies
                            where a.GridPosition.x == casterPos.x && a.GridPosition.y < casterPos.y
                            select a;

            _selectedTargetables.AddRange(from Targetable a in alliesLeftView
                             select a);

            _selectedTargetables.AddRange(from Targetable a in alliesRightView
                             select a);

            _selectedTargetables.AddRange(from Targetable a in alliesFrontView
                             select a);

            _selectedTargetables.AddRange(from Targetable a in alliesBackView
                             select a);

            _targetableCells.Clear();
            _targetableCells = ShowBeelineCells(casterPos);
        }

        private void SelectTargetablesAllEnemies()
        {
            if (_tClass == TargetClass.Actor)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Enemy
                            select t;

                _allBosses =
                                from t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Boss
                                select t;

                _selectedTargetables.AddRange(from t in _allBosses
                                 select t);
            }
            else if (_tClass == TargetClass.Enemy || _tClass == TargetClass.Boss)
            {
                _allFoes =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Actor
                            select t;
            }



            _selectedTargetables.AddRange(from t in _allFoes
                             select t);

            _targetableCells.Clear();
            _targetableCells = ShowSupportCells(_selectedTargetables);
        }

        private void SelectTargetablesAllAllies()
        {
            if (_tClass == TargetClass.Actor)
            {
                _allAllies =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Actor
                            select t;
            }
            else if (_tClass == TargetClass.Enemy || _tClass == TargetClass.Boss)
            {
                _allAllies =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Enemy
                            select t;

                _allBosses =
                            from t in _availableTargetables
                            where t.GetTargetClass() == TargetClass.Boss
                            select t;

                _selectedTargetables.AddRange(from t in _allBosses
                                              select t);
            }

            _selectedTargetables.AddRange(from t in _allAllies
                             select t);

            _targetableCells.Clear();
            _targetableCells = ShowSupportCells(_selectedTargetables);
        }

        private static void AccessError()
        {
            Debug.Log("Ability accessed by forbidden Target Class");
        }

        public Vector2Int TranslateCoordinates(Vector2Int targetCoord, Vector2Int casterCoord, Wing wing)
        {
            int baseX = casterCoord.x;
            int baseY = casterCoord.y;
            int newX = targetCoord.x;
            int newY = targetCoord.y;

            switch (wing)
            {
                case Wing.Port:
                    newX = targetCoord.y;
                    newY = baseY + targetCoord.x;
                    break;
                case Wing.Starboard:
                    newX = baseX - targetCoord.y;
                    newY = baseY - targetCoord.x;
                    break;
                case Wing.Bow:
                    newX = baseX + targetCoord.x;
                    newY = baseY - targetCoord.y;
                    break;
            }

            Vector2Int newCoord = new Vector2Int(newX, newY);

            return newCoord;
        }

        public List<Targetable> SortTargetables()
        {
            CompileAllTargetables(_ability);

            switch (_ability.targetingType)
            {
                case TargetingType.allallies:
                    SelectTargetablesAllAllies(); break;
                case TargetingType.allfoes:
                    SelectTargetablesAllEnemies(); break;
                case TargetingType.beeline:
                    SelectTargetablesBeeline(_caster.GridPosition); break;
                case TargetingType.melee:
                    SelectTargetablesMelee(_caster.GridPosition); break;
                case TargetingType.volley:
                    SelectTargetablesVolley(); break;
                case TargetingType.cardinalAlly:
                    SelectTargetablesCardinalAlly(_caster.GridPosition); break;
                case TargetingType.cell:
                    SelectTargetablesCell(); break;
                case TargetingType.self:
                    SelectTargetablesSelf(_caster); break;
                case TargetingType.support:
                    SelectTargetablesSupport(); break;
                case TargetingType.supportNotSelf:
                    SelectTargetablesSupportNotSelf(_caster); break;
                case TargetingType.supportAdjacent:
                    SelectTargetablesSupportAdjacent(_caster.GridPosition); break;
                case TargetingType.random:
                    SelectTargetablesRandom(); break;
                case TargetingType.obstacle:
                    SelectTargetablesObstacles(); break;
                case TargetingType.gridGlobal:
                    GlobalGridTargeting(_ability.gridTargetingCoordinates);
                    break;
                case TargetingType.ordinal:
                    SelectTargetablesOrdinal(_caster.GridPosition);
                    break;
                case TargetingType.gridRelative:
                    RelativeGridTargeting(_ability.gridTargetingCoordinates, _caster, _caster.GetComponent<GridPosition>().GetWing());
                    break;
                default:
                    Debug.Log("invalid targeting type");
                    break;
            }

            return _selectedTargetables;
        }

        private void SelectTargetablesObstacles()
        {
            var _allObstacles = from Targetable t in _availableTargetables
                                where t.GetTargetClass() == TargetClass.Obstacle
                                select t;

            _selectedTargetables.AddRange(from t in _allObstacles
                                          select t);
        }

        private void CompileAllTargetables(Ability ability)
        {
            foreach (Targetable t in _allTargetables)
            {
                if (t.GetTargetClass() == TargetClass.Cell)
                {
                    _allCells.Add(t);
                    continue;
                }

                if (t.GetTargetClass() == TargetClass.Obstacle)
                {
                    _availableTargetables.Add(t);
                    continue;
                }

                if (t.GetComponent<StatusHandler>() == null) continue;
                if (t.GetComponent<StatsHandler>() == null) continue;
                if (!t.Stats.Alive && !ability.canRevive) continue;


                if (t.GetComponent<StatusHandler>().State != NeutralState.cloak)
                {
                    _availableTargetables.Add(t);
                }

                if (t.GetComponent<StatusHandler>().State == NeutralState.cloak && ability.neverMiss)
                {
                    _availableTargetables.Add(t);
                }
            }
        }
    }
}