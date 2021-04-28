using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    FOOD,
    WOOD,
    GOLD,
    NONE
}

public enum ResourceRaw
{
    BERRIES,
    GOLD,
    WOOD,
    FISH,
    FARM
}

[System.Serializable]
public struct ResourceCost
{
    public int foodCost;
    public int woodCost;
    public int goldCost;
}

[System.Serializable]
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
                case ResourceType.FOOD:
                    newFoodAmount += resourceCamp.amountStored;
                    break;
                case ResourceType.GOLD:
                    newGoldAmount += resourceCamp.amountStored;
                    break;
                case ResourceType.WOOD:
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
            case ResourceRaw.BERRIES:
            case ResourceRaw.FARM:
            case ResourceRaw.FISH:
                return ResourceType.FOOD;
            case ResourceRaw.GOLD:
                return ResourceType.GOLD;
            case ResourceRaw.WOOD:
                return ResourceType.WOOD;
        }
        return ResourceType.NONE;
    }
}
