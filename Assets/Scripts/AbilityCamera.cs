using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace TTW.Combat
{
    public class AbilityCamera : MonoBehaviour
    {
        [SerializeField] [Range(1, 3)] int cameraActive = 1;
        [SerializeField] CinemachineVirtualCamera primaryCamera;
        [SerializeField] CinemachineVirtualCamera casterCamera;
        [SerializeField] CinemachineVirtualCamera targetCamera;
        [SerializeField] CinemachineVirtualCamera fieldCamera;

        Animator animator;

        private void Start()
        {
            FindCameras();
        }

        public void FindCameras()
        {
            animator = GameObject.FindGameObjectWithTag("CamAnimator").GetComponent<Animator>();
            primaryCamera = GameObject.FindGameObjectWithTag("PrimaryCam").GetComponent<CinemachineVirtualCamera>();
            casterCamera = GameObject.FindGameObjectWithTag("CasterCam").GetComponent<CinemachineVirtualCamera>();
            targetCamera = GameObject.FindGameObjectWithTag("TargetCam").GetComponent<CinemachineVirtualCamera>();
            fieldCamera = GameObject.FindGameObjectWithTag("FieldCam").GetComponent<CinemachineVirtualCamera>();
        }

        public void FocusCaster(Targetable caster)
        {
            casterCamera.LookAt = caster.transform;
        }

        public void FocusTarget(Targetable target)
        {
            targetCamera.LookAt = target.transform;
        }

        public void SetCamera(CameraType camera)
        {
            if (camera == CameraType.caster)
            {
                animator.SetTrigger("casterCam");
            }
            if (camera == CameraType.target)
            {
                animator.SetTrigger("targetCam");
            }
            if (camera == CameraType.field)
            {
                animator.SetTrigger("fieldCam");
            }
            if (camera == CameraType.home)
            {
                animator.SetTrigger("primaryCam");
            }
        }

        public void DutchAngle(float degree)
        {
            fieldCamera.m_Lens.Dutch = degree;
        }

        public void Reset()
        {
            fieldCamera.m_Lens.Dutch = 0;
        }
    }
}
