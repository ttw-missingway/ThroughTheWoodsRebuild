using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TTW.Combat
{
    public class TargetIcon : MonoBehaviour, IUIElement
    {
        Image _image;
        public bool _hasHealthBar = false;
        [SerializeField] Image _healthBarShell;
        [SerializeField] HealthBar _healthBar;

        public void HideUI()
        {
            _image.enabled = false;
        }

        public void RevealUI()
        {
            _image.enabled = true;
        }

        public void LoadStats(Sprite sprite, StatsHandler stats)
        {
            DeactivateHealthBars();

            if (stats.GetComponent<ActorEntity>() != null)
            {
                ActivateHealthBars();
            }

            _image = GetComponent<Image>();
            _image.sprite = sprite;

            var health = stats.Health;
            var maxHealth = stats.MaxHealth;

            _healthBar.UpdateHealth(health, maxHealth);
        }

        private void DeactivateHealthBars()
        {
            _healthBar.GetComponent<Image>().enabled = false;
            _healthBarShell.enabled = false;
            _hasHealthBar = false;
        }

        private void ActivateHealthBars()
        {
            _healthBar.GetComponent<Image>().enabled = true;
            _healthBarShell.enabled = true;
            _hasHealthBar = true;
        }
    }
}
