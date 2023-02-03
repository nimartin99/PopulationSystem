using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Riot : MonoBehaviour {
    [SerializeField] private Transform protestIndicator;
    [SerializeField] private Transform _protestingCircle;

    private List<Resident> currentlyProtestingResidents = new List<Resident>();
    public bool _movingProtest = false;
    
    [SerializeField] private Transform rioter;
    [SerializeField] private Transform riotTargetPrefab;
    private Transform _moveTarget;
    public List<Transform> riotingResidents = new List<Transform>();

    public string riotStatus = "Riot intensifies...";
    public float riotProgress = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        protestIndicator.Rotate(0, 30 * Time.deltaTime, 0);
        if (!_movingProtest) {
            transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
    }

    private void OnTriggerEnter(Collider other) {
        Resident resident = other.gameObject.GetComponent<Resident>();
        RiotingResident riotingResident = other.gameObject.GetComponent<RiotingResident>();
        if (resident && resident.currentTask == Resident.AvailableTasks.Protest) {
            resident.DisableResident();
            currentlyProtestingResidents.Add(resident);
            if (currentlyProtestingResidents.Count == 4) {
                _protestingCircle.GetChild(0).gameObject.SetActive(false);
                _protestingCircle.GetChild(1).gameObject.SetActive(false);
                _protestingCircle.GetChild(2).gameObject.SetActive(false);
                for (int i = 0; i < 4; i++) {
                    AddNewProtesterToMoving();
                }
                protestIndicator.parent = riotingResidents[0];
                _movingProtest = true;
                _moveTarget = Instantiate(riotTargetPrefab, GetNextPosition(), Quaternion.identity, transform);
                foreach (Transform rioterTransform in riotingResidents) {
                    Vector2 destinationVariation = new Vector2(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
                    rioterTransform.GetComponent<NavMeshAgent>().destination = 
                        new Vector3(_moveTarget.position.x + destinationVariation.x, _moveTarget.position.y, _moveTarget.position.z + destinationVariation.y);
                }
            } else if(currentlyProtestingResidents.Count < 4) {
                _protestingCircle.GetChild(currentlyProtestingResidents.Count - 1).gameObject.SetActive(true);
                _protestingCircle.GetChild(currentlyProtestingResidents.Count - 1).GetComponent<Animator>()
                    .SetBool("Protesting", true);
                protestIndicator.gameObject.SetActive(true);
            } else {
                AddNewProtesterToMoving();
            }
        } else if (riotingResident) {
            Vector2 destinationVariation = new Vector2(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
            riotingResident.GetComponent<NavMeshAgent>().destination = 
                    new Vector3(_moveTarget.position.x + destinationVariation.x, _moveTarget.position.y, _moveTarget.position.z + destinationVariation.y);
        }
    }

    private Vector3 GetNextPosition() {
        RaycastHit[] hitsAround = Physics.SphereCastAll(transform.position, 15f, transform.up);
        RaycastHit pathFurthestAway = hitsAround[0];
        foreach (RaycastHit hit in hitsAround) {
            if (hit.transform.CompareTag("Path") && hit.distance > pathFurthestAway.distance && IsPathAvailable(hit.transform.position)) {
                pathFurthestAway = hit;
            }
        }
        Vector3 pathPosition = pathFurthestAway.transform.position;
        switch (pathFurthestAway.transform.eulerAngles.y) {
            case 0:
            default:
                return new Vector3(pathPosition.x + 0.5f, pathPosition.y + 0.5f, pathPosition.z + 0.5f);
            case 90:
                return new Vector3(pathPosition.x + 0.5f, pathPosition.y + 0.5f, pathPosition.z - 0.5f);
            case 180:
                return new Vector3(pathPosition.x - 0.5f, pathPosition.y + 0.5f, pathPosition.z - 0.5f);
            case 270:
                return new Vector3(pathPosition.x - 0.5f, pathPosition.y + 0.5f, pathPosition.z + 0.5f);
        }
    }

    private bool IsPathAvailable(Vector3 destination) {
        NavMeshPath Path = new NavMeshPath();
        NavMeshAgent anyResidentAgent = riotingResidents[0].GetComponent<NavMeshAgent>();
        NavMesh.CalculatePath(transform.position, destination, anyResidentAgent.areaMask, Path);
        return Path.status == NavMeshPathStatus.PathComplete;
    }

    private void AddNewProtesterToMoving() {
        Transform newRiotingResident = Instantiate(rioter, transform.position,
            Quaternion.identity, transform);
        riotingResidents.Add(newRiotingResident);
        foreach (Transform riotingResident in riotingResidents) {
            riotingResident.GetComponent<RiotingResident>().riotRange = riotingResidents.Count;
        }
        StartCoroutine(StartAsyncAnimation(newRiotingResident.GetChild(0)));
    }

    private IEnumerator StartAsyncAnimation(Transform riotingResident) {
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        riotingResident.GetComponent<Animator>().SetBool("Protesting", true);
    }
}
