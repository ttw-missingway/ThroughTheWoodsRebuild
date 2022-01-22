using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace TTW.Combat
{
    public class AbilityText : UIText
    {
        Ability assignedAbility;
        [SerializeField] float yPos;

        private void Awake()
        {
            LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), 0.4f).setEaseInOutSine();
        }

        public void AssignAbility(Ability ability)
        {
            ScrollDown();
            assignedAbility = ability;
            SetText(assignedAbility.name);
        }

        private void ScrollDown()
        {
            LeanTween.cancel(gameObject.GetComponent<RectTransform>());
            LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector2(0f, yPos), 0.4f).setEaseInOutSine();
        }

        public void ScrollUp()
        {
            LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), 0.4f).setEaseInOutSine().setOnComplete(ClearDisplay);
        }
    }
}