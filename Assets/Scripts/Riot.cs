using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riot : MonoBehaviour {
    [SerializeField] private Transform _protestIndicator;
    [SerializeField] private Transform _protestingCircle;

    private List<Resident> currentlyProtestingResidents = new List<Resident>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(0, 30 * Time.deltaTime, 0);
        _protestIndicator.Rotate(0, 30 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("OnTriggerEnter");
        Resident resident = other.gameObject.GetComponent<Resident>();
        if (resident && resident.currentTask == Resident.AvailableTasks.Protest) {
            resident.DisableVisual();
            _protestingCircle.GetChild(currentlyProtestingResidents.Count).gameObject.SetActive(true);
            _protestingCircle.GetChild(currentlyProtestingResidents.Count).GetComponent<Animator>()
                .SetBool("Protesting", true);
            currentlyProtestingResidents.Add(resident);
            _protestIndicator.gameObject.SetActive(true);
        }
    }
}
