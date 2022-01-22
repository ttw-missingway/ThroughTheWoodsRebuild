using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class Bench : MonoBehaviour
    {
        MeshRenderer[] highlightMeshes;

        private void Start()
        {
            highlightMeshes = GetComponentsInChildren<MeshRenderer>();
        }

        public void SetHighlight(bool isHighlighted)
        {
            foreach (MeshRenderer m in highlightMeshes)
            {
                m.enabled = isHighlighted;
            }
        }
    }
}
