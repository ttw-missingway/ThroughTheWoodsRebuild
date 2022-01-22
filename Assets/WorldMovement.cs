using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMovement : MonoBehaviour
{
    public float walkSpeed = 8f;
    public float jumpSpeed = 7f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        WalkHandler();
    }

    void WalkHandler()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        float distance = walkSpeed * Time.deltaTime;
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(hAxis * distance, 0f, vAxis * distance);
        Vector3 currPosition = transform.position;
        Vector3 newPosition = currPosition + movement;
        rb.MovePosition(newPosition);
    }
}
