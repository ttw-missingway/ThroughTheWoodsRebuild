using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public class AnimationController : MonoBehaviour
    {
        public static AnimationController singleton;
        [SerializeField] bool _isFrozen = false;
        [SerializeField] int ongoingAnimations = 0;
        [SerializeField] PauseIcon pauseIcon;
        

        public event EventHandler OnAnimationFreeze;
        public event EventHandler OnAnimationFreezeEnd;
        public event EventHandler OnMenuFreeze;
        public event EventHandler OnMenuFreezeEnd;
        

        private void Awake()
        {
            SetSingleton();
        }

        private void SetSingleton()
        {
            if (singleton == null)
            {
                singleton = this;
            }
            else
            {
                print("ERROR: more than one AnimationFreeze in scene!");
            }
        }

        public void StartAnimationFreeze()
        {
            ongoingAnimations++;
            _isFrozen = true;
            OnAnimationFreeze?.Invoke(this, EventArgs.Empty);
            HideGUI();
        }

        public void EndAnimationFreeze()
        {
            ongoingAnimations--;

            if (ongoingAnimations == 0)
            {
                _isFrozen = false;
                OnAnimationFreezeEnd?.Invoke(this, EventArgs.Empty);
                RevealGUI();
            }
        }

        private static void HideGUI()
        {
            var gui = FindObjectsOfType<MonoBehaviour>().OfType<IUIElement>();
            foreach (IUIElement u in gui)
            {
                u.HideUI();
            }
        }

        private static void RevealGUI()
        {
            var gui = FindObjectsOfType<MonoBehaviour>().OfType<IUIElement>();
            foreach (IUIElement u in gui)
            {
                u.RevealUI();
            }
        }

        public void StartMenuFreeze()
        {
            _isFrozen = true;
            OnMenuFreeze?.Invoke(this, EventArgs.Empty);
            pauseIcon.SetMenuOpen(true);
        }

        public void EndMenuFreeze()
        {
            _isFrozen = false;
            OnMenuFreezeEnd?.Invoke(this, EventArgs.Empty);
            pauseIcon.SetMenuOpen(false);
        }
    }
}