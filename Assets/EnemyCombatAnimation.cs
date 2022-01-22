using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class EnemyCombatAnimation : MonoBehaviour, ICombatAnimation
    {
        private Animator animator;

        private string currentStateString;

        private void Start()
        {
            currentStateString = " ";
            animator = GetComponent<Animator>();
        }

        public void ChangeAnimationState(CombatAnimStates newState, DirectionTypes facing)
        {

            string stateString = currentStateString;
            StatsHandler stats = GetComponent<StatsHandler>();

            switch (newState)
            {
                case CombatAnimStates.Idle:
                    if (stats.Health > 0)
                    {
                        stateString = "Idle";
                    }
                    else
                    {
                        stateString = "Dead";
                    }
                    break;

                case CombatAnimStates.Channel:
                    stateString = "Channel";
                    break;

                case CombatAnimStates.Attack:
                    stateString = "Attack";
                    break;

                case CombatAnimStates.Special1:
                    stateString = "Special1";
                    break;

                case CombatAnimStates.Special2:
                    stateString = "Special2";
                    break;

                case CombatAnimStates.Guard:
                    stateString = "Guard";
                    break;

                case CombatAnimStates.LightDamage:
                    stateString = "Damage";
                    break;

                case CombatAnimStates.HeavyDamage:
                    stateString = "Damage";
                    break;
            }

            if (currentStateString == stateString) return;

            animator.Play(stateString);
        }

        public void EndAttackAnimation()
        {
            ChangeAnimationState(CombatAnimStates.Idle, DirectionTypes.None);
        }
    }
}
