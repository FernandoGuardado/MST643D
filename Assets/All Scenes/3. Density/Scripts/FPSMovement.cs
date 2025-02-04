﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSMovement: MonoBehaviour {

    public float speed = 10.0f;
    public float jumpStrength = 25.0f;
	public int fireRate = 1200;
    public float liftForce = 700;
    public float liftCooldown = 15.0f;
    public float dodgeCooldown = 300.0f;
    public float dodgeForce = 7000f;
    public float friction = 0.34f;
    public AudioClip liftSound;
    public AudioClip dodgeSound;
    public AudioClip landSound;

    private bool onGround = false;
    private bool lifting = false;
    private float _liftCooldown;
    private float _dodgeCooldown;
    private float distanceToGround;
    [SerializeField]
    private int initialLiftMultiplier = 3;

    private Vector3 boostRate = new Vector3(0, 0, 0);

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        _liftCooldown = liftCooldown;
        _dodgeCooldown = dodgeCooldown;
        dodgeCooldown = 0;
        liftCooldown *= 3;
        distanceToGround = GetComponent<Collider>().bounds.extents.y;
	}
	
	// Update is called once per frame
	void Update () {

    }

    // Called at a consistent point per frame
    void FixedUpdate() {
        onGround = IsGrounded();
        MovePlayer();
        Jump();
        Lift();
        Dodge();
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == 8) {
            lifting = false;
            GetComponent<AudioSource>().PlayOneShot(landSound);
        }
    }

    void OnCollisionStay(Collision col) {
        // Layer 8 = Ground
        if (col.gameObject.layer == 8) {
            lifting = false;
        }
    }

    void OnCollisionExit(Collision col) {
        // Layer 8 = Ground
        if (col.gameObject.layer == 8) {

        }
    }

    // Move player
    void MovePlayer() {
        // Movement strengths
        float forwardPos = Input.GetAxis("Vertical") * speed;
        float strafePos = Input.GetAxis("Horizontal") * speed;

        // Normalize
        strafePos *= Time.deltaTime;
        forwardPos *= Time.deltaTime;

        Vector3 movePosition = new Vector3(strafePos, 0, forwardPos);

        // Translate by strength
        GetComponent<Rigidbody>().MovePosition((transform.position + transform.TransformDirection(movePosition)));

        // Allow cursor to be freed on Esc press
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Jump() {
        if (onGround && Input.GetButtonDown("Jump")) {
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, jumpStrength, GetComponent<Rigidbody>().velocity.z);
        }
        if (!onGround && GetComponent<Rigidbody>().velocity.y < 0) {
            GetComponent<Rigidbody>().AddForce(Physics.gravity);
        }
    }

    void Dodge() {
        if (!onGround) {
            if (Input.GetButtonDown("Dodge") && dodgeCooldown == 0) {
                GetComponent<AudioSource>().PlayOneShot(dodgeSound);
                GetComponent<Rigidbody>().AddRelativeForce(new Vector3(dodgeForce * Input.GetAxis("Horizontal"), 0, dodgeForce * Input.GetAxis("Vertical")));
                dodgeCooldown = _dodgeCooldown;
            }
        }

        if (dodgeCooldown > 0) {
            dodgeCooldown--;
        }
    }

    void Lift() {
        if (!onGround) {
            if (Input.GetButton("Lift") && liftCooldown > 0) {
                if (Input.GetButtonDown("Lift")) {
                    if (liftCooldown <= 0) {
                        liftCooldown = 0;
                    }

                    if (!lifting) {
                        boostRate = new Vector3(1, 1, 1);
                        lifting = true;
                    }

                    boostRate.z = 1;
                    boostRate.y = 0;
                    boostRate.x = 1;
                    boostRate.z *= liftForce * initialLiftMultiplier * Input.GetAxis("Vertical");
                    boostRate.x *= liftForce * initialLiftMultiplier * Input.GetAxis("Horizontal");
                }
                else {
                    if (liftCooldown <= 0) {
                        liftCooldown = 0;
                    }

                    if (!lifting) {
                        boostRate = new Vector3(1, 1, 1);
                        lifting = true;
                    }
                    boostRate.z = 1;
                    boostRate.y = (-Physics.gravity.y) + jumpStrength / (jumpStrength / (1 + (liftCooldown / _liftCooldown)));
                    boostRate.x = 1;
                    boostRate.z *= liftForce * initialLiftMultiplier * liftCooldown / _liftCooldown * (Input.GetAxis("Vertical"));
                    boostRate.x *= liftForce * initialLiftMultiplier * liftCooldown / _liftCooldown * (Input.GetAxis("Horizontal"));
                }
                GetComponent<Rigidbody>().AddRelativeForce(boostRate);
                GetComponent<AudioSource>().PlayOneShot(liftSound);
                liftCooldown -= 15f * Time.deltaTime;
            }
            else {

            }
        }

        else if (onGround) {
            if (liftCooldown < _liftCooldown * 3) {
                liftCooldown += 10f * Time.deltaTime;
            }
            else if (liftCooldown > _liftCooldown * 3) {
                liftCooldown = _liftCooldown * 3;
            }
        }
    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }

    public float GetLiftFill() {
        return liftCooldown / (_liftCooldown * 3);
    }

    public float GetDodgeFill() {
        return 1 - (dodgeCooldown / _dodgeCooldown);
    }
}
