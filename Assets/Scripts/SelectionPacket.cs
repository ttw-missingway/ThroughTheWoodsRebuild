using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public class SelectionPacket 
    {
        public SelectionState SelectionState { get; set; }
        public Ability HighlightedAbility { get; set; }
        public Cell HighlightedCell { get; set; }
        public List<Targetable> HighlightedTargets { get; set; }

        public SelectionPacket(SelectionState state, Ability ability, Cell cell, List<Targetable> targets)
        {
            SelectionState = state;
            HighlightedCell = cell;
            HighlightedAbility = ability;
            HighlightedTargets = targets;
        }
    }
}