using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class BlackShield : MonoBehaviour
    {
        [SerializeField] List<float> healthPercentagePins = new List<float>();
        List<float> removeList = new List<float>();
        [SerializeField] StatsHandler statsHandler;
        MeshRenderer mesh;
        [SerializeField] bool activated = false;

        float _maxHealth = 0f;

        private void Start()
        {
            mesh = GetComponent<MeshRenderer>();
            statsHandler.OnDamageTaken += StatsHandler_OnDamageTaken;
            _maxHealth = statsHandler.MaxHealth;
        }

        private void StatsHandler_OnDamageTaken(object sender, System.EventArgs e)
        {
            CheckPercentagePins();
        }

        private void CheckPercentagePins()
        {
            _maxHealth = statsHandler.MaxHealth;

            foreach(float pin in healthPercentagePins)
            {
                if (statsHandler.Health < _maxHealth * pin)
                {
                    Activate();
                    removeList.Add(pin);
                }
            }

            RemovePins();
        }

        private void RemovePins()
        {
            foreach(float pin in removeList)
            {
                healthPercentagePins.Remove(pin);
            }

            removeList.Clear();
        }

        public void Activate()
        {
            activated = true;
            mesh.enabled = true;
        }

        public void ElectricLightUsed()
        {
            activated = false;
            mesh.enabled = false;
        }

        public bool GetActivated => activated;
    }
}