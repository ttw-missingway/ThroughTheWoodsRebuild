using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TTW.Combat
{
    public class VFX : MonoBehaviour
    {
        private ParticleSystem ps;
        CinemachineImpulseSource impulseSource;

        bool noParticles = false;

        [SerializeField] Vector3 targetHeightOffset = new Vector3(0f, 0f, 0f);
        [SerializeField] Vector3 casterHeightOffset = new Vector3(0f, 0f, 0f);



        public void Awake()
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();

            if (GetComponentInChildren<ParticleSystem>() != null)
            {
                ps = GetComponentInChildren<ParticleSystem>();
            }
            else
            {
                noParticles = true;
            }
            

            if (GetComponent<CinemachineImpulseSource>() != null)
            {
                impulseSource.GenerateImpulse();
            }
        }

        public void Update()
        {
            if (noParticles) return;

            if (!ps)
            {
                Destroy(gameObject);
            }
        }

        public void TargetLookAt(Targetable target)
        {
            transform.position += casterHeightOffset;
            transform.LookAt(target.transform, targetHeightOffset);
        }
    }
}