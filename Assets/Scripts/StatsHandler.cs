using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTW.Persistent;

namespace TTW.Combat
{
    public enum Stats
    {
        None,
        Heart,
        Gait,
        Grit,
        Spirit,
        Mind
    }

    public class StatsHandler : MonoBehaviour
    {
        ActorType actor;
        StatModifierTool statModifierTool;
        SharedDamage sharedDamage;
        GameOverListener gameOver;
        [SerializeField] bool alive = true;
        [SerializeField] float health = 0f;
        [SerializeField] float baseHealth = 0f;
        [SerializeField] float maxHealth = 0f;

        [SerializeField] float heart = 0f;
        [SerializeField] [Range(-3, 3)] int heartModifier = 0;

        [SerializeField] float gait = 0f;
        [SerializeField] [Range(-3, 3)] int gaitModifier = 0;

        [SerializeField] float grit = 0f;
        [SerializeField] [Range(-3, 3)] int gritModifier = 0;

        [SerializeField] float spirit = 0f;
        [SerializeField] [Range(-3, 3)] int spiritModifier = 0;

        [SerializeField] float mind = 0f;
        [SerializeField] [Range(-3, 3)] int mindModifier = 0;

        PublicAbilities publicAbilities;

        VFX lightDamageFX;
        VFX normalDamageFX;
        VFX heavyDamageFX;

        private float BossHeavyDamageThreshold = 0.1f;
        private float BossNormalDamageThreshold = 0.05f;
        private float ActorHeavyDamageThreshold = 0.3f;


        public bool Alive => alive;
        public float Health => health;
        public float BaseHealth => baseHealth;
        public float MaxHealth => maxHealth;
        public float Heart => ModifierAlgorithm(heart, heartModifier);
        public float Gait => ModifierAlgorithm(gait, gaitModifier);
        public float Grit => ModifierAlgorithm(grit, gritModifier);
        public float Spirit => ModifierAlgorithm(spirit, spiritModifier);
        public float Mind => ModifierAlgorithm(mind,  mindModifier);

        public int ModHeart => heartModifier;
        public int ModGait => gaitModifier;
        public int ModGrit => gritModifier;
        public int ModSpirit => spiritModifier;
        public int ModMind => mindModifier;

        public event EventHandler OnRebirth;
        public event EventHandler OnDeath;
        public event EventHandler OnDamageTaken;

        ICombatAnimation animManager;

        private void Awake()
        {
            statModifierTool = new StatModifierTool(FindObjectOfType<PublicValues>());
            if (GetComponent<SharedDamage>() != null)
                sharedDamage = GetComponent<SharedDamage>();
            animManager = GetComponent<ICombatAnimation>();
        }

        private void Start()
        {
            gameOver = GameOverListener.singleton;
            publicAbilities = PublicAbilities.singleton;
        }

        public void PullStats(bool initial)
        {
            PartyManager partyManager = FindObjectOfType<PartyManager>();

            baseHealth = partyManager.GetBaseHealth(actor);
            heart = partyManager.GetStat(actor, Stats.Heart);
            gait = partyManager.GetStat(actor, Stats.Gait);
            grit = partyManager.GetStat(actor, Stats.Grit);
            spirit = partyManager.GetStat(actor, Stats.Spirit);
            mind = partyManager.GetStat(actor, Stats.Mind);

            heartModifier = partyManager.GetStatModifier(actor, Stats.Heart);
            gaitModifier = partyManager.GetStatModifier(actor, Stats.Gait);
            gritModifier = partyManager.GetStatModifier(actor, Stats.Grit);
            spiritModifier = partyManager.GetStatModifier(actor, Stats.Spirit);
            mindModifier = partyManager.GetStatModifier(actor, Stats.Mind);

            maxHealth = statModifierTool.HealthAdjustment(this) + baseHealth;

            if (initial)
            {
                health = maxHealth;
            }
            else
            {
                health = partyManager.GetCurrentHealth(actor);
            }
            
        }

        public void PushStats()
        {
            PartyManager partyManager = FindObjectOfType<PartyManager>();

            partyManager.SaveModStats(actor, this);
            partyManager.SaveCurrentHealth(actor, health);
        }

        public void SetActor(ActorType actor)
        {
            this.actor = actor;
        }

        public int TotalStatSum()
        {
            return (heartModifier + gaitModifier + gritModifier + spiritModifier + mindModifier);
        }

        private float ModifierAlgorithm(float stat, int statMod)
        {
            return stat + (stat * (statMod * 0.5f));
        }

