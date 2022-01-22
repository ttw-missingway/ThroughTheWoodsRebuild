using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class StatModifierTool
    {
        PublicValues publicValues;

        public StatModifierTool(PublicValues publicValues)
        {
            this.publicValues = publicValues;
        }

        private float ConvertToPercentage(float value) => value / 100;

        public float UmbraAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.spiritForMagicMin, publicValues.spiritForMagicMax, ConvertToPercentage(caster.Spirit))) * 2;

        public float SunLightAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.heartForSunLightMin, publicValues.heartForSunLightMax, ConvertToPercentage(caster.Heart))) + ConvertToPercentage(Mathf.Lerp(publicValues.spiritForMagicMin, publicValues.spiritForMagicMax, ConvertToPercentage(caster.Spirit)));

        public float MoonLightAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.mindForMoonLightMin, publicValues.mindForMoonLightMax, ConvertToPercentage(caster.Mind))) + ConvertToPercentage(Mathf.Lerp(publicValues.spiritForMagicMin, publicValues.spiritForMagicMax, ConvertToPercentage(caster.Spirit)));

        public float LampLightAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.gritForLampLightMin, publicValues.gritForLampLightMax, ConvertToPercentage(caster.Grit))) + ConvertToPercentage(Mathf.Lerp(publicValues.spiritForMagicMin, publicValues.spiritForMagicMax, ConvertToPercentage(caster.Spirit)));

        public float ElectricLightAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.gaitForElectricLightMin, publicValues.gaitForElectricLightMax, ConvertToPercentage(caster.Gait))) + ConvertToPercentage(Mathf.Lerp(publicValues.spiritForMagicMin, publicValues.spiritForMagicMax, ConvertToPercentage(caster.Spirit)));

        public float BaseDamageAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.gritForDamageMin, publicValues.gritForDamageMax, ConvertToPercentage(caster.Grit)));

        public float BaseHealingAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.spiritForHealingMin, publicValues.spiritForHealingMax, ConvertToPercentage(caster.Spirit)));

        public float CritChanceAdjustment(StatsHandler caster) => Mathf.Lerp(publicValues.gaitForCritMin, publicValues.gaitForCritMax, ConvertToPercentage(caster.Gait));

        public float ArmorBreakAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.gaitForArmorBreakMin, publicValues.gaitForArmorBreakMax, ConvertToPercentage(caster.Gait)));

        public float VampirismAdjustment(StatsHandler caster) => ConvertToPercentage(Mathf.Lerp(publicValues.spiritForLifestealMin, publicValues.spiritForLifestealMax, ConvertToPercentage(caster.Spirit)));

        public float SplashAdjustment(StatsHandler caster) => ConvertToPercentage(Mathf.Lerp(publicValues.heartForSplashMin, publicValues.heartForSplashMax, ConvertToPercentage(caster.Heart)));

        public float BewilderAdjustment(StatsHandler caster) => Mathf.Lerp(publicValues.mindForBewilderMin, publicValues.mindForBewilderMax, ConvertToPercentage(caster.Mind));

        public float EvasionAdjustment(StatsHandler target) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.mindForEvasionMin, publicValues.mindForEvasionMax, ConvertToPercentage(target.Mind))) + ConvertToPercentage(Mathf.Lerp(publicValues.gaitForEvasionMin, publicValues.gaitForEvasionMax, ConvertToPercentage(target.Gait)));

        public float AccuracyAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.mindForAccuracyMin, publicValues.mindForAccuracyMax, ConvertToPercentage(caster.Mind))) + ConvertToPercentage(Mathf.Lerp(publicValues.gritForAccuracyMin, publicValues.gritForAccuracyMax, ConvertToPercentage(caster.Grit)));

        public float BurnAdjustment(StatsHandler caster) => ConvertToPercentage(Mathf.Lerp(publicValues.spiritForStatusPotencyMin, publicValues.spiritForStatusPotencyMax, ConvertToPercentage(caster.Spirit)));

        public float HealthAdjustment(StatsHandler target) => Mathf.Lerp(publicValues.heartForHealthMin, publicValues.heartForHealthMax, ConvertToPercentage(target.Heart));

        public float MeleeAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.heartForMeleeMin, publicValues.heartForMeleeMax, ConvertToPercentage(caster.Heart)));

        public float DefenseAdjustment(StatsHandler target) => Mathf.Lerp(publicValues.heartForDefenseMin, publicValues.heartForDefenseMax, ConvertToPercentage(target.Heart));

        public float MagicResAdjustment(StatsHandler target) => Mathf.Lerp(publicValues.spiritForMagicResMin, publicValues.spiritForMagicResMax, ConvertToPercentage(target.Spirit));

        public float GuardAdjustment(StatsHandler target) => 1 - ConvertToPercentage(Mathf.Lerp(publicValues.gritForDamageReductionMin, publicValues.gritForDamageReductionMax, ConvertToPercentage(target.Grit)));

        public float CooldownAdjustment(StatsHandler target) => 1 - ConvertToPercentage(Mathf.Lerp(publicValues.gaitForCooldownMin, publicValues.gaitForCooldownMax, ConvertToPercentage(target.Gait)));

        public float ChannelAdjustment(StatsHandler target) => 1 - ConvertToPercentage(Mathf.Lerp(publicValues.spiritForChannelMin, publicValues.spiritForChannelMax, ConvertToPercentage(target.Spirit)));

        public float StatusAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.spiritForStatusPotencyMin, publicValues.spiritForStatusPotencyMax, ConvertToPercentage(caster.Spirit)));

        public float RecoilAdjustment(StatsHandler caster) => 1 - ConvertToPercentage(Mathf.Lerp(publicValues.gritForRecoilReductionMin, publicValues.gritForRecoilReductionMax, ConvertToPercentage(caster.Grit)));

        public float BeelineAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.gaitForBeelineMin, publicValues.gaitForBeelineMax, ConvertToPercentage(caster.Gait)));

        public float CritDamageAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.gritForCritDamageMin, publicValues.gritForCritDamageMax, ConvertToPercentage(caster.Grit)));

        public float ReceivedHealingAdjustment(StatsHandler caster) => 1 + ConvertToPercentage(Mathf.Lerp(publicValues.heartForRegenMin, publicValues.heartForRegenMax, ConvertToPercentage(caster.Heart)));

        public float TrapAdjustment(StatsHandler caster) => Mathf.Lerp(publicValues.mindForTrapsMin, publicValues.mindForTrapsMax, ConvertToPercentage(caster.Mind));

        public float CounterAdjustment(StatsHandler caster) => Mathf.Lerp(publicValues.mindForCounterMin, publicValues.mindForCounterMax, ConvertToPercentage(caster.Mind));

    }
}