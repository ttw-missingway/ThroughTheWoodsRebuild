using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TTW.Combat
{
    public class BenchText : UIText
    {
        ActorEntity assignedBenchActor;

        public void AssignAbility(ActorEntity actor)
        {
            assignedBenchActor = actor;
            SetText(assignedBenchActor.name);
        }
    }
}
