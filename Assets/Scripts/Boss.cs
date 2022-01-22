using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public enum BossType { Gevaudan, ChoCho, Kraken, Lupin, Wendigo, Ipabog, Twins, Noh, Kabuki, Atlas, Suzume, Daphne, Airavata, Simurgh, Anansi, Coyote, Puck, MawMarshall, MawKanaloa, MawSuzume, MawLupin, MawFeist, Maw  }
    [CreateAssetMenu(fileName = "New Boss", menuName = "Boss")]

    public class Boss : ScriptableObject
    {
        public string bossName;
        public BossType bossType;
        public Sprite combatIcon;

        [Range(0, 100)] public int Heart;
        [Range(0, 100)] public int Gait;
        [Range(0, 100)] public int Grit;
        [Range(0, 100)] public int Mind;
        [Range(0, 100)] public int Spirit;
    }
}