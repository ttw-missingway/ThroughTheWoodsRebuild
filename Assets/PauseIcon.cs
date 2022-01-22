using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseIcon : MonoBehaviour, IUIElement
{
    Image image;
    bool menuOpen = false;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void HideUI()
    {
        image.enabled = false;
    }

    public void RevealUI()
    {
        if (!menuOpen) return;

        image.enabled = true;
    }

    public void SetMenuOpen(bool open)
    {
        menuOpen = open;
        image.enabled = open;
    }
}
