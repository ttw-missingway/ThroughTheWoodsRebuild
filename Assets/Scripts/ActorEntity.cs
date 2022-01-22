using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TTW.Combat
{
    public class ActorEntity : MonoBehaviour
    {
        [SerializeField] Actor _actor;

        [SerializeField] SelectionState _selectionState = SelectionState.ActorSelect;

        [SerializeField] Ability _highlightedAbility;

        [SerializeField] List<Targetable> _highlightedTargets;

        [SerializeField] Cell _highlightedCell;

        [SerializeField] Material _defaultMaterial;

        [SerializeField] Material _highlightMaterial;

        [SerializeField] Material _unavailableMaterial;

        [SerializeField] Material _channelingMaterial;

        [SerializeField] Material _deadMaterial;

        [SerializeField] DirectionTypes _facingDirection = DirectionTypes.Down;

        [SerializeField] bool isHighlighted = false;

        SpriteRenderer spriteRenderer;

        AnimationController animController;

        Cooldown cooldown;

        StatsHandler stats;

        public bool DestroyFlag { get; set; }

        public Actor Actor => _actor;

        public Vector2Int GridPosition => GetComponent<GridPosition>().GetGridPos();

        public Ability GetAbility(int abilityIndex) => GetComponent<AbilitySlots>().GetAbilities(abilityIndex);

        public int GetAbilityCount => GetComponent<AbilitySlots>().GetAbilities().Count;

        private void Awake()
        {
            DestroyFlag = false;
            spriteRenderer = GetComponent<SpriteRenderer>();
            stats = GetComponent<StatsHandler>();
            cooldown = GetComponent<Cooldown>();
        }

        private void Start()
        {
            animController = AnimationController.singleton;
            animController.OnAnimationFreeze += AnimController_OnAnimationFreeze;
            animController.OnAnimationFreezeEnd += AnimController_OnAnimationFreezeEnd;
            cooldown.OnCooldownEnd += Cooldown_OnCooldownEnd;
        }

        private void Cooldown_OnCooldownEnd(object sender, EventArgs e)
        {
            Redraw();
        }

        private void AnimController_OnAnimationFreezeEnd(object sender, EventArgs e)
        {
            Redraw();
        }

        private void AnimController_OnAnimationFreeze(object sender, EventArgs e)
        {
            ResetHighlights();
        }

        public void SaveSelectionState(SelectionPacket packet)
        {
            _selectionState = packet.SelectionState;
            _highlightedAbility = packet.HighlightedAbility;
            _highlightedCell = packet.HighlightedCell;
            _highlightedTargets = packet.HighlightedTargets;
        }

        public void Redraw()
        {
            ResetHighlights();

            if (cooldown.IsChanneling)
            {
                ActorChanneling();
            }
            else if (cooldown.IsOnCooldown)
            {
                DitherActor();
            }
            else if (isHighlighted)
            {
                HighlightActor();
            }
        }

        public SelectionPacket LoadSelectionState()
        {
            SelectionPacket packet = new SelectionPacket(_selectionState, _highlightedAbility, _highlightedCell, _highlightedTargets);

            return packet;
        }

        public void ClearHighlight()
        {
            isHighlighted = false;
        }

        public void SetHighlightedActor()
        {
            isHighlighted = true;
        }

        public void HighlightActor()
        {
            spriteRenderer.material = _highlightMaterial;
        }

        public void DitherActor()
        {
            spriteRenderer.material = _unavailableMaterial;
        }

        public void ResetHighlights()
        {
            spriteRenderer.material = _defaultMaterial;
        }

        public void ActorChanneling()
        {
            spriteRenderer.material = _channelingMaterial;
        }

        public void DeadActor()
        { 
            spriteRenderer.material = _deadMaterial;
        }

        public void ChangeFacingDirection(DirectionTypes direction)
        {
            _facingDirection = direction;
        }

        public DirectionTypes GetFacingDirection()
        {
            return _facingDirection;
        }

        public void Unsubscribe()
        {
            animController.OnAnimationFreeze -= AnimController_OnAnimationFreeze;
            animController.OnAnimationFreezeEnd -= AnimController_OnAnimationFreezeEnd;
        }

        public bool IsHighlighted => (spriteRenderer.material == _highlightMaterial);
    }
}
