using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    [ExecuteInEditMode]
    public class GridPosition : MonoBehaviour
    {
        [SerializeField] Vector2Int _gridPos;
        [SerializeField] Wing wing;
        [SerializeField] bool wingOverride = false;
        const int gridSize = 10;
        [SerializeField] bool nonPositional;

        private void Awake()
        {
            GetGridPos();
        }

        private void Update()
        {
            GetGridPos();
            GetWing();
        }

        public Wing GetWing()
        {
            if (wingOverride) return wing;

            if (_gridPos.x <= 0)
                wing = Wing.Port;
            else if (_gridPos.x >= 5)
                wing = Wing.Starboard;
            else if (_gridPos.y >= 3)
                wing = Wing.Bow;
            else
                wing = Wing.Amidships;

            return wing;
        }

        public Vector2Int GetGridPos()
        {
            _gridPos = new Vector2Int

            (
                Mathf.RoundToInt(transform.position.x / gridSize),
                Mathf.RoundToInt(transform.position.z / gridSize)
            );

            return _gridPos;
        }

        public int GetGridSize()
        {
            return gridSize;
        }
    }
}