using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.World
{
    public class WorldMovement : MonoBehaviour
    {
        public float walkSpeed = 8f;
        public float jumpSpeed = 7f;

        Rigidbody rb;
        WorldActorAnimation anim;
        DirectionTypes facingDir;

        void Start()
        {
            facingDir = DirectionTypes.Down;
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<WorldActorAnimation>();
        }

        void Update()
        {
            WalkHandler();
        }

        void WalkHandler()
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            float distance = walkSpeed * Time.deltaTime;
            float hAxis = Input.GetAxis("Horizontal");
            float vAxis = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(hAxis * distance, 0f, vAxis * distance);
            Vector3 currPosition = transform.position;
            Vector3 newPosition = currPosition + movement;
            UpdateAnimation(currPosition, newPosition);
            rb.MovePosition(newPosition);
        }

        private void UpdateAnimation(Vector3 currPosition, Vector3 newPosition)
        {
            if (newPosition.x > currPosition.x)
            {
                facingDir = DirectionTypes.Right;
            }
            else if (newPosition.x < currPosition.x)
            {
                facingDir = DirectionTypes.Left;
            }
            else if (newPosition.z > currPosition.z)
            {
                facingDir = DirectionTypes.Up;
            }
            else if (newPosition.z < currPosition.z)
            {
                facingDir = DirectionTypes.Down;
            }

            if (currPosition == newPosition)
            {
                anim.ChangeAnimationState(CombatAnimStates.Idle, facingDir);
            }
            else
            {
                anim.ChangeAnimationState(CombatAnimStates.Run, facingDir);
            }
        }
    }
}