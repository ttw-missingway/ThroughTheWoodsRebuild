using System.Collections;
using System.Collections.Generic;
using TTW.Combat;
using UnityEngine;

public enum CombatAnimStates
{
    Idle,
    Channel,
    Attack,
    Cast,
    LightDamage,
    NormalDamage,
    HeavyDamage,
    Guard,
    Run,
    Special1,
    Special2,
    Special3
}

public class ActorCombatAnimation : MonoBehaviour, ICombatAnimation
{
    private Animator animator;

    private string currentStateString;

    private Cooldown cd;

    private void Start()
    {
        currentStateString = " ";
        animator = GetComponent<Animator>();
        cd = GetComponent<Cooldown>();
    }

    public void ChangeAnimationState(CombatAnimStates newState, DirectionTypes facing)
    {
        string stateString = currentStateString;
        StatsHandler actorStats = GetComponent<StatsHandler>();

        switch (newState)
        {
            case CombatAnimStates.Idle:
                if (cd.IsChanneling)
                {
                    stateString = "Art Channel";
                    break;
                }
                    
                if (actorStats.Health > actorStats.MaxHealth / 2)
                {
                    if (facing == DirectionTypes.Down) stateString = "Art Idle Front";
                    else if (facing == DirectionTypes.Left) stateString = "Art Idle Left";
                    else if (facing == DirectionTypes.Right) stateString = "Art Idle Right";
                    else stateString = "Art Idle Back";
                }
                else if (actorStats.Health > actorStats.MaxHealth / 10)
                {
                    if (facing == DirectionTypes.Down) stateString = "Art HH Down";
                    else if (facing == DirectionTypes.Left) stateString = "Art HH Left";
                    else if (facing == DirectionTypes.Right) stateString = "Art HH Right";
                    else stateString = "Art HH Up";
                }
                else if (actorStats.Alive)
                {
                    if (facing == DirectionTypes.Down) stateString = "Art ND Down";
                    else if (facing == DirectionTypes.Left) stateString = "Art ND Left";
                    else if (facing == DirectionTypes.Right) stateString = "Art ND Right";
                    else stateString = "Art ND Up";
                }


                if (!actorStats.Alive)
                {
                    stateString = "Art Dead";
                }

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
        }

        if (currentStateString == stateString) return;

        animator.Play(stateString);
    }

    public void EndAttackAnimation()
    {
        if (cd.IsChanneling)
        {
            ChangeAnimationState(CombatAnimStates.Channel, GetComponent<ActorEntity>().GetFacingDirection());
        }
        else
        {
            ChangeAnimationState(CombatAnimStates.Idle, GetComponent<ActorEntity>().GetFacingDirection());
        }  
    }
}
