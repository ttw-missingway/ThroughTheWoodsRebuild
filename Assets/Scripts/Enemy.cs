using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]

    public class Enemy : ScriptableObject
    {
        public string EnemyName;
        public Sprite combatIcon;
    }
}
