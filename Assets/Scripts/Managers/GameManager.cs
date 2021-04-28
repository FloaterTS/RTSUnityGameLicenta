using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    PLAYER,
    ENEMY1,
    ENEMY2,
    ENEMY3,
    NEUTRAL,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Terrain mainTerrain;
    public UnitStats[] unitStatsList;
    public BuildingStats[] buildingStatsList;

    public List<Unit> activeUnits;
    public List<Building> activeBuildings;
    public List<ResourceField> activeResourceFields;
    public List<ResourceDrop> activeResourceDrops;

    [HideInInspector]
    public bool isPaused = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another game manager present.");

        foreach (UnitStats unitStats in unitStatsList)
            unitStats.ResetStats();

        foreach (BuildingStats buildingStats in buildingStatsList)
            buildingStats.ResetStats();

        activeUnits = new List<Unit>();
        activeBuildings = new List<Building>();
        activeResourceFields = new List<ResourceField>();
        activeResourceDrops = new List<ResourceDrop>();
    }

    public void PauseGameState()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void UnPauseGameState()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void ClearSceneEntities()
    {
        foreach(Unit unit in activeUnits)
            Destroy(unit.gameObject);
        activeUnits.Clear();

        foreach (Building building in activeBuildings)
            Destroy(building.gameObject);
        activeBuildings.Clear();

        foreach (ResourceField resourceField in activeResourceFields)
            Destroy(resourceField.gameObject);
        activeResourceFields.Clear();

        foreach (ResourceDrop resourceDrop in activeResourceDrops)
            Destroy(resourceDrop.gameObject);
        activeResourceDrops.Clear();

        ResourceManager.instance.resourceCamps.Clear();

        SelectionManager.instance.selectedUnits.Clear();
        SelectionManager.instance.selectedBuilding = null;
        SelectionManager.instance.heroSelected = false;
    }

    public GameObject CheckForActiveObjectAtPosition(Vector3 positionToCheck)
    {
        foreach (Unit unit in activeUnits)
            if(unit.transform.position == positionToCheck)
                return unit.gameObject;

        foreach (Building building in activeBuildings)
        {
            if (building.gameObject.CompareTag("ResourceCamp"))
            {
                ResourceCamp resourceCamp = building.GetComponent<ResourceCamp>();
                if (resourceCamp != null)
                {
                    if (resourceCamp.accessLocation == positionToCheck)
                        return building.gameObject;
                }
                else
                {
                    Debug.LogError("ResourceCamp script missing from " + building.gameObject + " tagged as ResourceCamp");
                    return null;
                }
            }
            else if (building.transform.position == positionToCheck)
                return building.gameObject;
        }

        foreach (ResourceField resourceField in activeResourceFields)
            if (resourceField.transform.position == positionToCheck)
                return resourceField.gameObject;

        foreach (ResourceDrop resourceDrop in activeResourceDrops)
            if (resourceDrop.transform.position == positionToCheck)
                return resourceDrop.gameObject;

        return null;
    }

}
