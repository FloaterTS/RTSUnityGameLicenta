using UnityEngine;

[CreateAssetMenu]
public class BuildingStats : ScriptableObject
{
    public Team buildingTeam;
    public string buildingName;
    public int maxHitPoints;
    public float constructionTime;

    [SerializeField] private BuildingStats baseBuildingStats = null;

    public void ResetStats()
    {
        if (baseBuildingStats == null)
            return;
        buildingName = baseBuildingStats.buildingName;
        maxHitPoints = baseBuildingStats.maxHitPoints;
        constructionTime = baseBuildingStats.constructionTime;
    }
}
