using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TTW.Combat
{
    public class ActorProfile : MonoBehaviour, IUIElement
    {
        ActorEntity currentProfile = null;
        Sprite portrait = null;
        Image image = null;
        string actorName = null;
        [SerializeField] HealthBar healthBar;
        [SerializeField] TargetIcon targetIconShell;
        [SerializeField] TMP_Text actorNameText;
        [SerializeField] List<TargetIcon> activeTargetIcons = new List<TargetIcon>();
        Vector3 origin;

        private void Awake()
        {
            image = GetComponent<Image>();
            origin = GetComponent<RectTransform>().position;
        }

        public void UpdateProfile(ActorEntity newProfile)
        {
            currentProfile = newProfile;
            portrait = currentProfile.Actor.portrait;
            actorName = currentProfile.Actor.name;
            var health = currentProfile.GetComponent<StatsHandler>().Health;
            var maxHealth = currentProfile.GetComponent<StatsHandler>().MaxHealth;

            image.sprite = portrait;
            healthBar.UpdateHealth(health, maxHealth);
            actorNameText.text = actorName;
        }

        public void AddTargetIcons(List<Targetable> targetables)
        {
            ClearTargetIcons();

            for (int i = 0; i < targetables.Count; i++)
            {
                Sprite newSprite;
                Vector3 offset = new Vector3(120f * i, 264f, 0f);
                TargetIcon newTarget = Instantiate(targetIconShell, transform);
                newTarget.GetComponent<RectTransform>().position = transform.position + offset;

                switch (targetables[i].GetTargetClass())
                {
                    case TargetClass.Actor:
                        newSprite = targetables[i].GetComponent<ActorEntity>().Actor.portrait;
                        break;
                    case TargetClass.Boss:
                        newSprite = targetables[i].GetComponent<BossEntity>().Boss.combatIcon;
                        break;
                    case TargetClass.Enemy:
                        newSprite = targetables[i].GetComponent<EnemyEntity>().enemyType.combatIcon;
                        break;
                    default:
                        // add cases for cell and obstacle
                        newSprite = null;
                        break;
                }

                newTarget.LoadStats(newSprite, targetables[i].GetComponent<StatsHandler>());
                activeTargetIcons.Add(newTarget);
            }
        }

        public void ClearTargetIcons()
        {
            foreach(TargetIcon t in activeTargetIcons)
            {
                Destroy(t.gameObject);
            }

            activeTargetIcons.Clear();
        }

        public void HideUI()
        {
            LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector2(200f, 0f), 0.4f).setEaseInOutSine().setOnComplete(Disable);
        }

        public void Disable()
        {
            GetComponent<Image>().enabled = false;
            var childrenImg = GetComponentsInChildren<Image>();
            var childrenTxt = GetComponentsInChildren<TMP_Text>();
            foreach (Image i in childrenImg)
            {
                i.enabled = false;
            }
            foreach (TMP_Text t in childrenTxt)
            {
                t.enabled = false;
            }
        }

        public void RevealUI()
        {
            LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector2(0f, 0f), 0.4f).setEaseInOutSine();
            GetComponent<Image>().enabled = true;
            var childrenImg = GetComponentsInChildren<Image>();
            var childrenTxt = GetComponentsInChildren<TMP_Text>();
            foreach (Image i in childrenImg)
            {
                i.enabled = true;
            }
            foreach (TMP_Text t in childrenTxt)
            {
                t.enabled = true;
            }
        }
    }
}