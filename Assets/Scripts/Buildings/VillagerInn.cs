using UnityEngine;

public class VillagerInn : MonoBehaviour
{
    private Building building;
    private bool spawning = false;
    private float spawningIn;

    void Awake()
    {
        building = GetComponent<Building>();
    }

    void Update()
    {
        if (spawning)
            spawningIn -= Time.deltaTime;
    }

    public bool SpawnVillager()
    {
        return true;
    }

    public bool IsSpawning()
    {
        return spawning;
    }
}
