using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    [RequireComponent(typeof(GridPosition))]
    [ExecuteInEditMode]
    public class WingFlip : MonoBehaviour
    {
        GridPosition gridPosition;
        SpriteRenderer spriteRenderer;

        private void Awake()
        {
            gridPosition = GetComponent<GridPosition>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (gridPosition.GetWing() == Wing.Port)
            {
                spriteRenderer.flipX = true;
            }
            if (gridPosition.GetWing() == Wing.Starboard)
            {
                spriteRenderer.flipX = false;
            }
        }
    }
}
