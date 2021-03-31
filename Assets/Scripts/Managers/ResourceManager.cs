using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Food,
    Wood,
    Gold,
    None
}

public enum ResourceRaw
{
    Berries,
    Gold,
    Wood,
    Fish,
    Farm
}

[System.Serializable]
public struct ResourceCost
{
    public int foodCost;
    public int woodCost;
    public int goldCost;
}

public struct CarriedResource
{
    public int amount;
    public ResourceInfo resourceInfo;
}

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    [HideInInspector] public int currentFoodAmount;
    [HideInInspector] public int currentGoldAmount;
    [HideInInspector] public int currentWoodAmount;

    [HideInInspector] public List<ResourceCamp> resourceCamps;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another resource manager present.");

        resourceCamps = new List<ResourceCamp>();
    }

    void Update()
    {
        GetResourceAmount();
    }

    void GetResourceAmount()
    {
        int newFoodAmount = 0;
        int newGoldAmount = 0;
        int newWoodAmount = 0;
        foreach (ResourceCamp resourceCamp in resourceCamps)
        {
            switch (resourceCamp.campType)
            {
                case ResourceType.Food:
                    newFoodAmount += resourceCamp.amountStored;
                    break;
                case ResourceType.Gold:
                    newGoldAmount += resourceCamp.amountStored;
                    break;
                case ResourceType.Wood:
                    newWoodAmount += resourceCamp.amountStored;
                    break;
            }
        }
        currentFoodAmount = newFoodAmount;
        currentGoldAmount = newGoldAmount;
        currentWoodAmount = newWoodAmount;
    }

    public static ResourceType ResourceRawToType(ResourceRaw resourceRaw)
    {
        switch (resourceRaw)
        {
            case ResourceRaw.Berries:
            case ResourceRaw.Farm:
            case ResourceRaw.Fish:
                return ResourceType.Food;
            case ResourceRaw.Gold:
                return ResourceType.Gold;
            case ResourceRaw.Wood:
                return ResourceType.Wood;
        }
        return ResourceType.None;
    }
}
