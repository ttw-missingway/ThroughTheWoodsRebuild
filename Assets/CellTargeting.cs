using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTargeting : MonoBehaviour, IUIElement
{
    bool isActive = false;
    MeshRenderer mesh;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    public void HideUI()
    {
        if (mesh.enabled)
        {
            isActive = true;
            mesh.enabled = false;
        }
    }

    public void RevealUI()
    {
        if (isActive)
        {
            isActive = false;
            mesh.enabled = true;
        }
    }

    public void SetIsActive(bool active)
    {
        isActive = active;
    }
}
