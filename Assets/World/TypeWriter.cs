using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace TTW.World
{
	public class TypeWriter : MonoBehaviour
	{
		[SerializeField] DialogBranch testBranch;

		TMP_Text _txt;
		string _loadedLine;
		string _loadedSpeaker;
		int _lineIndex;
		bool _lineComplete = false;
		DialogBranch _loadedBranch;

        private void Awake()
        {
			_txt = GetComponent<TMP_Text>();
		}

        private void Start()
        {
			LoadBranch(testBranch);
        }

        private void Update()
        {
            AButton();
        }

        void AButton()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StopAllCoroutines();

                if (_lineComplete)
                {
                    GoToNextLine();
                }
                else
                {
                    _lineComplete = true;
                    _txt.text = _loadedSpeaker + ":" + System.Environment.NewLine;
                    _txt.text += _loadedLine;
                }
            }
        }

        void TypeText()
		{
			_txt.text = _loadedSpeaker + ":" + System.Environment.NewLine;

			StartCoroutine("PlayText");
		}

		public void LoadBranch(DialogBranch branch)
        {
			_lineIndex = 0;
			_loadedBranch = branch;
			_loadedLine = _loadedBranch.lines[0].text;
			_loadedSpeaker = _loadedBranch.lines[0].speaker;

			TypeText();
		}

		IEnumerator PlayText()
        {
            _lineComplete = false;

            foreach (char c in _loadedLine)
            {
                _txt.text += c;
                yield return new WaitForSeconds(0.05f);
            }

            LoadChoices();

            _lineComplete = true;
        }

        private void LoadChoices()
        {
            if (_loadedBranch.lines[_lineIndex].choices.Count > 0)
            {
                List<string> choices = _loadedBranch.GetChoicesAsStrings(_lineIndex);

                for (int i = 0; i < choices.Count; i++)
                {
                    _txt.text += System.Environment.NewLine + choices[i];
                }
            }
        }

        private void GoToNextLine()
        {
            if (_loadedBranch.lines.Count - 1 > _lineIndex)
            {
				_lineIndex++;
				_loadedLine = _loadedBranch.lines[_lineIndex].text;
				_loadedSpeaker = _loadedBranch.lines[_lineIndex].speaker;

				TypeText();
            }
            else
            {
				EndOfBranch();
            }
        }

        void EndOfBranch()
        {
			_txt.text = "";
        }
    }
}