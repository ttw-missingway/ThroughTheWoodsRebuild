using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class Cooldown : MonoBehaviour
    {
        [SerializeField] float _cooldown = 0f;
        [SerializeField] float _channel = 0f;
        [SerializeField] bool _onCooldown = false;
        [SerializeField] bool _isChanneling = false;
        [SerializeField] bool _animationFreeze = false;
        [SerializeField] bool _menuFreeze = false;
        [SerializeField] CooldownTimer _uiTimer;
        ActorSelection actorSelection;
        IEnumerator cooldownTimer;
        AnimationController animationController;
        StatModifierTool statModifierTool;
        StatsHandler statsHandler;


        public bool IsOnCooldown => _onCooldown;
        public bool IsChanneling => _isChanneling;

        public event EventHandler OnCooldownEnd;
        public event EventHandler OnChannelEnd;

        private void Awake()
        {
            statModifierTool = new StatModifierTool(FindObjectOfType<PublicValues>());
            cooldownTimer = CooldownTimer();
            actorSelection = FindObjectOfType<ActorSelection>();
            statsHandler = GetComponent<StatsHandler>();
        }

        private void Start()
        {
            animationController = AnimationController.singleton;
            animationController.OnAnimationFreeze += Animation_OnAnimationFreeze;
            animationController.OnAnimationFreezeEnd += Animation_OnAnimationFreezeEnd;
            animationController.OnMenuFreeze += Animation_OnMenuFreeze;
            animationController.OnMenuFreezeEnd += Animation_OnMenuFreezeEnd;
            statsHandler.OnDeath += EndTimers;
        }

        private void EndTimers(object sender, EventArgs e)
        {
            StopAllCoroutines();
            _isChanneling = false;
            _channel = 0f;
            _onCooldown = false;
            _cooldown = 0f;
        }

        private void Animation_OnAnimationFreezeEnd(object sender, EventArgs e)
        {
            _animationFreeze = false;
        }

        private void Animation_OnAnimationFreeze(object sender, EventArgs e)
        {
            _animationFreeze = true;
        }

        private void Animation_OnMenuFreezeEnd(object sender, EventArgs e)
        {
            _menuFreeze = false;
        }

        private void Animation_OnMenuFreeze(object sender, EventArgs e)
        {
            _menuFreeze = true;
        }

        public void SetCooldown(float cooldown)
        {
            cooldownTimer = CooldownTimer();
            _cooldown = cooldown;
            StartCoroutine(cooldownTimer);
        }

        public void StartAbilityChannel(AttackPacket attackPacket, float channelTime)
        {
            _channel = channelTime;

            if (channelTime > 0)
            {
                GetComponent<ICombatAnimation>().ChangeAnimationState(CombatAnimStates.Channel, DirectionTypes.None);
                if (GetComponent<BossEntity>() != null)
                {
                    GetComponent<BossEntity>().BossChanneling();
                }
                if (GetComponent<EnemyEntity>() != null)
                {
                    GetComponent<EnemyEntity>().EnemyChanneling();
                }
            }
            
            StartCoroutine(ChannelTimer(attackPacket));
        }

        public void AddToCooldown(float cooldown)
        {
            _cooldown += cooldown;

            ChannelBreak();

            if (!_onCooldown)
            {
                StartCoroutine(cooldownTimer);
            }
        }

        public void ChannelBreak()
        {
            if (_isChanneling)
            {
                StopAllCoroutines();
                _isChanneling = false;
                _channel = 0f;
            }
        }

        public void Refresh()
        {
            StopCoroutine(cooldownTimer);
            _cooldown = 0f;
            _onCooldown = false;
            actorSelection.OffCDAlert();
            _uiTimer?.UIUpdate(_cooldown, 1f);
        }

        public void AssignUITimer(CooldownTimer cd)
        {
            _uiTimer = cd;
        }

        public void RemoveUITimer()
        {
            _uiTimer = null;
        }

        private IEnumerator ChannelTimer(AttackPacket attackPacket)
        {
            StatsHandler stats = attackPacket.Caster.GetComponent<StatsHandler>();

            if (GetComponent<ActorEntity>() != null)
            {
                GetComponent<ActorEntity>().ActorChanneling();
            }

            while (_channel > 0f)
            {
                _isChanneling = true;

                yield return new WaitForSeconds(statModifierTool.ChannelAdjustment(stats));

                if (!_animationFreeze && !_menuFreeze)
                    _channel -= 1f;
            }

            if (GetComponent<ActorEntity>() != null)
            {
                GetComponent<ActorEntity>().ResetHighlights();
            }

            _isChanneling = false;

            attackPacket.Caster.GetComponent<AttackExecutor>().ReceiveAttackPacket(attackPacket);
            OnChannelEnd?.Invoke(this, EventArgs.Empty);
        }
        

        private IEnumerator CooldownTimer()
        {
            AttackExecutor executor = GetComponent<AttackExecutor>();
            StatsHandler stats = GetComponent<StatsHandler>();
            float _maxCD = _cooldown;

            //if (GetComponent<ActorEntity>() != null)
            //{
            //    GetComponent<ActorEntity>().DitherActor();
            //}

            while (_cooldown > 0f)
            {
                if (_cooldown > _maxCD)
                {
                    _maxCD = _cooldown;
                }

                _onCooldown = true;
                yield return new WaitForSeconds(statModifierTool.CooldownAdjustment(stats));
                if (!_animationFreeze && !_menuFreeze)
                {
                    _cooldown -= 1f;
                    _uiTimer?.UIUpdate(_cooldown, _maxCD);
                }    
            }

            //if (GetComponent<ActorEntity>() != null)
            //{
            //    GetComponent<ActorEntity>().ResetHighlights();
            //}

            _onCooldown = false;
            actorSelection.OffCDAlert();
            OnCooldownEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}
