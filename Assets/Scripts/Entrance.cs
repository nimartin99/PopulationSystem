using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Resident")) {
            transform.parent.GetComponent<IBuilding>().ResidentEnter(other.GetComponent<Resident>());
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Resident")) {
            transform.parent.GetComponent<IBuilding>().ResidentLeave(other.GetComponent<Resident>());
        }
    }
}
