using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace TTW.Combat
{
    public class BossCombatAnimation : MonoBehaviour, ICombatAnimation
    {
        private Animator animator;
        private StatsHandler bossStats;
        private bool playedDeathAnim = false;
        private Cooldown cd;
        [SerializeField] TMP_Text _winText;

        private string currentStateString;

        private void Start()
        {

            currentStateString = " ";
            animator = GetComponent<Animator>();
            bossStats = GetComponent<StatsHandler>();
            cd = GetComponent<Cooldown>();
        }

        public void ChangeAnimationState(CombatAnimStates newState, DirectionTypes facing)
        {
            string stateString = currentStateString;

            switch (newState)
            {
                case CombatAnimStates.Idle:
                    if (bossStats.Health >= bossStats.MaxHealth / 3)
                    {
                        stateString = "Boss Idle";
                    }
                    else if (bossStats.Health < bossStats.MaxHealth / 3 && bossStats.Health > 0)
                    {
                        stateString = "Boss Low";
                    }
                    else
                    {
                        stateString = "Boss Dead";
                    }
                    break;

                case CombatAnimStates.Channel:
                    stateString = "Boss Channel";
                    break;

                case CombatAnimStates.Attack:
                    stateString = "Boss Attack";
                    break;

                case CombatAnimStates.Special1:
                    stateString = "Boss Special1";
                    break;

                case CombatAnimStates.Special2:
                    stateString = "Boss Special2";
                    break;

                case CombatAnimStates.Guard:
                    stateString = "Boss Guard";
                    break;

                case CombatAnimStates.LightDamage:
                    stateString = "Boss Damage Light";
                    break;

                case CombatAnimStates.HeavyDamage:
                    stateString = "Boss Damage Heavy";
                    break;

                case CombatAnimStates.NormalDamage:
                    stateString = "Boss Damage";
                    break;
            }

            if (currentStateString == stateString) return;

            animator.Play(stateString);
        }

        public void EndAttackAnimation()
        {
            if (!bossStats.Alive)
            {
                if (playedDeathAnim)
                {
                    gameObject.SetActive(false);
                    _winText.enabled = true;
                }
                else
                {
                    playedDeathAnim = true;
                    ChangeAnimationState(CombatAnimStates.Idle, DirectionTypes.None);
                }

                return;
            }

            if (cd.IsChanneling)
            {
                ChangeAnimationState(CombatAnimStates.Channel, DirectionTypes.None);
            }  
            else
            {
                ChangeAnimationState(CombatAnimStates.Idle, DirectionTypes.None);
            }
        }
    }
}