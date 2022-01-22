using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class AtlasMove : MonoBehaviour
    {
        [SerializeField] Transform battleStage;
        [SerializeField] float speed;
        [SerializeField] float lurch;

        private void Start()
        {
            StartCoroutine(ApproachStage());
        }

        private IEnumerator ApproachStage()
        {
            while(transform.position.z > battleStage.position.z)
            {
                transform.position += Vector3.back * lurch;
                yield return new WaitForSeconds(speed);
            }
        }
    }
}
