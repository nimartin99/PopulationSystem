using UnityEngine;

public interface IBuilding {
    void BuildingPlaced();
    void ResidentEnter(Collider other);
    void ResidentLeave(Collider other);
}
