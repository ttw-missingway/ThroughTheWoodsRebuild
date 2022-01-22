using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class AI : MonoBehaviour
    {

        [SerializeField] float initialCooldown = 5f;
        [SerializeField] float emergencyCooldown = 10f;
        [SerializeField] bool comboStarted = false;
        [SerializeField] int comboIndex = 0;
        [SerializeField] Deck currentDeck = null;
        [SerializeField] int deckIndex = 0;
        float healthRatio;

        [SerializeField] Ability selectedAbility;
        [SerializeField] List<Targetable> selectedTargets = new List<Targetable>();
        [SerializeField] List<Targetable> availableTargets = new List<Targetable>();
        [SerializeField] List<AbilityOption> abilityOptions = new List<AbilityOption>();
        [SerializeField] List<AbilityOption> removeList = new List<AbilityOption>();
        [SerializeField] List<AbilityOption> addList = new List<AbilityOption>();
        [SerializeField] List<AbilityOption> abilityLottery = new List<AbilityOption>();

        Cooldown cooldown;
        AbilitySlots abilitySlots;
        AttackExecutor attackExecutor;
        EnemyEntity enemyEntity;

        private void Start()
        {
            cooldown = GetComponent<Cooldown>();
            abilitySlots = GetComponent<AbilitySlots>();
            attackExecutor = GetComponent<AttackExecutor>();
            enemyEntity = GetComponent<EnemyEntity>();
            cooldown.SetCooldown(initialCooldown);

            cooldown.OnCooldownEnd += Cooldown_OnCooldownEnd;
            cooldown.OnChannelEnd += Cooldown_OnChannelEnd;
        }

        private void Cooldown_OnChannelEnd(object sender, EventArgs e)
        {
            //ClearLists();
        }

        private void Cooldown_OnCooldownEnd(object sender, System.EventArgs e)
        {
            SelectAbility();
        }

        private void SelectAbility()
        {
            print("selecting ability");
            AddAbilitiesToOptions();
            CheckIfTargetsAvailable();
            FilterForAbilityRelevance();
            CheckAbilitySpecificConditions();
            AddRemoveTargetBasedOptions();

            if (CheckForNoTargetables()) return;

            WeighAbilities();
            CreateLottery();
            AssignAbilityAndTargets();
            PerformAbility();
            AIWaitTime();
        }

        private bool CheckForNoTargetables()
        {
            bool noTargets = true;

            foreach(AbilityOption o in abilityOptions)
            {
                if (o.MultipleTargets)
                {
                    noTargets = false;
                }

                if (o.GetAbility.usesRails)
                {
                    noTargets = false;
                }

                foreach(Targetable t in o.GetTargets)
                {
                    noTargets = false;
                }
            }

            if (noTargets)
            {
                print("Emergency Cooldown");
                cooldown.SetCooldown(emergencyCooldown);
            }

            return noTargets;
        }

        private void FilterForAbilityRelevance()
        {
            foreach (AbilityOption o in abilityOptions)
            {
                if (o.GetAbility.usesRails) continue;
                if (o.MultipleTargets) continue;

                if (o.GetAbility.damageType == DamageType.healing)
                {
                    CheckHealingRelevance(o);
                }

                if (o.GetAbility.support)
                {
                    FilterForBossSupport(o);
                }

                if (o.GetAbility.damageType == DamageType.physical ||
                    o.GetAbility.damageType == DamageType.magical)
                {
                    PrioritizeLowHealthTargets(o);
                }

                if (o.GetTargets.Count == 0)
                {
                    removeList.Add(o);
                }
            }
        }

        private static void PrioritizeLowHealthTargets(AbilityOption o)
        {
            if (o.GetTargets.Count == 0) return;

            List<Targetable> removeT = new List<Targetable>();
            Targetable lowestHealthTarget = o.GetTargets[0];
            float lowestHealthRatio = lowestHealthTarget.Stats.Health / lowestHealthTarget.Stats.MaxHealth;
            float currentHealthRatio = 0f;

            foreach (Targetable t in o.GetTargets)
            {
                currentHealthRatio = t.Stats.Health / t.Stats.MaxHealth;

                if (currentHealthRatio < lowestHealthRatio)
                {
                    lowestHealthRatio = currentHealthRatio;
                    lowestHealthTarget = t;
                }
            }

            foreach (Targetable t in o.GetTargets)
            {
                if (t != lowestHealthTarget)
                {
                    removeT.Add(t);
                }
            }

            foreach (Targetable t in removeT)
            {
                o.GetTargets.Remove(t);
            }
        }

        private static void FilterForBossSupport(AbilityOption o)
        {
            bool foundBoss = false;
            List<Targetable> removeT = new List<Targetable>();

            foreach (Targetable t in o.GetTargets)
            {
                if (t.GetBossEntity() != null)
                {
                    foundBoss = true;
                }
            }

            if (foundBoss)
            {
                foreach (Targetable t in o.GetTargets)
                {
                    if (t.GetBossEntity() == null)
                    {
                        removeT.Add(t);
                    }
                }
            }

            foreach (Targetable t in removeT)
            {
                o.GetTargets.Remove(t);
            }
        }

        private static void CheckHealingRelevance(AbilityOption o)
        {
            if (!o.MultipleTargets)
            {
                List<Targetable> removeT = (from Targetable t in o.GetTargets
                                            where t.Stats.Health == t.Stats.MaxHealth
                                            select t).ToList();

                foreach (Targetable t in removeT)
                {
                    o.GetTargets.Remove(t);
                }
            }
        }

        private void AddAbilitiesToOptions()
        {
            if (comboStarted)
            {
                print("following combo starter");
                abilityOptions.Clear();
                AbilityOption combo = new AbilityOption(currentDeck.combo[comboIndex], comboIndex);
                abilityOptions.Add(combo);
                return;
            }

            if (!abilitySlots.usesDeck)
                AddAbilitiesNoDeck();
            else if (abilitySlots.usesDeck && currentDeck == null)
            {
                SwitchToNewDeck();
                AddAbilitiesFromCurrentDeck();
            }
            else
            {
                if (CheckHealthThreshold())
                {
                    AddAbilitiesFromCurrentDeck();
                } 
                else
                {
                    deckIndex++;
                    SwitchToNewDeck();
                    AddAbilitiesFromCurrentDeck();
                }    
            }
        }

        private bool CheckHealthThreshold()
        {
            var health = GetComponent<StatsHandler>().Health;
            var maxHealth = GetComponent<StatsHandler>().MaxHealth;
            healthRatio = (health / maxHealth);

            if (healthRatio <= currentDeck.healthPercentageThreshold)
            {
                return false;
            }  
            else
            {
                return true;
            }
        }

        private void AddAbilitiesFromCurrentDeck()
        {
            for (int i = 0; i < currentDeck.abilities.Length; i++)
            {
                AbilityOption option = new AbilityOption(currentDeck.abilities[i], i);
                abilityOptions.Add(option);
            }
        }

        private void SwitchToNewDeck()
        {
            currentDeck = abilitySlots.GetDeck(deckIndex);
        }

        private void AddAbilitiesNoDeck()
        {
            for (int i = 0; i < abilitySlots.GetAbilities().Count; i++)
            {
                AbilityOption option = new AbilityOption(abilitySlots.GetAbilities()[i], i);
                abilityOptions.Add(option);
            }
        }

        private void CheckIfTargetsAvailable()
        {
            foreach(AbilityOption o in abilityOptions)
            {
                Targetable caster = GetComponent<Targetable>();
                var allTargetables = FindObjectsOfType<Targetable>();
                TargetingTool tTool = new TargetingTool(allTargetables.ToList(), o.GetAbility, caster, caster.GetTargetClass());

                availableTargets.Clear();
                availableTargets = tTool.SortTargetables();

                if (o.GetAbility.targetingType == TargetingType.allallies ||
                    o.GetAbility.targetingType == TargetingType.allfoes ||
                    o.GetAbility.targetingType == TargetingType.gridRelative ||
                    o.GetAbility.targetingType == TargetingType.gridGlobal)
                {
                    o.MultipleTargets = true;
                }

                foreach (Targetable t in availableTargets)
                {
                    o.AddTarget(t);
                }

                if (o.GetTargets.Count() == 0)
                {
                    if (o.GetAbility.targetingType == TargetingType.gridRelative ||
                        o.GetAbility.targetingType == TargetingType.gridGlobal ||
                        o.GetAbility.usesRails)
                    {
                        continue;
                    }
                    else
                    {
                        removeList.Add(o);
                    }
                }

                FilterForEnemyCreation(o);
            }
        }

        private void FilterForEnemyCreation(AbilityOption o)
        {
            if (o.GetAbility.enemyEntity.Length > 0)
            {
                EnemyEntity[] enemyEntities = FindObjectsOfType<EnemyEntity>();
                int relevantEnemiesCount = 0;

                foreach (EnemyEntity e in enemyEntities)
                {
                    for (int i = 0; i < o.GetAbility.enemyEntity.Length; i++)
                    {
                        if (e.enemyType == o.GetAbility.enemyEntity[i].enemyType)
                        {
                            relevantEnemiesCount++;
                        }
                    }
                }

                if (relevantEnemiesCount >= o.GetAbility.entityMax)
                {
                    removeList.Add(o);
                }
            }
        }

        private void AIWaitTime()
        {
            if (selectedAbility.aiWaitTime > 0f)
                GetComponent<Cooldown>().AddToCooldown(selectedAbility.aiWaitTime);
        }

        private void CheckAbilitySpecificConditions()
        {
            BattleGrid battleGrid = BattleGrid.singleton;

            foreach (AbilityOption o in abilityOptions)
            {
                if (!o.GetAbility.usesRails) continue; //continue

                if (!o.GetAbility.relative)
                {
                    if (battleGrid.allCells[o.GetAbility.endingCell].IsOccupied)
                        removeList.Add(o);
                }
                else
                {
                    if (GetComponent<GridPosition>().GetWing() == Wing.Bow) return;

                    if (battleGrid.allCells[TranslateCoordinates(o.GetAbility.endingCell, GetComponent<GridPosition>().GetWing())].IsOccupied)
                        removeList.Add(o);
                }
            }
        }

        private void ClearLists()
        {
            selectedTargets.Clear();
            availableTargets.Clear();
            abilityOptions.Clear();
            removeList.Clear();
            addList.Clear();
            abilityLottery.Clear();
            print("Clearing Lottery");
        }

        private void CreateLottery()
        {
            foreach(AbilityOption o in abilityOptions)
            {
                for (int i = 0; i < o.GetWeight; i++)
                {
                    print("adding " + o.GetAbility.name + " to lottery");
                    abilityLottery.Add(o);
                }
            }
        }

        private void AddRemoveTargetBasedOptions()
        {
            foreach (AbilityOption o in removeList)
            {
                abilityOptions.Remove(o);
            }
        }

        private void PerformAbility()
        {
            AttackPacket attackPacket = new AttackPacket(selectedAbility, attackExecutor, selectedTargets);

            GetComponent<Cooldown>().StartAbilityChannel(attackPacket, selectedAbility.attackChannelTime);

            ClearLists();
        }

        private void AssignAbilityAndTargets()
        {
            int randomIndex = UnityEngine.Random.Range(0, abilityLottery.Count);
            var selectedOption = abilityLottery[randomIndex];

            selectedAbility = selectedOption.GetAbility;
            print("selected Ability: " + selectedAbility.name);

            if (abilitySlots.usesDeck)
            {
                Combo();
            }

            if (selectedOption.MultipleTargets)
            {
                foreach (Targetable t in abilityLottery[randomIndex].GetTargets)
                {
                    selectedTargets.Add(t);
                }
            }
            else
            {
                int randomTarget = UnityEngine.Random.Range(0, abilityLottery[randomIndex].GetTargets.Count);

                selectedTargets.Add(abilityLottery[randomIndex].GetTargets[randomTarget]);
            }
        }

        private void Combo()
        {
            if (currentDeck.combo.Length == 0) return;

            if (currentDeck.combo[0] == selectedAbility)
            {
                comboStarted = true;
            }

            if (comboStarted)
            {
                if (comboIndex == currentDeck.combo.Length - 1)
                {
                    comboIndex = 0;
                    comboStarted = false;
                }
                else
                {
                    comboIndex++;
                }
            }
        }

        private void WeighAbilities()
        {
            if (abilitySlots.usesDeck)
            {
                foreach (AbilityOption a in abilityOptions)
                {
                    a.AddWeight(currentDeck.entries[a.GetIndex]); //found error involving index of array, happened first after changing aspects of wolfdash
                }
            }
            else
            {
                foreach (AbilityOption a in abilityOptions)
                {
                    a.AddWeight(1);
                }
            }
            
        }

        public Vector2Int TranslateCoordinates(Vector2Int coord, Wing wing)
        {
            int baseX = enemyEntity.GridPosition.x;
            int baseY = enemyEntity.GridPosition.y;
            int newX = coord.x;
            int newY = coord.y;

            switch (wing)
            {
                case Wing.Port:
                    newX = coord.y;
                    newY = baseY + coord.x;
                    break;
                case Wing.Starboard:
                    newX = baseX - coord.y;
                    newY = baseY - coord.x;
                    break;
                case Wing.Bow:
                    newX = baseX + coord.x;
                    newY = baseY - coord.y;
                    break;
            }

            Vector2Int newCoord = new Vector2Int(newX, newY);

            return newCoord;
        }

        private class AbilityOption
        {
            readonly Ability _ability;
            int _index;
            int _weight;
            List<Targetable> _targets = new List<Targetable>();

            public Ability GetAbility => _ability;
            public int GetWeight => _weight;
            public int GetIndex => _index;
            public List<Targetable> GetTargets => _targets;
            public bool MultipleTargets = false;

            public AbilityOption(Ability ability, int index)
            {
                _ability = ability;
                _index = index;
            }

            public void SetWeight(int weight)
            {
                _weight = weight;
            }

            public void AddWeight(int weight)
            {
                _weight += weight;
            }

            public void AddTarget(Targetable target)
            {
                _targets.Add(target);
            }
        }
    }
}
