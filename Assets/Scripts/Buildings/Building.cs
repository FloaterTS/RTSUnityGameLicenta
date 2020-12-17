using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingStats buildingStats;

    private int currentHitPoints;

    private GameObject selectedArea = null;

    void Start()
    {
        currentHitPoints = buildingStats.maxHitPoints;

        if (buildingStats.buildingTeam == Team.Player)
            selectedArea = transform.Find("Selected").gameObject;
    }

    public void SetSelected(bool isSelected)
    {
        if (buildingStats.buildingTeam != Team.Player)  //For Player Units Only
            return;

        if (selectedArea != null)
            selectedArea.SetActive(isSelected);
    }
}
