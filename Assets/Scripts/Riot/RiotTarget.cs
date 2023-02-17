using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RiotTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        RiotingResident riotingResident = other.transform.GetComponent<RiotingResident>();
        if (riotingResident) {
            riotingResident.GetComponent<NavMeshAgent>().destination = transform.parent.position;
        }
    }
}
