using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TTW.Combat
{
    public class HealthBarName : MonoBehaviour, IUIElement
    {
        TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        public void HideUI()
        {
            text.enabled = false;
        }

        public void RevealUI()
        {
            text.enabled = true;
        }
    }
}