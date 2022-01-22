using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TTW.Combat
{
    public class SelectionText : MonoBehaviour, IUIElement
    {
        TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        public void HideUI()
        {
            LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector2(-200f, 0f), 0.4f).setEaseInOutSine().setOnComplete(() => { text.enabled = false; });
        }

        public void RevealUI()
        {
            LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), 0.4f).setEaseInOutSine();
            text.enabled = true;
        }

        public void SetText(string newText)
        {
            text.text = newText;
        }
    }
}