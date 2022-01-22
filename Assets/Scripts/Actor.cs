using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public enum ActorType { Art, Aisling, Rim, Nicolo, Kazan, Krusk, Freyja, Rusalka, Giles, Unit, Whisperwill, Kanaloa}
    [CreateAssetMenu(fileName = "New Actor", menuName = "Actor")]

    public class Actor : ScriptableObject
    {
        public string ActorName;
        public ActorType actorType;
        public Sprite portrait;

        [Range(0, 1000)] public float baseHealth;
        [Range(0, 100)] public int Heart;
        [Range(0, 100)] public int Gait;
        [Range(0, 100)] public int Grit;
        [Range(0, 100)] public int Mind;
        [Range(0, 100)] public int Spirit;
    }
}