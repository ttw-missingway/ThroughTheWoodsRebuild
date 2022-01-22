using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TTW.Combat
{
    public abstract class UIText : MonoBehaviour
    {
        TMP_Text displayText;

        private void Start()
        {
            displayText = GetComponent<TMP_Text>();
        }

        public void SetText(string text)
        {
            displayText.text = text;
        }

        public void ClearHighlight()
        {
            displayText.fontStyle = FontStyles.Normal;
        }

        public void Highlight()
        {
            displayText.fontStyle = FontStyles.Underline;
        }

        public void ClearDisplay()
        {
            displayText.text = " ";
        }

        public void DisableText()
        {
            displayText.enabled = false;
        }

        public void EnableText()
        {
            displayText.enabled = true;
        }
    }
}