using UnityEngine;

public interface IBuilding {
    void BuildingPlaced();
    void BuildingDestroyed();
    void ResidentEnter(Collider other);
    void ResidentLeave(Collider other);
}
