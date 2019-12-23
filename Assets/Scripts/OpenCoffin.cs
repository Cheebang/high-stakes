﻿using UnityEngine;

public class OpenCoffin : MonoBehaviour {
    public bool open = false;
    public GameObject lid;
    public GameObject minion;

    public void OnOpen () {
        open = true;
        lid.SetActive (false);

        if (minion != null) {
            minion.GetComponent<MinionController> ().WakeUp ();
        }
    }
}