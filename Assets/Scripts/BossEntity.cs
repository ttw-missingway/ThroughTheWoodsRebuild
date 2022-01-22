using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TTW.Combat
{
    public class BossEntity : MonoBehaviour
    {
        [SerializeField] Boss _boss;
        [SerializeField] bool _distant;
        [SerializeField] BlackShield blackShield;
        [SerializeField] Material _defaultMaterial;
        [SerializeField] Material _channelingMaterial;
        SpriteRenderer spriteRenderer;
        Cooldown cooldown;
        AnimationController animationController;
        

        public BlackShield BlackShield => blackShield;

        public bool DestroyFlag { get; set; }

        public Boss Boss => _boss;

        public Wing GridWing => GetComponent<GridPosition>().GetWing();

        public Ability GetAbility(int abilityIndex) => GetComponent<AbilitySlots>().GetAbilities(abilityIndex);

        public int GetAbilityCount => GetComponent<AbilitySlots>().GetAbilities().Count;

        public bool Distant => _distant;

        public void SetDistant(bool distance)
        {
            _distant = distance;
        }

        private void Start()
        {
            animationController = AnimationController.singleton;
            animationController.OnAnimationFreeze += AnimationController_OnAnimationFreeze;
            animationController.OnAnimationFreezeEnd += AnimationController_OnAnimationFreezeEnd;
            cooldown = GetComponent<Cooldown>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            DestroyFlag = false;
        }

        private void AnimationController_OnAnimationFreezeEnd(object sender, EventArgs e)
        {
            Redraw();
        }

        private void AnimationController_OnAnimationFreeze(object sender, EventArgs e)
        {
            ResetHighlights();
        }

        public void Redraw()
        {
            ResetHighlights();

            if (cooldown.IsChanneling)
                BossChanneling();
        }

        public void BossChanneling()
        {
            spriteRenderer.material = _channelingMaterial;
        }

        public void ResetHighlights()
        {
            spriteRenderer.material = _defaultMaterial;
        }
    }
}