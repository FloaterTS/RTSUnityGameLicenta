using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    [Header("Units Prefabs")]
    public GameObject villagerPrefab;

    [Header("Buildings Prefabs")]
    public GameObject resourceCampPrefab;
    public GameObject resourceCampConstructionPrefab;

    [Header("ResourceFields Prefabs")]
    public GameObject berryBushSmallPrefab;
    public GameObject berryBushLargePrefab;
    public GameObject lumberTreePrefab;
    public GameObject goldOreMinePrefab;

    [Header("Resource Drops Pefabs")]
    public GameObject berriesDropPrefab;
    public GameObject logPileDropPrefab;
    public GameObject goldOreDropPrefab;

    [Space] 
    [Header("Units Stats")]
    public UnitStats playerVillagerStats;

    public UnitStats enemyVillagerStats;

    [Header("Buildings Stats")]
    public BuildingStats playerResourceCamp;

    [Header("Resource Info")]
    public ResourceInfo berriesInfo;
    public ResourceInfo woodInfo;
    public ResourceInfo goldInfo;

    [Space][Header("Transform Parents")]
    public GameObject unitsTransformParentGO;
    public GameObject buildingsTransformParentGO;
    public GameObject resourceFieldsTransformParentGO;
    public GameObject resourceDropsTransformParentGO;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another prefab manager present.");
    }
}
