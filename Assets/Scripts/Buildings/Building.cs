using UnityEngine;

public interface IBuilding {
    void BuildingPlaced();
    void BuildingDestroyed();
    void ResidentEnter(Resident resident);
    void ResidentLeave(Resident resident);
}
