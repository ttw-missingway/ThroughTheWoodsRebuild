using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.World
{
    [CreateAssetMenu(fileName = "New DialogBranch", menuName = "DialogBranch")]
    public class DialogBranch : ScriptableObject
    {
        public string branchKeyword;
        public List<DialogLine> lines = new List<DialogLine>();

        [System.Serializable]
        public struct DialogLine
        {
            public string speaker;
            public string text;
            public bool newLine;
            //public textFX textFX;
            public List<DialogChoice> choices;
        }

        [System.Serializable]
        public struct DialogChoice
        {
            public string text;
            public DialogBranch newBranch;
        }

        public List<string> GetChoicesAsStrings(int index)
        {
            List<string> choices = new List<string>();

            foreach (DialogChoice dialog in lines[index].choices)
            {
                choices.Add(dialog.text);
            }

            return choices;
        }
    }
}