using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TTW.Combat
{
    public class AbilityLighting : MonoBehaviour
    {
        private PlayableDirector _lightStage;
        private PlayableDirector _lightScene;
        private PlayableDirector _lightBackground;

        [SerializeField] List<TimelineAsset> fadeInTimelines;
        [SerializeField] List<TimelineAsset> fadeOutTimelines;

        private void Awake()
        {
            _lightStage = GameObject.FindGameObjectWithTag("stageLight").GetComponent<PlayableDirector>();
            _lightScene = GameObject.FindGameObjectWithTag("sceneLight").GetComponent<PlayableDirector>();
            _lightBackground = GameObject.FindGameObjectWithTag("backgroundLight").GetComponent<PlayableDirector>();
        }

        public void DimTheLights()
        {
            _lightStage.Play(fadeOutTimelines[0]);
            _lightScene.Play(fadeOutTimelines[1]);
            _lightBackground.Play(fadeOutTimelines[2]);
        }

        public void LightsOn()
        {
            _lightStage.Play(fadeInTimelines[0]);
            _lightScene.Play(fadeInTimelines[1]);
            _lightBackground.Play(fadeInTimelines[2]);
        }
    }
}
