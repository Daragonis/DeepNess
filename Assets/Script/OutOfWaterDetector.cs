using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfWaterDetector : MonoBehaviour {

    [HideInInspector] public bool inWater = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Water>() != null) {
            inWater = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<Water>() != null) {
            inWater = false;
        }
    }
}
