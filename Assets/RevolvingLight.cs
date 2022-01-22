using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolvingLight : MonoBehaviour
{

    Light _light;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        _light.transform.Rotate(new Vector3(0.1f, 0f, 0f), Space.Self);
    }
}
