using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TTW.Combat
{
    public class CooldownTimer : MonoBehaviour, IUIElement
    {
        [SerializeField] Cooldown _worldTarget;
        [Range(0f, 1f)][SerializeField] float pointerOffset = 0.2f;

        Image image;
        float fillRatio = 0f;
        RectTransform rectTransform;

        private void Start()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void UIUpdate(float cd, float cdMax)
        {
            fillRatio = (cd / cdMax);
            image.fillAmount = fillRatio;

            print("cooldown: " + cd);
            print("max CD: " + cdMax);

            CoolDownInactive();
        }

        public void CoolDownInactive()
        {
            var height = Screen.height;
            var heightOffset = height * pointerOffset;

            if (fillRatio > 1f)
            {
                image.enabled = false;
            }
            else
            {
                rectTransform.position = Camera.main.WorldToScreenPoint(_worldTarget.transform.position) + new Vector3(0f, heightOffset, 0f);
            }
        }

        public void SetTarget(Cooldown target)
        {
            _worldTarget = target;
        }

        public Cooldown TargetAssigned()
        {
            return _worldTarget;
        }

        public void RemoveTarget()
        {
            _worldTarget = null;
        }

        public void HideUI()
        {
            GetComponent<Image>().enabled = false;
        }

        public void RevealUI()
        {
            GetComponent<Image>().enabled = true;
        }
    }
}
