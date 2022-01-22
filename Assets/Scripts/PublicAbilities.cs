using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class PublicAbilities : MonoBehaviour
    {
        public static PublicAbilities singleton;
        public Ability vampire;
        public Ability splash;
        public VFX lightFX;
        public VFX normalFX;
        public VFX heavyFX;
        public VFX critFX;
        public VFX missFX;
        public VFX armorBreakFX;
        public VFX cancelledFX;

        private void Awake()
        {
            singleton = this;
        }
    }
}