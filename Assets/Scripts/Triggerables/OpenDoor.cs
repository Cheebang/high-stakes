﻿using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : Triggerable {
    public string keyRequired = null;
    public int speed = 1;
    public Vector3 targetPos;
    public bool locked = false;

    public float doorOpenAngle = 270f;
    public float doorCloseAngle = 0f;
    public float smooth = 2f;
    public bool open = false;

    public enum OPTIONS {
        Rotate = 0,
        Slide = 1
    }

    public OPTIONS openOption = OPTIONS.Rotate;

    private Vector3 startPos;
    private HUDController hud;

    private void Start () {
        startPos = transform.position;
        targetPos = new Vector3 (transform.position.x, transform.position.y + 3.5f, transform.position.z);
    }

    void Update () {
        if (hud == null) {
            hud = FindObjectOfType<HUDController> ();
        }

        switch (openOption) {
            case OPTIONS.Rotate:
            default:
                if (open) {
                    Quaternion targetRotation = Quaternion.Euler (0, doorOpenAngle, 0);
                    transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation, smooth * Time.deltaTime);
                } else {
                    Quaternion targetRotation = Quaternion.Euler (0, doorCloseAngle, 0);
                    transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation, smooth * Time.deltaTime);
                }
                break;
            case OPTIONS.Slide:
                if (open) {
                    transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * speed);
                } else {
                    transform.position = Vector3.Lerp (transform.position, startPos, Time.deltaTime * speed);
                }
                break;
        }
    }

    public override void Interact () {
        AttemptOpen ();
    }

    public override string GetInteractionMessage () {
        return open ? "Press E to close" : "Press E to open";
    }

    private void AttemptOpen () {
        if (locked) {
            hud.Log ("door is locked");
            return;
        }

        bool doorRequiresKey = !string.IsNullOrEmpty (keyRequired);
        if (doorRequiresKey) {
            List<string> items = FindObjectOfType<ItemManager> ().items;
            if (!items.Contains (keyRequired)) {
                hud.Log ("door requires " + keyRequired);
                return;
            }
            hud.Log ("door opened with " + keyRequired);
        }

        SwitchState ();
    }

    public void SwitchState () {
        open = !open;
    }
}