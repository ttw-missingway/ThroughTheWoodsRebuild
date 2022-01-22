using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class AbilitySlots : MonoBehaviour
    {
        [SerializeField] List<Ability> abilities = new List<Ability>();
        [SerializeField] bool usedLegendary = false;
        [SerializeField] public bool usesDeck = false;
        [SerializeField] List<Deck> enemyDeck = new List<Deck>();

        public void UseLegendary()
        {
            usedLegendary = true;
        }

        public bool UsedLegendary => usedLegendary;

        public Ability GetAbilities(int abilityIndex) => abilities[abilityIndex];

        public List<Ability> GetAbilities() => abilities;

        public Deck GetDeck(int index) => enemyDeck[index];
    }
}