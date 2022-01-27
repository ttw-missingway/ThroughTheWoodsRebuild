using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.World
{
    public class WorldActorAnimation : MonoBehaviour
    {
        private Animator animator;
        private string currentStateString;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void ChangeAnimationState(CombatAnimStates newState, DirectionTypes facing)
        {
            string stateString = currentStateString;

            switch (newState)
            {
                case CombatAnimStates.Idle:
                    if (facing == DirectionTypes.Down) stateString = "Art Idle Front";
                    else if (facing == DirectionTypes.Left) stateString = "Art Idle Left";
                    else if (facing == DirectionTypes.Right) stateString = "Art Idle Right";
                    else stateString = "Art Idle Back";

                    break;

                case CombatAnimStates.Channel:
                    stateString = "Art Channel";
                    break;

                case CombatAnimStates.Attack:
                    if (facing == DirectionTypes.Left) stateString = "Art Attack Left";
                    else if (facing == DirectionTypes.Right) stateString = "Art Attack Right";
                    else stateString = "Art Attack Up";
                    break;

                case CombatAnimStates.Cast:
                    if (facing == DirectionTypes.Left) stateString = "Art Cast Left";
                    else if (facing == DirectionTypes.Right) stateString = "Art Cast Right";
                    else if (facing == DirectionTypes.Up) stateString = "Art Cast Up";
                    else stateString = "Art Cast Down";
                    break;

                case CombatAnimStates.Guard:
                    stateString = "Art Guard";
                    break;

                case CombatAnimStates.Run:
                    if (facing == DirectionTypes.Down) stateString = "Art Run Down";
                    else if (facing == DirectionTypes.Left) stateString = "Art Run Left";
                    else if (facing == DirectionTypes.Right) stateString = "Art Run Right";
                    else stateString = "Art Run Up";
                    break;

                case CombatAnimStates.LightDamage:
                    stateString = "Art Damage Light";
                    break;

                case CombatAnimStates.HeavyDamage:
                    stateString = "Art Heavy Damage";
                    break;

                case CombatAnimStates.NormalDamage:
                    stateString = "Art Heavy Damage";
                    break;

                //case jump
                //case world action
                //case world interact
                //etc.
            }

            if (currentStateString == stateString) return;

            animator.Play(stateString);
        }
    }
}
