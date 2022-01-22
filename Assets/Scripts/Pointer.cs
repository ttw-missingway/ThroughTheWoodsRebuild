using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TTW.Combat
{
    public enum PointerTypes
    {
        Chevron,
        Footstep,
        Invalid,
        Crosshair,
        Confirm
    }

    public class Pointer : MonoBehaviour, IUIElement
    {
        [SerializeField] Sprite currentSprite;
        [SerializeField] Sprite chevron;
        [SerializeField] Sprite footstep;
        [SerializeField] Sprite invalid;
        [SerializeField] Sprite crosshair;
        [SerializeField] Sprite confirm;
        [Range(0f, 1f)][SerializeField] float pointerOffset = 0f;
        [SerializeField] private Camera cam;
        bool spriteEnabled = false;

        public void ChangePointer(PointerTypes pointerType)
        {
            switch (pointerType)
            {
                case PointerTypes.Chevron:
                    currentSprite = chevron;
                    break;
                case PointerTypes.Footstep:
                    currentSprite = footstep;
                    break;
                case PointerTypes.Invalid:
                    currentSprite = invalid;
                    break;
                case PointerTypes.Crosshair:
                    currentSprite = crosshair;
                    break;
                case PointerTypes.Confirm:
                    currentSprite = confirm;
                    break;
            }

            GetComponent<Image>().sprite = currentSprite;
        }

        public void DisablePointer()
        {
            GetComponent<Image>().enabled = false;
            spriteEnabled = false;
        }

        public void EnablePointer()
        {
            GetComponent<Image>().enabled = true;
            spriteEnabled = true;
        }

        public void SetPointerObject(GameObject target)
        {
            RectTransform rect = GetComponent<RectTransform>();
            LeanTween.cancel(rect);

            var height = Screen.height;
            var heightOffset = height * pointerOffset;

            Vector3 currentPosition = rect.position;
            Vector3 newPosition = cam.WorldToScreenPoint(target.transform.position) + new Vector3(0f, heightOffset, 0f);

            LeanTween.value(gameObject, MoveCursorUpdate, currentPosition, newPosition, 0.2f)
                .setEaseInSine();

            //GetComponent<RectTransform>().position = cam.WorldToScreenPoint(target.transform.position) + new Vector3(0f, heightOffset, 0f);
        }

        private void MoveCursorUpdate(Vector3 pos)
        {
            GetComponent<RectTransform>().position = pos;
        }


        public void HideUI()
        {
            if (spriteEnabled)
                GetComponent<Image>().enabled = false;
        }

        public void RevealUI()
        {
            if (spriteEnabled)
                GetComponent<Image>().enabled = true;
        }
    }
}
