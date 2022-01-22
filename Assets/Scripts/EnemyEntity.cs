using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public enum Wing
    {
        Amidships,
        Starboard,
        Port,
        Bow
    }

    public class EnemyEntity : MonoBehaviour
    {
        [SerializeField] string _name;
        [SerializeField] Material _defaultMaterial;
        [SerializeField] Material _channelingMaterial;
        SpriteRenderer spriteRenderer;
        Cooldown cooldown;
        AnimationController animationController;

        public Vector2Int GridPosition => GetComponent<GridPosition>().GetGridPos();

        public string Name => _name;
        public Enemy enemyType;

        private void Start()
        {
            animationController = AnimationController.singleton;
            animationController.OnAnimationFreeze += AnimationController_OnAnimationFreeze;
            animationController.OnAnimationFreezeEnd += AnimationController_OnAnimationFreezeEnd;
            cooldown = GetComponent<Cooldown>();
            spriteRenderer = GetComponent<SpriteRenderer>();
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
                EnemyChanneling();
        }

        public void EnemyChanneling()
        {
            spriteRenderer.material = _channelingMaterial;
        }

        public void ResetHighlights()
        {
            spriteRenderer.material = _defaultMaterial;
        }
    }
}
