using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] Cell startCell;
        [SerializeField] Ability trapAbility;
        [SerializeField] AttackExecutor _parent;

        StatModifierTool statModifierTool;

        private void Awake()
        {
            statModifierTool = new StatModifierTool(FindObjectOfType<PublicValues>());
        }

        public void Materialize(Cell cell)
        {
            var yOffset = new Vector3(0f, 10f, 0f);
            transform.position = cell.transform.position + yOffset;
            cell.SetTrap(this);
        }

        public void Activate(AttackReceiver target)
        {
            AttackPacket trapPacket = new AttackPacket(trapAbility, _parent);

            float randomRoll = UnityEngine.Random.Range(0, 100f);
            float successRate = trapAbility.trapSuccessRate + statModifierTool.TrapAdjustment(_parent.GetComponent<StatsHandler>());

            if (randomRoll > successRate)
            {
                print("Trap failed!");
                Destroy(gameObject);
                return;
            }

            target.ReceiveAttackPacket(trapPacket);

            Destroy(gameObject);
        }

        public void SetAbility(Ability ability)
        {
            trapAbility = ability;
        }

        public void SetParent(AttackExecutor parent)
        {
            _parent = parent;
        }
    }
}