using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Teams classification:
//0 = PlayerTeam
//1 = Enemy

//Unit types classification:
//0 = Villager

//Building types classification:
//0 = Resource Camp Construction
//1 = Resource Camp

//Resource types classification:
//0 = None
//1 = Food / Berries
//2 = Wood
//3 = Gold

[System.Serializable]
public class UnitData
{
    public float[] unitPosition;
    public float[] unitRotation;
    public int unitTeam;
    public int unitType;
    public int unitResourceAmountCarried;
    public int unitResourceTypeCarried;

    public UnitData(Unit unit)
    {
        unitPosition = new float[3];
        unitRotation = new float[3];

        unitPosition[0] = unit.transform.position.x;
        unitPosition[1] = unit.transform.position.y;
        unitPosition[2] = unit.transform.position.z;

        unitRotation[0] = unit.transform.rotation.x;
        unitRotation[1] = unit.transform.rotation.y;
        unitRotation[2] = unit.transform.rotation.z;

        switch (unit.unitStats.unitTeam)
        {
            case Team.Player:
                unitTeam = 0;
                break;
            case Team.Enemy1:
                unitTeam = 1;
                break;
            default:
                Debug.LogError("Unknown unit team detected.");
                break;
        }

        switch (unit.unitStats.unitType)
        {
            case UnitType.villager:
                unitType = 0;
                break;
            default:
                Debug.LogError("Unknown unit type detected.");
                break;
        }

        if(unit.worker != null && unit.worker.carriedResource.amount > 0)
        {
            unitResourceAmountCarried = unit.worker.carriedResource.amount;
            switch (unit.worker.carriedResource.resourceInfo.resourceRaw)
            {
                case ResourceRaw.Berries:
                    unitResourceTypeCarried = 1;
                    break;
                case ResourceRaw.Wood:
                    unitResourceTypeCarried = 2;
                    break;
                case ResourceRaw.Gold:
                    unitResourceTypeCarried = 3;
                    break;
                default:
                    unitResourceTypeCarried = 0;
                    break;
            }
        }
        else
        {
            unitResourceAmountCarried = 0;
            unitResourceTypeCarried = 0;
        }
    }
}

[System.Serializable]
public class BuildingData
{
    public float[] buildingPosition;
    public float[] buildingRotation;
    public int buildingTeam;
    public int buildingType;
    public int storedResourceAmount;
    public int storedResourceType;

    public BuildingData(Building building)
    {
        buildingPosition = new float[3];
        buildingRotation = new float[3];

        buildingPosition[0] = building.transform.position.x;
        buildingPosition[1] = building.transform.position.y;
        buildingPosition[2] = building.transform.position.z;

        buildingRotation[0] = building.transform.rotation.x;
        buildingRotation[1] = building.transform.rotation.y;
        buildingRotation[2] = building.transform.rotation.z;

        switch (building.buildingStats.buildingTeam)
        {
            case Team.Player:
                buildingTeam = 0;
                break;
            case Team.Enemy1:
                buildingTeam = 1;
                break;
            default:
                Debug.LogError("Unknown building team detected.");
                break;
        }

        switch (building.buildingStats.buildingType)
        {
            case BuildingType.resourceCampConstruction:
                buildingType = 0;
                break;
            case BuildingType.resourceCamp:
                buildingType = 1;
                break;
            default:
                Debug.LogError("Unknown building type detected.");
                break;
        }

        ResourceCamp resourceCamp = building.gameObject.GetComponent<ResourceCamp>();
        if (resourceCamp != null)
        {
            storedResourceAmount = resourceCamp.amountStored;
            switch (resourceCamp.campType)
            {
                case ResourceType.Food:
                    storedResourceType = 1;
                    break;
                case ResourceType.Wood:
                    storedResourceType = 2;
                    break;
                case ResourceType.Gold:
                    storedResourceType = 3;
                    break;
                default:
                    storedResourceType = 0;
                    break;
            }
        }
        else
        {
            storedResourceAmount = 0;
            storedResourceType = 0;
        }
    }
}

