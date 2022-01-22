using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TTW.Combat
{
    public class PublicValues : MonoBehaviour
    {
        [SerializeField] [Range(0f, 1000f)] public   float heartForHealthMin         = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float heartForHealthMax         = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float heartForDefenseMin        = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float heartForDefenseMax        = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float heartForSunLightMin       = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float heartForSunLightMax       = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float heartForMeleeMin          = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float heartForMeleeMax          = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float heartForRegenMin          = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float heartForRegenMax          = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float heartForSplashMin         = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float heartForSplashMax         = 0f;
        [Space(20)]
        [SerializeField] [Range(0f, 1000f)] public   float gritForDamageMin          = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gritForDamageMax          = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gritForDamageReductionMin = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gritForDamageReductionMax = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gritForLampLightMin       = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gritForLampLightMax       = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gritForAccuracyMin        = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gritForAccuracyMax        = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gritForRecoilReductionMin = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gritForRecoilReductionMax = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gritForCritDamageMin      = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gritForCritDamageMax      = 0f;
        [Space(20)]
        [SerializeField] [Range(0f, 1000f)] public   float gaitForCooldownMin        = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gaitForCooldownMax        = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gaitForArmorBreakMin      = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gaitForArmorBreakMax      = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gaitForElectricLightMin   = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gaitForElectricLightMax   = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gaitForBeelineMin         = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gaitForBeelineMax         = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gaitForEvasionMin         = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gaitForEvasionMax         = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float gaitForCritMin            = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float gaitForCritMax            = 0f;
        [Space(20)]
        [SerializeField] [Range(0f, 1000f)] public   float spiritForChannelMin       = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float spiritForChannelMax       = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float spiritForMagicResMin      = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float spiritForMagicResMax      = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float spiritForMagicMin         = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float spiritForMagicMax         = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float spiritForLifestealMin     = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float spiritForLifestealMax     = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float spiritForHealingMin       = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float spiritForHealingMax       = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float spiritForStatusPotencyMin = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float spiritForStatusPotencyMax = 0f;
        [Space(20)]
        [SerializeField] [Range(0f, 1000f)] public   float mindForAccuracyMin        = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float mindForAccuracyMax        = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float mindForEvasionMin         = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float mindForEvasionMax         = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float mindForMoonLightMin       = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float mindForMoonLightMax       = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float mindForTrapsMin           = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float mindForTrapsMax           = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float mindForCounterMin         = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float mindForCounterMax         = 0f;
        [Space(10)]
        [SerializeField] [Range(0f, 1000f)] public   float mindForBewilderMin        = 0f;
        [SerializeField] [Range(0f, 1000f)] public   float mindForBewilderMax        = 0f;

        public static PublicValues singleton;

        private void Awake()
        {
            singleton = this;
        }
    }
}