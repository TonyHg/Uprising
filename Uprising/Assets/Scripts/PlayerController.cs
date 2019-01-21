﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // This script is to be attached to the player. It tests the collision with a item.
    public Rigidbody rb;
    public float speed = 1;
    private GameObject model;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        model = rb.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            this.transform.position += this.transform.forward * speed * Time.fixedDeltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += Vector3.back * (speed/2) * Time.fixedDeltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.position += Vector3.left * (speed / 2) * Time.fixedDeltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += Vector3.right * (speed / 2) * Time.fixedDeltaTime;
        }

        if(Input.GetKey(KeyCode.A))
        {
            Quaternion deltaRot = Quaternion.Euler(new Vector3(0, 50, 0) * Time.deltaTime);
            rb.MoveRotation(deltaRot * rb.rotation);
            //   this.transform.rotation += 
        }

        if (Input.GetKey(KeyCode.E))
        {
            Quaternion deltaRot = Quaternion.Euler(new Vector3(0, -50, 0) * Time.deltaTime);
            rb.MoveRotation(deltaRot * rb.rotation);
            //   this.transform.rotation += 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("item"))
        {
            // other.gameObject.SetActive(false);
            other.gameObject.SendMessage("Collect", rb.gameObject);
        }
    }
}
