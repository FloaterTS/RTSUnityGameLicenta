using UnityEngine;

[CreateAssetMenu]
public class BuildingStats : ScriptableObject
{
    public Team buildingTeam;
    public string buildingName;
    public string toolConstructionName;
    public int maxHitPoints;
    public float constructionTime;
    public ResourceCost buildingCost;

    [SerializeField] private BuildingStats baseBuildingStats = null;

    public void ResetStats()
    {
        if (baseBuildingStats == null)
            return;

        buildingName = baseBuildingStats.buildingName;
        toolConstructionName = baseBuildingStats.toolConstructionName;
        maxHitPoints = baseBuildingStats.maxHitPoints;
        constructionTime = baseBuildingStats.constructionTime;

        buildingCost.foodCost = baseBuildingStats.buildingCost.foodCost;
        buildingCost.woodCost = baseBuildingStats.buildingCost.woodCost;
        buildingCost.goldCost = baseBuildingStats.buildingCost.goldCost;
    }
}
