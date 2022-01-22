using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneIcon : MonoBehaviour, IUIElement
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void HideUI()
    {
        image.enabled = true;
    }

    public void RevealUI()
    {
        image.enabled = false;
    }
}
