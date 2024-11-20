using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetButton("Horizontal"))
            HorizontalMove();
        if(Input.GetButton("Vertical"))
            VerticalMove();
    }

    private void HorizontalMove()
    {
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed*Time.deltaTime);
    }

    private void VerticalMove()
    {
        Vector3 dir = transform.up * Input.GetAxis("Vertical");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed*Time.deltaTime);
    }
}
