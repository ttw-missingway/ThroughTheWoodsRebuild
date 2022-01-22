using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class EventTransformer : MonoBehaviour
    {
        public void StartTilt(DirectionTypes dir)
        {
            float amount = 8f;

            if (dir == DirectionTypes.Right)
            {
                amount *= -1;
            }

            LeanTween.rotateZ(gameObject, amount, 3f).setOnComplete(ReturnTilt);
        }

        public void ReturnTilt()
        {
            LeanTween.rotateZ(gameObject, 0f, 3f);
        }
    }
}