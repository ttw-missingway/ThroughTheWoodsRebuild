using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class UVPosition : MonoBehaviour
    {
        public float pixelSize = 1f;
        public float tileX = 1f;
        public float tileY = 1f;

        Vector2[] quadUVs = new Vector2[4];

        void Start()
        {
            float tilePerc = 1f;

            float umin = tilePerc * tileX;
            float umax = tilePerc * (tileX + 1);
            float vmin = tilePerc * tileY;
            float vmax = tilePerc * (tileY + 1);

            quadUVs[0] = new Vector2(umin, vmin);
            quadUVs[1] = new Vector2(umax, vmin);
            quadUVs[2] = new Vector2(umin, vmax);
            quadUVs[3] = new Vector2(umax, vmax);

            GetComponent<MeshFilter>().mesh.uv = quadUVs;
        }

        public void UpdateUV()
        {
            float tilePerc = 1f;

            float umin = tilePerc * tileX;
            float umax = tilePerc * (tileX + 1);
            float vmin = tilePerc * tileY;
            float vmax = tilePerc * (tileY + 1);

            quadUVs[0] = new Vector2(umin, vmin);
            quadUVs[1] = new Vector2(umax, vmin);
            quadUVs[2] = new Vector2(umin, vmax);
            quadUVs[3] = new Vector2(umax, vmax);

            GetComponent<MeshFilter>().mesh.uv = quadUVs;
        }

        public void ResetUV()
        {
            tileX = 0f;
            tileY = -2f;
            UpdateUV();
        }
    }
}
