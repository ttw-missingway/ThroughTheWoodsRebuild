using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public enum DirectionTypes
    {
        None,
        Up,
        Left,
        Right,
        Down
    }

    public class Directions
    {
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down
        };

        Vector2Int[] ordinalDirections =
        {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.down
        };

        public Vector2Int[] AllDirections => directions;
        public Vector2Int[] AllOrdinalDirections => ordinalDirections;
    }
}