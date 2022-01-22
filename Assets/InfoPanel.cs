using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour, IUIElement
{
    TMP_Text text;
    [SerializeField] string displayText;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void HideUI()
    {
        GetComponentInParent<Image>().enabled = false;
        text.enabled = false;
    }

    public void RevealUI()
    {
        GetComponentInParent<Image>().enabled = true;
        text.enabled = true;
    }

    public void ReadInfo(string info)
    {
        displayText = info;
        text.text = displayText;
    }
}
