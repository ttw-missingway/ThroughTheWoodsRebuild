using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTW.Combat;
using System;
using System.Linq;

namespace TTW.Persistent
{
    public class PartyManager : MonoBehaviour
    {
        [SerializeField] List<Actor> allActors = new List<Actor>();
        [SerializeField] List<ActorEntity> actorsInParty = new List<ActorEntity>();
        [SerializeField] List<ActorEntity> actorsOnBench = new List<ActorEntity>();
        [SerializeField] List<Vector2Int> spawnPoints = new List<Vector2Int>();
        [SerializeField] List<ActorStats> actorStats = new List<ActorStats>();

        [SerializeField] Dictionary<ActorEntity, Vector2Int> actorsToSpawnPoints = new Dictionary<ActorEntity, Vector2Int>();

        BattleGrid battleGrid;
        CombatUnloader unloader;

        private void Start()
        {
            CreateActorStats();
            //this will be changed to a method that is called on scene load for combat
            unloader = FindObjectOfType<CombatUnloader>();
            battleGrid = BattleGrid.singleton;
            PopulateSpawnPointDictionary();
            PopulateUnloader();
            SpawnActors();
        }

        private void PopulateUnloader()
        {
            foreach(ActorEntity a in actorsInParty)
                unloader.AddPartyMember(a);

            foreach (ActorEntity a in actorsOnBench)
                unloader.AddBenchMember(a);
        }

        public List<ActorEntity> GetActiveParty() => actorsInParty;
        public List<ActorEntity> GetBenchParty() => actorsOnBench;

        public void SpawnActors()
        {
            foreach (ActorEntity actor in actorsInParty)
            {
                Cell spawnCell = battleGrid.playerCells[actorsToSpawnPoints[actor]];
                unloader.ManifestActor(actor, spawnCell, initial: true);
            }

            WakeSelectionController();
        }

        public void SaveStatusSnapshot(ActorType actor, List<StatusSnapShot> snapShots)
        {
            foreach (var a in from ActorStats a in actorStats
                              where a.Actor == actor
                              select a)
            {
                a.ClearStatusEffects();

                foreach(StatusSnapShot s in snapShots)
                {
                    a.AddStatusEffect(s);
                }
            }
        }

        public List<StatusSnapShot> LoadStatusSnapshot(ActorType actor)
        {
            foreach (var a in from ActorStats a in actorStats
                              where a.Actor == actor
                              select a)
            {
                if (a.GetActiveStatusEffects() != null)
                {
                    return a.GetActiveStatusEffects();
                }
            }

            return null;
        }

        private void CreateActorStats()
        {
            foreach(Actor a in allActors)
            {
                var actor = new ActorStats(a.actorType, a.baseHealth, a.Mind, a.Heart, a.Spirit, a.Gait, a.Grit);
                actorStats.Add(actor);
            }
        }

        public int GetStat(ActorType actor, Stats stat)
        {

            int statAmount = 0;
            foreach (var a in from ActorStats a in actorStats
                              where a.Actor == actor
                              select a)
            {
                statAmount = a.TotalStat(stat);
            }

            return statAmount;
        }

        public int GetStatModifier(ActorType actor, Stats stat)
        {

            int statAmount = 0;
            foreach (var a in from ActorStats a in actorStats
                              where a.Actor == actor
                              select a)
            {
                statAmount = a.GetModStat(stat);
            }

            return statAmount;
        }

        public float GetCurrentHealth(ActorType actor)
        {
            float health = 0;
            foreach (var a in from ActorStats a in actorStats
                              where a.Actor == actor
                              select a)
            {
                health = a.Health;
            }

            return health;
        }

        public float GetBaseHealth(ActorType actor)
        {
            float health = 0;
            foreach (var a in from ActorStats a in actorStats
                              where a.Actor == actor
                              select a)
            {
                health = a.BaseHealth;
            }

            return health;
        }

        public void SaveCurrentHealth(ActorType actor, float health)
        {
            foreach (var a in from ActorStats a in actorStats
                              where a.Actor == actor
                              select a)
            {
                a.UpdateHealth(health);
            }
        }

        public void SaveModStats(ActorType actor, StatsHandler stats)
        {
            foreach (var a in from ActorStats a in actorStats
                              where a.Actor == actor
                              select a)
            {
                a.ModStat(Stats.Gait, stats.ModGait);
                a.ModStat(Stats.Grit, stats.ModGrit);
                a.ModStat(Stats.Heart, stats.ModHeart);
                a.ModStat(Stats.Mind, stats.ModMind);
                a.ModStat(Stats.Spirit, stats.ModSpirit);
            }
        }

        private void WakeSelectionController()
        {
            var selectionController = FindObjectOfType<SelectionController>();

            selectionController.Initialize();
        }