        public void ModifyStat(Stats stat, int amount)
        {
            if (CheckForBubble()) return;

            if (sharedDamage != null)
            {
                sharedDamage.SendBuffsToCore(stat, amount);
                return;
            }

            switch (stat)
            {
                case Stats.Gait:
                    gaitModifier += amount;
                    gaitModifier = Mathf.Clamp(gaitModifier, -3, 3);
                    break;
                case Stats.Grit:
                    gritModifier += amount;
                    gritModifier = Mathf.Clamp(gritModifier, -3, 3);
                    break;
                case Stats.Heart:
                    heartModifier += amount;
                    heartModifier = Mathf.Clamp(heartModifier, -3, 3);
                    HeartChangeHealth();
                    break;
                case Stats.Mind:
                    mindModifier += amount;
                    mindModifier = Mathf.Clamp(mindModifier, -3, 3);
                    break;
                case Stats.Spirit:
                    spiritModifier += amount;
                    spiritModifier = Mathf.Clamp(spiritModifier, -3, 3);
                    break;
            }
        }

        private bool CheckForBubble()
        {
            if (GetComponent<StatusHandler>() == null) return false;
            if (GetComponent<StatusHandler>().Bubble) return true;

            return false;
        }

        private void HeartChangeHealth()
        {
            var newMaxHealth = statModifierTool.HealthAdjustment(this);
            print("new max health: " + newMaxHealth);
            var healthDiff = newMaxHealth - maxHealth;
            print("health diff: " + healthDiff);
            health += healthDiff;
            maxHealth = newMaxHealth + baseHealth;
        }

        public void Dispel()
        {
            heartModifier = 0;
            mindModifier = 0;
            spiritModifier = 0;
            gaitModifier = 0;
            gritModifier = 0;
        }

        public void Revive(float amount)
        {
            if (alive == true) return;

            alive = true;
            ChangeHealth(amount);

            animManager.ChangeAnimationState(CombatAnimStates.Idle, DirectionTypes.Down);
        }

        public void ChangeHealth(float change)
        {
            if (alive == false) return;
            if (sharedDamage != null)
            {
                sharedDamage.SendDamageToCore(change);
                return;
            }

            DamageAnimation(change);

            health += change;
            OnDamageTaken?.Invoke(this, EventArgs.Empty);

            if (health > maxHealth) health = maxHealth;

            if (health < 0)
            {
                health = 0;
                Death();
            }

            if (GetComponent<StatusHandler>() == null) return;

            if (GetComponent<StatusHandler>().Rebirth)
            {
                Rebirth();
            }
        }

        private void DamageAnimation(float change)
        {
            float heavyThresh = 0f;
            float normalThresh = 0f;
            lightDamageFX = publicAbilities.lightFX;
            normalDamageFX = publicAbilities.normalFX;
            heavyDamageFX = publicAbilities.heavyFX;

            if (GetComponent<BossEntity>() != null)
            {
                heavyThresh = BossHeavyDamageThreshold;
                normalThresh = BossNormalDamageThreshold;
            }

            if (GetComponent<ActorEntity>() != null)
            {
                heavyThresh = ActorHeavyDamageThreshold;
                normalThresh = ActorHeavyDamageThreshold;
            }

            if (change < 0)
            {
                if (Mathf.Abs(change) > maxHealth * heavyThresh)
                {
                    animManager.ChangeAnimationState(CombatAnimStates.HeavyDamage, DirectionTypes.None);
                    Instantiate(heavyDamageFX, transform.position, Quaternion.identity);
                }
                else if (Mathf.Abs(change) > maxHealth * normalThresh)
                {
                    animManager.ChangeAnimationState(CombatAnimStates.NormalDamage, DirectionTypes.None);
                    Instantiate(normalDamageFX, transform.position, Quaternion.identity);
                }
                else
                {
                    animManager.ChangeAnimationState(CombatAnimStates.LightDamage, DirectionTypes.None);
                    Instantiate(lightDamageFX, transform.position, Quaternion.identity);
                }
            }
        }

        public void Rebirth()
        {
            if (!alive)
            {
                health = maxHealth * 0.25f;
                alive = true;
                OnRebirth?.Invoke(this, EventArgs.Empty);

                if (GetComponent<ActorEntity>() != null)
                {
                    GetComponent<ActorEntity>().ResetHighlights();
                }

                animManager.ChangeAnimationState(CombatAnimStates.Idle, DirectionTypes.Down);
            }
        }

        public void Death()
        {
            alive = false;
            OnDeath?.Invoke(this, EventArgs.Empty);

            gameOver.GameOverCheck();

            if (GetComponent<ActorEntity>() != null)
            {
                GetComponent<Cooldown>().SetCooldown(0f);
                GetComponent<ActorEntity>().DeadActor();
            }

            if (GetComponent<EnemyEntity>() != null)
            {
                gameObject.SetActive(false);
            }

            //if (GetComponent<BossEntity>() != null)
            //{
            //    Destroy(gameObject);
            //}
        }
    }
}