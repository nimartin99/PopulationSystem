using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiotingResident : MonoBehaviour {
    public float riotRange = 1f;
    private SphereCollider _sphereCollider;
    private void Start() {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update() {
        _sphereCollider.radius = riotRange;
    }

    private void OnTriggerEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident && !resident.rioting) {
            float satisfactionCap =
                Math.Clamp(30f + 2f * transform.parent.GetComponent<Riot>().residents.Count, 30f, 50f);
            if (resident && resident.currentTask != Resident.AvailableTasks.Riot 
                         && resident._overallSatisfactionOverTime < satisfactionCap) {
                resident.rioting = true;
            }
        }
    }
}
