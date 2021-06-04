using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingStats buildingStats;

    private float currentHitPoints;
    private Transform selectedArea = null;

    private void Awake()
    {
        SetInitialHitpoints();

        if (buildingStats.buildingTeam == Team.PLAYER)
            selectedArea = transform.Find("Selected");
    }

    void Start()
    {
        GameManager.instance.activeBuildings.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.instance.activeBuildings.Remove(this);

        if (SelectionManager.instance.selectedBuilding == this)
            SelectionManager.instance.selectedBuilding = null;
    }

    public void SetSelected(bool isSelected)
    {
        if (buildingStats.buildingTeam != Team.PLAYER)  //For Player Units Only
            return;

        if (selectedArea != null)
            selectedArea.gameObject.SetActive(isSelected);
    }

    private void SetInitialHitpoints()
    {
        currentHitPoints = buildingStats.maxHitPoints;
    }

    public void Repair(float amount)
    {
        currentHitPoints += amount;
        Mathf.Clamp(currentHitPoints, 0f, buildingStats.maxHitPoints);
    }

    public float GetCurrentHitpoints()
    {
        return currentHitPoints;
    }

    public void SetCurrentHitpoints(float newHitpointsValue)
    {
        currentHitPoints = newHitpointsValue;
    }
}
