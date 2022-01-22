using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TTW.Combat
{
    public class HealthBar : MonoBehaviour, IUIElement
    {
        Image healthbar = null;
        [SerializeField] Image healthbarShell = null;
        [SerializeField] float _displayHealth = 0f;
        [SerializeField] float _maxHealth = 0f;

        public void HideUI()
        {
            healthbar.enabled = false;
            healthbarShell.enabled = false;
        }

        public void RevealUI()
        { 
            healthbar.enabled = true;
            healthbarShell.enabled = true;
        }

        public void UpdateHealth(float displayHealth, float maxHealth)
        {
            healthbar = GetComponent<Image>();

            _displayHealth = displayHealth;
            _maxHealth = maxHealth;

            DrawHealth();
        }

        private void DrawHealth()
        {
            healthbar.fillAmount = _displayHealth / _maxHealth;
        }
    }
}