using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] Transform targetTransform;
        [SerializeField] Vector3 targetOffset;
        [SerializeField] float flightTime = 1f;

        private Rigidbody rb;
        private GridPosition gridPos;
        [SerializeField] Vector3 originPosition;
        [SerializeField] float deathTimer = 3f;
        [SerializeField] VFX deathvfx;

        public void LoadProjectile(Vector3 origin, Transform target)
        {
            targetTransform = target;
            originPosition = origin;

            rb = GetComponent<Rigidbody>();
            transform.position = originPosition + targetOffset;
            gridPos = GetComponent<GridPosition>();

            LaunchProjectile();
        }

        public void LoadProjectile(Vector3 origin, Transform target, Vector3 offset)
        {
            targetTransform = target;
            originPosition = origin;
            targetOffset = offset;

            rb = GetComponent<Rigidbody>();
            transform.position = originPosition + targetOffset;
            gridPos = GetComponent<GridPosition>();

            LaunchProjectile();
        }

        public void LoadProjectile(Vector3 origin, Transform target, VFX death)
        {
            targetTransform = target;
            originPosition = origin;
            deathvfx = death;

            rb = GetComponent<Rigidbody>();
            transform.position = originPosition + targetOffset;
            gridPos = GetComponent<GridPosition>();

            LaunchProjectile();
        }

        public void LoadProjectile(Vector3 origin, Transform target, Vector3 offset, VFX death)
        {
            targetTransform = target;
            originPosition = origin;
            targetOffset = offset;
            deathvfx = death;

            rb = GetComponent<Rigidbody>();
            transform.position = originPosition + targetOffset;
            gridPos = GetComponent<GridPosition>();

            LaunchProjectile();
        }

        private void Update()
        {
            DestroyOnTarget();
            DeathTimer();
        }

        private void DeathTimer()
        {
            if (deathTimer >= 0)
            {
                deathTimer -= Time.deltaTime;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void DestroyOnTarget()
        {
            if (gridPos.GetGridPos() == targetTransform.GetComponent<GridPosition>().GetGridPos())
            {
                VFX newVFX = Instantiate(deathvfx);
                newVFX.transform.position = targetTransform.position + targetOffset;
                //caster.DealDamage();
                Destroy(this.gameObject);
            }
        }

        void LaunchProjectile()
        {
            Vector3 vo = CalculateVelocity(targetTransform.position, originPosition, flightTime);

            transform.rotation = Quaternion.identity;
            rb.velocity = vo;
        }

        Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
        {
            Vector3 distance = target - origin;
            Vector3 distanceXz = distance;
            distanceXz.y = 0f;

            float sY = distance.y;
            float sXz = distanceXz.magnitude;

            float Vxz = sXz / time;
            float Vy = (sY / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);

            Vector3 result = distanceXz.normalized;
            result *= Vxz;
            result.y = Vy;

            return result;
        }
    }
}
