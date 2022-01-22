using System.Collections;
using System.Collections.Generic;
using TTW.Combat;
using UnityEngine;

public interface ICombatAnimation
{
    void ChangeAnimationState(CombatAnimStates newState, DirectionTypes facing);
    void EndAttackAnimation();
}
