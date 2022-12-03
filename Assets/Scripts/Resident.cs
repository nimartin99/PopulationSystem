using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Resident : MonoBehaviour {
    [SerializeField] public List<Transform> tasks;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    private House _home;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (tasks.Count == 0) {
            _navMeshAgent.destination = _home.entrance.position;
        }
    }

    public void ResidentConstructor(House home) {
        _home = home;
    }

    public void AddTask(Transform destinationTransform) {
        tasks.Add(destinationTransform);
        _navMeshAgent.destination = tasks[0].position;
    }

    public void RemoveTask(int index) {
        tasks.RemoveAt(index);
    }

    public void CompleteTask(int index) {
        tasks.RemoveAt(index);
    }

    public void DisableVisual() {
        skinnedMeshRenderer.enabled = false;
    }
    
    public void EnableVisual() {
        skinnedMeshRenderer.enabled = true;
    }
}