[System.Serializable]
public class ResourceFieldData
{
    public float[] resourceFieldPosition;
    public float[] resourceFieldRotation;
    public int resourceFieldAmount;
    public int resourceFieldType;

    public ResourceFieldData(ResourceField resourceField)
    {
        resourceFieldPosition = new float[3];
        resourceFieldRotation = new float[3];

        resourceFieldPosition[0] = resourceField.transform.position.x;
        resourceFieldPosition[1] = resourceField.transform.position.y;
        resourceFieldPosition[2] = resourceField.transform.position.z;

        resourceFieldRotation[0] = resourceField.transform.rotation.x;
        resourceFieldRotation[1] = resourceField.transform.rotation.y;
        resourceFieldRotation[2] = resourceField.transform.rotation.z;

        resourceFieldAmount = resourceField.leftAmount;
        switch (resourceField.resourceInfo.resourceRaw)
        {
            case ResourceRaw.Berries:
                resourceFieldType = 1;
                break;
            case ResourceRaw.Wood:
                resourceFieldType = 2;
                break;
            case ResourceRaw.Gold:
                resourceFieldType = 3;
                break;
            default:
                resourceFieldType = 0;
                break;
        }
    }
}

[System.Serializable]
public class ResourceDropData
{
    public float[] resourceDropPosition;
    public int resourceDropAmount;
    public int resourceDropType;

    public ResourceDropData(ResourceDrop resourceDrop)
    {
        resourceDropPosition = new float[3];

        resourceDropPosition[0] = resourceDrop.transform.position.x;
        resourceDropPosition[1] = resourceDrop.transform.position.y;
        resourceDropPosition[2] = resourceDrop.transform.position.z;

        resourceDropAmount = resourceDrop.droppedResource.amount;
        switch (resourceDrop.droppedResource.resourceInfo.resourceRaw)
        {
            case ResourceRaw.Berries:
                resourceDropType = 1;
                break;
            case ResourceRaw.Wood:
                resourceDropType = 2;
                break;
            case ResourceRaw.Gold:
                resourceDropType = 3;
                break;
            default:
                resourceDropType = 0;
                break;
        }
    }
}

[System.Serializable]
public class GameSaveData
{ 
    public UnitData[] unitsData;
    public BuildingData[] buildingsData;
    public ResourceFieldData[] resourceFieldsData;
    public ResourceDropData[] resourceDropsData;
    public float[] cameraPosition;
    public float[] cameraRotation;

    public GameSaveData()
    {
        Vector3 cameraPos = CameraController.instance.GetCurrentDesiredCameraPosition();
        cameraPosition = new float[3];
        cameraPosition[0] = cameraPos.x;
        cameraPosition[1] = cameraPos.y;
        cameraPosition[2] = cameraPos.z;

        Quaternion cameraRot = CameraController.instance.GetCurrentDesiredCameraRotation();
        cameraRotation = new float[3];
        cameraRotation[0] = cameraRot.x;
        cameraRotation[1] = cameraRot.y;
        cameraRotation[2] = cameraRot.z;

        unitsData = new UnitData[GameManager.instance.activeUnits.Count];
        for(int i = 0; i < GameManager.instance.activeUnits.Count; i++)
            unitsData[i] = new UnitData(GameManager.instance.activeUnits[i]);

        buildingsData = new BuildingData[GameManager.instance.activeBuildings.Count];
        for (int i = 0; i < GameManager.instance.activeBuildings.Count; i++)
            buildingsData[i] = new BuildingData(GameManager.instance.activeBuildings[i]);

        resourceFieldsData = new ResourceFieldData[GameManager.instance.activeResourceFields.Count];
        for (int i = 0; i < GameManager.instance.activeResourceFields.Count; i++)
            resourceFieldsData[i] = new ResourceFieldData(GameManager.instance.activeResourceFields[i]);

        resourceDropsData = new ResourceDropData[GameManager.instance.activeResourceDrops.Count];
        for (int i = 0; i < GameManager.instance.activeResourceDrops.Count; i++)
            resourceDropsData[i] = new ResourceDropData(GameManager.instance.activeResourceDrops[i]);

    }
}
