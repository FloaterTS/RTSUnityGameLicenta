using UnityEngine;

public class VillagerInn : MonoBehaviour
{
    private Building building;

    private float spawnOffset = 4f;
    private float walkOffsetMin = 1.5f;
    private float walkOffsetMax = 4f;

    private float spawningIn;
    private bool spawning = false;
    
    private ResourceCost villagerCost;


    void Awake()
    {
        building = GetComponent<Building>();
    }

    private void Start()
    {
        Unit villagerUnit;
        villagerUnit = PrefabManager.instance.villagerPlayerPrefab.GetComponent<Unit>();
        if (villagerUnit != null)
            villagerCost = villagerUnit.unitStats.unitCost;
        else
            Debug.LogError("Villager prefab doesn't have Unit script attached");
    }

    void Update()
    {
        if (spawning)
            spawningIn -= Time.deltaTime;
    }

    public void RecruitVillager()
    {
        if (ResourceManager.instance.UseResources(villagerCost))
        {
            GameObject spawnedVillager = Instantiate(PrefabManager.instance.villagerPlayerPrefab, transform.position + transform.forward * spawnOffset, Quaternion.identity, PrefabManager.instance.unitsTransformParentGO.transform);
            spawnedVillager.GetComponent<Unit>().MoveToLocation(spawnedVillager.transform.position + transform.forward * Random.Range(walkOffsetMin, walkOffsetMax));
        }
    }

    public bool IsSpawning()
    {
        return spawning;
    }
}
