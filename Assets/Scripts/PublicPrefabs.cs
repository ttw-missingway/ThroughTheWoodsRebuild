using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class PublicPrefabs : MonoBehaviour
    {
        [SerializeField] Trap _trapPrefab;

        public Trap GetTrapShell() => _trapPrefab;
    }
}