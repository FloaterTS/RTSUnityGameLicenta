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

[System.Serializable]
public struct CarriedResource
{
    public int amount;
    public ResourceInfo resourceInfo;
}

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public int initialFoodAmount = 0;
    public int initialWoodAmount = 0;
    public int initialGoldAmount = 0;

    [HideInInspector] public int currentFoodAmount;
    [HideInInspector] public int currentWoodAmount;
    [HideInInspector] public int currentGoldAmount;

    [HideInInspector] public List<ResourceCamp> resourceCamps;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another resource manager present.");

        resourceCamps = new List<ResourceCamp>();

        currentFoodAmount = initialFoodAmount;
        currentWoodAmount = initialWoodAmount;
        currentGoldAmount = initialGoldAmount;
    }

    private void Update()
    {
        //GetResourceAmount();
    }

    private void GetResourceAmount()
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

    public void AddResources(int amount, ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.FOOD:
                currentFoodAmount += amount;
                break;
            case ResourceType.WOOD:
                currentWoodAmount += amount;
                break;
            case ResourceType.GOLD:
                currentGoldAmount += amount;
                break;
        }
    }

    public bool UseResources(ResourceCost resourceCost, bool check)
    {
        if (currentFoodAmount >= resourceCost.foodCost)
        {
            if (currentWoodAmount >= resourceCost.woodCost)
            {
                if (currentGoldAmount >= resourceCost.goldCost)
                {
                    if (!check)
                    {
                        currentFoodAmount -= resourceCost.foodCost;
                        currentWoodAmount -= resourceCost.woodCost;
                        currentGoldAmount -= resourceCost.goldCost;
                    }
                    return true;
                }
                else
                    UIManager.instance.ShowScreenAlert("Not enough gold...");
            }
            else
                UIManager.instance.ShowScreenAlert("Not enough wood...");
        }
        else
            UIManager.instance.ShowScreenAlert("Not enough food...");

        return false;
    }

    public static ResourceType ResourceRawToType(ResourceRaw resourceRaw)
    {
        switch (resourceRaw)
        {
            case ResourceRaw.BERRIES:
            case ResourceRaw.Farm:
            case ResourceRaw.Fish:
                return ResourceType.FOOD;
            case ResourceRaw.GOLD:
                return ResourceType.GOLD;
            case ResourceRaw.WOOD:
                return ResourceType.WOOD;
        }
        return ResourceType.NONE;
    }

}
