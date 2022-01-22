using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    [CreateAssetMenu(fileName = "New Deck", menuName = "Deck")]

    public class Deck: ScriptableObject
    {
        public Ability[] abilities;
        public int[] entries;
        public Ability[] combo;
        public float healthPercentageThreshold;
        public Wing wing;
    }
}