using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingStats buildingStats;

    private float currentHitPoints;
    private Transform selectedArea = null;

    void Start()
    {
        SetInitialHitpoints(0.1f);

        if (buildingStats.buildingTeam == Team.Player)
            selectedArea = transform.Find("Selected");

        GameManager.instance.activeBuildings.Add(this);
    }

    public void SetSelected(bool isSelected)
    {
        if (buildingStats.buildingTeam != Team.Player)  //For Player Units Only
            return;

        if (selectedArea != null)
            selectedArea.gameObject.SetActive(isSelected);
    }

    public void SetInitialHitpoints(float amount)
    {
        currentHitPoints = amount;
    }

    public float GetCurrentHitpoints()
    {
        return currentHitPoints;
    }

    public void Repair(float amount)
    {
        currentHitPoints += amount;
        Mathf.Clamp(currentHitPoints, 0f, buildingStats.maxHitPoints);
    }

}
