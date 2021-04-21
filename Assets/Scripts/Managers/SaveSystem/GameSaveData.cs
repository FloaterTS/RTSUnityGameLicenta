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

//Resource field model classification
//0 - Small berry bush / Fallen Tree / Gold Ore Mine
//1 - Large berry bush

[System.Serializable]
public class UnitData
{
    public float[] unitPosition;
    public float[] unitRotation;
    public float unitHealth;
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

        unitRotation[0] = unit.transform.eulerAngles.x;
        unitRotation[1] = unit.transform.eulerAngles.y;
        unitRotation[2] = unit.transform.eulerAngles.z;

        unitHealth = unit.GetHealth();

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
    public float buildingHealth;
    public int buildingTeam;
    public int buildingType;
    public int storedResourceAmount;
    public int storedResourceType;
    public float amountConstructed;

    public BuildingData(Building building)
    {
        buildingPosition = new float[3];
        buildingRotation = new float[3];

        buildingPosition[0] = building.transform.position.x;
        buildingPosition[1] = building.transform.position.y;
        buildingPosition[2] = building.transform.position.z;

        buildingRotation[0] = building.transform.eulerAngles.x;
        buildingRotation[1] = building.transform.eulerAngles.y;
        buildingRotation[2] = building.transform.eulerAngles.z;

        buildingHealth = building.GetCurrentHitpoints();

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
            case BuildingType.resourceCamp:
                {
                    if (building.gameObject.GetComponent<UnderConstruction>() != null)
                    {
                        buildingType = 0;
                        Debug.Log(building.gameObject.GetComponent<UnderConstruction>());
                    }
                    else
                        buildingType = 1;
                }
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
                case ResourceType.None:
                    storedResourceType = 0;
                    break;
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
                    storedResourceType = -1;
                    break;
            }
        }
        else
        {
            storedResourceAmount = 0;
            storedResourceType = 0;
        }

        UnderConstruction underConstruction = building.GetComponent<UnderConstruction>();
        if (underConstruction != null)
            amountConstructed = underConstruction.GetAmountConstructed();
        else
            amountConstructed = 0f;
    }
}

[System.Serializable]
public class ResourceFieldData
{
    public float[] resourceFieldPosition;
    public float[] resourceFieldRotation;
    public int resourceFieldAmount;
    public int resourceFieldType;
    public int resourceFieldModelType;

    public ResourceFieldData(ResourceField resourceField)
    {
        resourceFieldPosition = new float[3];
        resourceFieldRotation = new float[3];

        resourceFieldPosition[0] = resourceField.transform.position.x;
        resourceFieldPosition[1] = resourceField.transform.position.y;
        resourceFieldPosition[2] = resourceField.transform.position.z;

        resourceFieldRotation[0] = resourceField.transform.eulerAngles.x;
        resourceFieldRotation[1] = resourceField.transform.eulerAngles.y;
        resourceFieldRotation[2] = resourceField.transform.eulerAngles.z;

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

        switch (resourceField.resourceFieldModel)
        {
            case ResourceFieldModel.BerryBushSmall:
            case ResourceFieldModel.LumberTree:
            case ResourceFieldModel.GoldOreMine:
                resourceFieldModelType = 0;
                break;
            case ResourceFieldModel.BerryBushLarge:
                resourceFieldModelType = 1;
                break;
            default:
                resourceFieldModelType = 0;
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
    public float cameraYRotationDegrees;

    public GameSaveData()
    {
        Vector3 cameraPos = CameraController.instance.GetCurrentDesiredCameraPosition();
        cameraPosition = new float[3];
        cameraPosition[0] = cameraPos.x;
        cameraPosition[1] = cameraPos.y;
        cameraPosition[2] = cameraPos.z;

        Quaternion cameraRot = CameraController.instance.GetCurrentDesiredCameraRotation();
        cameraYRotationDegrees = cameraRot.eulerAngles.y;

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
