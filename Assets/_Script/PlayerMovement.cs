using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] 
    private Rigidbody2D rb;

    [Range(0f, 20f)] public int speed;

    private float horizontal;
    private float vertical;

    private void Start()
    {
        var cam = GameObject.FindWithTag("Cam").GetComponent<CinemachineVirtualCamera>();
        cam.Follow = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {

        if (horizontal < -0.1f || horizontal > 0.1f || vertical < -0.1f || vertical > 0.1f)
        {
            rb.AddForce(new Vector2(horizontal, vertical) * speed, ForceMode2D.Impulse);
        }
    }
}