        private void PopulateSpawnPointDictionary()
        {
            for (int i = 0; i < actorsInParty.Count; i++)
            {
                actorsToSpawnPoints.Add(actorsInParty[i], spawnPoints[i]);
            }
        }

        private class ActorStats
        {
            ActorType _actor;
            List<StatusSnapShot> statusEffects = new List<StatusSnapShot>();

            float _baseHealth = 0;
            int _baseMind = 0;
            int _baseHeart = 0;
            int _baseSpirit = 0;
            int _baseGait = 0;
            int _baseGrit = 0;

            int _permMind = 0;
            int _permHeart = 0;
            int _permSpirit = 0;
            int _permGait = 0;
            int _permGrit = 0;

            int _tempMind = 0;
            int _tempHeart = 0;
            int _tempSpirit = 0;
            int _tempGait = 0;
            int _tempGrit = 0;

            int _modMind = 0;
            int _modHeart = 0;
            int _modSpirit = 0;
            int _modGait = 0;
            int _modGrit = 0;

            float _currentHealth = 0;

            public ActorStats(ActorType actor, float baseHealth, int mind, int heart, int spirit, int gait, int grit)
            {
                PublicValues publicValues = FindObjectOfType<PublicValues>();

                _actor = actor;
                _baseHealth = baseHealth;
                _baseMind = mind;
                _baseHeart = heart;
                _baseSpirit = spirit;
                _baseGait = gait;
                _baseGrit = grit;
                _currentHealth = baseHealth + Mathf.Lerp(publicValues.heartForHealthMin, publicValues.heartForHealthMax, _baseHeart / 100f);
            }

            public void AddStatusEffect(StatusSnapShot statusEffect)
            {
                statusEffects.Add(statusEffect);
            }

            public void ClearStatusEffects()
            {
                statusEffects.Clear();
            }

            public List<StatusSnapShot> GetActiveStatusEffects()
            {
                return statusEffects;
            }

            public void PermanentStat(Stats stat, int amount)
            {
                switch (stat)
                {
                    case Stats.Mind:
                        _permMind += amount;
                        break;
                    case Stats.Gait:
                        _permGait += amount;
                        break;
                    case Stats.Grit:
                        _permGrit += amount;
                        break;
                    case Stats.Heart:
                        _permHeart += amount;
                        break;
                    case Stats.Spirit:
                        _permSpirit += amount;
                        break;
                }
            }

            public void TemporaryStat(Stats stat, int amount)
            {
                switch (stat)
                {
                    case Stats.Mind:
                        _tempMind = amount;
                        break;
                    case Stats.Gait:
                        _tempGait = amount;
                        break;
                    case Stats.Grit:
                        _tempGrit = amount;
                        break;
                    case Stats.Heart:
                        _tempHeart = amount;
                        break;
                    case Stats.Spirit:
                        _tempSpirit = amount;
                        break;
                }
            }

            public void ModStat(Stats stat, int amount)
            {
                switch (stat)
                {
                    case Stats.Mind:
                        _modMind = amount;
                        break;
                    case Stats.Gait:
                        _modGait = amount;
                        break;
                    case Stats.Grit:
                        _modGrit = amount;
                        break;
                    case Stats.Heart:
                        _modHeart = amount;
                        break;
                    case Stats.Spirit:
                        _modSpirit = amount;
                        break;
                }
            }

            public int TotalStat(Stats stat)
            {
                switch (stat)
                {
                    case Stats.Mind:
                        return _baseMind + _permMind + _tempMind;
                    case Stats.Gait:
                        return _baseGait + _permGait + _tempGait;
                    case Stats.Grit:
                        return _baseGrit + _permGrit + _tempGrit;
                    case Stats.Heart:
                        return _baseHeart + _permHeart + _tempHeart;
                    case Stats.Spirit:
                        return _baseSpirit + _permSpirit + _tempSpirit;
                }

                return 0;
            }

            public int GetModStat(Stats stat)
            {
                switch (stat)
                {
                    case Stats.Mind:
                        return _modMind;
                    case Stats.Gait:
                        return _modGait;
                    case Stats.Grit:
                        return _modGrit;
                    case Stats.Heart:
                        return _modHeart;
                    case Stats.Spirit:
                        return _modSpirit;
                }

                return 0;
            }

            public void ClearModStats()
            {
                _modGait = 0;
                _modGrit = 0;
                _modHeart = 0;
                _modMind = 0;
                _modSpirit = 0;
                _currentHealth = 0;
            }

            public void ClearTempStats()
            {
                _tempGait = 0;
                _tempGrit = 0;
                _tempHeart = 0;
                _tempMind = 0;
                _tempSpirit = 0;
            }

            public void UpdateHealth(float health)
            {
                _currentHealth = health;
            }

            public float BaseHealth => _baseHealth;
            public float Health => _currentHealth;
            public ActorType Actor => _actor;
        }
    }
}