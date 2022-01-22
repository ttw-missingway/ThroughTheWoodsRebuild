using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    [ExecuteInEditMode]
    [SelectionBase]
    [RequireComponent(typeof(GridPosition))]


    public class EditorSnap : MonoBehaviour
    {
        [SerializeField] Vector2Int _gridPos;
        const int gridSize = 10;

        void Update()
        {
            SnapToGrid();
            UpdateLabel();
        }

        private void SnapToGrid()
        {
            transform.position = new Vector3(GetGridPos().x * gridSize, 0f, GetGridPos().y * gridSize);
        }

        private void UpdateLabel()
        {
            Vector2Int snapPos = GetGridPos();

            string labelText = snapPos.x + "," + snapPos.y;
            gameObject.name = labelText;
        }

        private Vector2Int GetGridPos()
        {
            _gridPos = new Vector2Int

            (
                Mathf.RoundToInt(transform.position.x / gridSize),
                Mathf.RoundToInt(transform.position.z / gridSize)
            );

            return _gridPos;
        }
    }
}