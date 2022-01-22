using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class AbilitySelection : MonoBehaviour
    {
        [SerializeField] List<Ability> displayedAbilities = new List<Ability>();
        [SerializeField] List<AbilityText> abilityTexts = new List<AbilityText>();
        [SerializeField] InfoPanel _infoPanel;
        int highlightIndex = 0;

        public void DisplayAbilities(ActorEntity actor)
        {
            displayedAbilities.Clear();

            for (int i = 0; i < actor.GetAbilityCount; i++)
            {
                displayedAbilities.Add(actor.GetAbility(i));
                abilityTexts[i].AssignAbility(displayedAbilities[i]);
            }
        }

        public void ClearDisplay()
        {
            foreach (var abilityText in abilityTexts)
            {
                abilityText.ScrollUp();
            }
        }

        public void Highlight(DirectionTypes direction)
        {
            foreach (AbilityText text in abilityTexts)
            {
                text.ClearHighlight();
            }

            switch (direction)
            {
                case DirectionTypes.Up:
                    if (highlightIndex > 0)
                        highlightIndex--;
                    break;
                case DirectionTypes.Down:
                    if (highlightIndex < displayedAbilities.Count - 1)
                        highlightIndex++;
                    break;
                default:
                    print("ERROR, unexpected highlight direction");
                    break;
            }

            abilityTexts[highlightIndex].Highlight();
            _infoPanel.ReadInfo(displayedAbilities[highlightIndex].description);
        }

        public Ability SelectAbility()
        {
            return displayedAbilities[highlightIndex];
        }

        public void ResetHighlightIndex()
        {
            highlightIndex = 0;
        }
    }
}