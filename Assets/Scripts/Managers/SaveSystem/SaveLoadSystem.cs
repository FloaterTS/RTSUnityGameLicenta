using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem instance;

    public LoadSaveBool loadLastSaveTrigger;
    public LoadSaveBool saveFileExists;

    private readonly string saveName = "guildsSave";
    private readonly string saveExtension = ".rts";

    private string saveFilePath;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another save and load system present.");
    }

    private void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, saveName + saveExtension);


        if(loadLastSaveTrigger.saveToBeLoaded)
        {
            LoadGame();
            loadLastSaveTrigger.saveToBeLoaded = false;
        }
    }

    public void SaveGame()
    {
        Debug.Log("Saving game..");

        //  Creating the save file at the save path location
        FileStream fileStream = new FileStream(saveFilePath, FileMode.Create);

        // Initializing a binary formatter that will be used to serialize the save data
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        // Saving game objects data
        GameSaveData gameSaveData = new GameSaveData();

        try
        {
            // Serializing the game data to the save file
            binaryFormatter.Serialize(fileStream, gameSaveData);
            Debug.Log("Game saved");
            saveFileExists.saveToBeLoaded = true;
        }
        catch(SerializationException e)
        {
            Debug.LogError("There was a problem serializing save data: " + e.Message);
        }
        finally
        {
            // Closing the file stream
            fileStream.Close();
        }
    }

    public void LoadGame()
    {
        Debug.Log("Loading game save..");

        // Checking to see if the save file exists
        if(!File.Exists(saveFilePath))
        {
            Debug.LogError("Save file not found in " + saveFilePath);
            saveFileExists.saveToBeLoaded = false;
            return;
        }

        // Initializing a binary formatter that will be used to deserialize the save data
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        // Opening the save file located at the save path
        FileStream fileStream = new FileStream(saveFilePath, FileMode.Open);

        try
        {
            // Deserializing the game data from the save file
            GameSaveData gameSaveData = binaryFormatter.Deserialize(fileStream) as GameSaveData;
            Debug.Log("Game save loaded");

            // Setting up the game data from the save file
            SetUpLoadedGame(gameSaveData);
        }
        catch (SerializationException e)
        {
            Debug.LogError("There was a problem deserializing save data: " + e.Message);
        }
        finally
        {
            // Closing the file stream
            fileStream.Close();
        }
    }
       
    public void DeleteGameSave()
    {
        // Checking to see if the save file exists
        if (File.Exists(saveFilePath))
        {
            // Deleting the save file
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted");
        }
        else
            Debug.LogError("Save file to be deleted not found in " + saveFilePath);
    }

    private void SetUpLoadedGame(GameSaveData data)
    {
        Debug.Log("Setting up loaded save..");

        // Clearing current active objects from scene
        GameManager.instance.ClearSceneEntities();

        for(int i = 0; i < data.unitsData.Length; i++)
        {
            GameObject unitGO;
            Unit unitScript;

            Vector3 unitPosition = new Vector3(data.unitsData[i].unitPosition[0], data.unitsData[i].unitPosition[1], data.unitsData[i].unitPosition[2]);
            Quaternion unitRotation = Quaternion.Euler(data.unitsData[i].unitRotation[0], data.unitsData[i].unitRotation[1], data.unitsData[i].unitRotation[2]);

            switch (data.unitsData[i].unitType)
            {
                case 0:
                    {
                        unitGO = Instantiate(PrefabManager.instance.villagerPrefab, unitPosition, unitRotation, PrefabManager.instance.unitsTransformParentGO.transform);
                        unitScript = unitGO.GetComponent<Unit>();

                        if(unitScript == null)
                        {
                            Debug.LogError("Unit prefab doesn't have Unit script attached");
                            return;
                        }

                        switch(data.unitsData[i].unitTeam)
                        {
                            case 0: // Player Team
                                unitScript.unitStats = PrefabManager.instance.playerVillagerStats;
                                break;
                            case 1: // Enemy Team
                                unitScript.unitStats = PrefabManager.instance.enemyVillagerStats;
                                break;
                            default:
                                Debug.LogError("Unit team " + data.unitsData[i].unitTeam + " from save data not recognized");
                                return;
                        }
                    }
                    break;
                default:
                    Debug.LogError("Unit type " + data.unitsData[i].unitType + " from save data not recognized");
                    return;
            }

            unitScript.SetCurrentHealth(data.unitsData[i].unitHealth);

            Worker worker = unitGO.GetComponent<Worker>();
            if(worker != null && data.unitsData[i].unitResourceAmountCarried > 0)
            {
                worker.carriedResource.amount = data.unitsData[i].unitResourceAmountCarried;

                switch (data.unitsData[i].unitResourceTypeCarried)
                {
                    case 1: // Berries
                        worker.carriedResource.resourceInfo = PrefabManager.instance.berriesInfo;
                        break;
                    case 2: // Wood
                        worker.carriedResource.resourceInfo = PrefabManager.instance.woodInfo;
                        break;
                    case 3: // Gold
                        worker.carriedResource.resourceInfo = PrefabManager.instance.goldInfo;
                        break;
                    case 0:
                        Debug.LogError("Unit carrying non-zero amount of resource type None in save data");
                        return;
                    default:
                        Debug.LogError("Carried resource type " + data.unitsData[i].unitResourceTypeCarried + " from save data not recognized");
                        return;
                }

                worker.LiftResorce(true);
            }
        }

        for (int i = 0; i < data.buildingsData.Length; i++)
        {
            GameObject buildingGO;
            Building buildingScript;

            Vector3 buildingPosition = new Vector3(data.buildingsData[i].buildingPosition[0], data.buildingsData[i].buildingPosition[1], data.buildingsData[i].buildingPosition[2]);
            Quaternion buildingRotation = Quaternion.Euler(data.buildingsData[i].buildingRotation[0], data.buildingsData[i].buildingRotation[1], data.buildingsData[i].buildingRotation[2]);

            switch (data.buildingsData[i].buildingType)
            {
                case 0: // Resource Camp Construction
                    {
                        buildingGO = Instantiate(PrefabManager.instance.resourceCampConstructionPrefab, buildingPosition, buildingRotation, PrefabManager.instance.buildingsTransformParentGO.transform);
                        buildingScript = buildingGO.GetComponent<Building>();

                        if (buildingScript == null)
                        {
                            Debug.LogError("Resource Camp Construction prefab doesn't have Building script attached");
                            return;
                        }

                        switch (data.buildingsData[i].buildingTeam)
                        {
                            case 0:
                                buildingScript.buildingStats = PrefabManager.instance.playerResourceCamp;
                                break;
                            default:
                                Debug.LogError("Building team " + data.buildingsData[i].buildingTeam + " from save data not recognized");
                                return;
                        }

                        UnderConstruction underConstruction = buildingGO.GetComponent<UnderConstruction>();
                        if (underConstruction != null)
                        {
                            underConstruction.Construct(data.buildingsData[i].amountConstructed);
                        }
                        else
                        {
                            Debug.LogError("Resource Camp Construction prefab doesn't have under construction script");
                            return;
                        }
                    }
                    break;
                case 1: // Resource Camp
                    {
                        buildingGO = Instantiate(PrefabManager.instance.resourceCampPrefab, buildingPosition, buildingRotation, PrefabManager.instance.buildingsTransformParentGO.transform);
                        buildingScript = buildingGO.GetComponent<Building>();

                        if (buildingScript == null)
                        {
                            Debug.LogError("Resource Camp prefab doesn't have Building script attached");
                            return;
                        }

                        switch (data.buildingsData[i].buildingTeam)
                        {
                            case 0:
                                buildingScript.buildingStats = PrefabManager.instance.playerResourceCamp;
                                break;
                            default:
                                Debug.LogError("Building team " + data.buildingsData[i].buildingTeam + " from save data not recognized");
                                return;
                        }

                        ResourceCamp resourceCampScript = buildingGO.GetComponent<ResourceCamp>();
                        if (resourceCampScript == null)
                        {
                            Debug.LogError("Resource Camp prefab doesn't have Building script attached");
                            return;
                        }
                        switch (data.buildingsData[i].storedResourceType)
                        {
                            case 0:
                                break;
                            case 1:
                                resourceCampScript.StoreResourceInCamp(data.buildingsData[i].storedResourceAmount, ResourceType.Food);
                                break;
                            case 2:
                                resourceCampScript.StoreResourceInCamp(data.buildingsData[i].storedResourceAmount, ResourceType.Wood);
                                break;
                            case 3:
                                resourceCampScript.StoreResourceInCamp(data.buildingsData[i].storedResourceAmount, ResourceType.Gold);
                                break;
                            default:
                                Debug.LogError("Camp resource type " + data.buildingsData[i].storedResourceType + " from save data not recognized");
                                return;
                        }
                    }
                    break;
                default:
                    Debug.LogError("Building type " + data.buildingsData[i].buildingType + " from save data not recognized");
                    return;
            }

            buildingScript.SetCurrentHitpoints(data.buildingsData[i].buildingHealth);
        }

        for (int i = 0; i < data.resourceFieldsData.Length; i++)
        {
            GameObject resourceFieldGO;
            ResourceField resourceFieldScript;

            Vector3 resourceFieldPosition = new Vector3(data.resourceFieldsData[i].resourceFieldPosition[0], data.resourceFieldsData[i].resourceFieldPosition[1], data.resourceFieldsData[i].resourceFieldPosition[2]);
            Quaternion resourceFieldRotation = Quaternion.Euler(data.resourceFieldsData[i].resourceFieldRotation[0], data.resourceFieldsData[i].resourceFieldRotation[1], data.resourceFieldsData[i].resourceFieldRotation[2]);

            switch (data.resourceFieldsData[i].resourceFieldType)
            {
                case 1: // Berries
                    {
                        switch (data.resourceFieldsData[i].resourceFieldModelType)
                        {
                            case 0: // Small berry bush
                                resourceFieldGO = Instantiate(PrefabManager.instance.berryBushSmallPrefab, resourceFieldPosition, resourceFieldRotation, PrefabManager.instance.resourceFieldsTransformParentGO.transform);
                                break;
                            case 1: // Large berry bush
                                resourceFieldGO = Instantiate(PrefabManager.instance.berryBushLargePrefab, resourceFieldPosition, resourceFieldRotation, PrefabManager.instance.resourceFieldsTransformParentGO.transform);
                                break;
                            default:
                                Debug.LogError("Resource field model type " + data.resourceFieldsData[i].resourceFieldModelType + " from save data not recognized");
                                return;
                        }
                    }
                    break;
                case 2: // Wood
                    {
                        switch (data.resourceFieldsData[i].resourceFieldModelType)
                        {
                            case 0: // Lumber tree
                                resourceFieldGO = Instantiate(PrefabManager.instance.lumberTreePrefab, resourceFieldPosition, resourceFieldRotation, PrefabManager.instance.resourceFieldsTransformParentGO.transform);
                                break;
                            default:
                                Debug.LogError("Resource field model type " + data.resourceFieldsData[i].resourceFieldModelType + " from save data not recognized");
                                return;
                        }
                    }
                    break;
                case 3: // Gold
                    {
                        switch (data.resourceFieldsData[i].resourceFieldModelType)
                        {
                            case 0: // Gold Ore Mine
                                resourceFieldGO = Instantiate(PrefabManager.instance.goldOreMinePrefab, resourceFieldPosition, resourceFieldRotation, PrefabManager.instance.resourceFieldsTransformParentGO.transform);
                                break;
                            default:
                                Debug.LogError("Resource field model type " + data.resourceFieldsData[i].resourceFieldModelType + " from save data not recognized");
                                return;
                        }
                    }
                    break;
                case 0:
                    Debug.LogError("Resource field type None from save data not possible");
                    return;
                default:
                    Debug.LogError("Resource field type " + data.resourceFieldsData[i].resourceFieldType + " from save data not recognized");
                    return;
            }

            resourceFieldScript = resourceFieldGO.GetComponent<ResourceField>();
            if (resourceFieldScript != null)
            {
                resourceFieldScript.leftAmount = data.resourceFieldsData[i].resourceFieldAmount;
            }
            else
            {
                Debug.LogError("Resource field prefab doesn't have resource field script");
                return;
            }
        }

        for (int i = 0; i < data.resourceDropsData.Length; i++)
        {
            GameObject resourceDropGO;
            ResourceDrop resourceDropScript;

            Vector3 resourceDropPosition = new Vector3(data.resourceDropsData[i].resourceDropPosition[0], data.resourceDropsData[i].resourceDropPosition[1], data.resourceDropsData[i].resourceDropPosition[2]);

            switch (data.resourceDropsData[i].resourceDropType)
            {
                case 1: // Berries
                    resourceDropGO = Instantiate(PrefabManager.instance.berriesDropPrefab, resourceDropPosition, Quaternion.identity, PrefabManager.instance.resourceDropsTransformParentGO.transform);
                    break;
                case 2: // Wood
                    resourceDropGO = Instantiate(PrefabManager.instance.logPileDropPrefab, resourceDropPosition, Quaternion.identity, PrefabManager.instance.resourceDropsTransformParentGO.transform);
                    break;
                case 3: // Gold
                    resourceDropGO = Instantiate(PrefabManager.instance.goldOreDropPrefab, resourceDropPosition, Quaternion.identity, PrefabManager.instance.resourceDropsTransformParentGO.transform);
                    break;
                case 0:
                    Debug.LogError("Resource drop type None from save data not possible");
                    return;
                default:
                    Debug.LogError("Resource drop type " + data.resourceDropsData[i].resourceDropType + " from save data not recognized");
                    return;
            }

            resourceDropScript = resourceDropGO.GetComponent<ResourceDrop>();
            if (resourceDropScript != null)
                resourceDropScript.droppedResource.amount = data.resourceDropsData[i].resourceDropAmount;
            else
            {
                Debug.LogError("Resource drop prefab doesn't have resource drop script");
                return;
            }
        }

        Vector3 cameraPositionVector = new Vector3(data.cameraPosition[0], data.cameraPosition[1], data.cameraPosition[2]);
        CameraController.instance.SetCameraPositionAndRotation(cameraPositionVector, data.cameraYRotationDegrees);
       
        Debug.Log("Finished setting up loaded game save");
    }

}
