using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player,
    Enemy1,
    Enemy2,
    Enemy3,
    Neutral,
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

    private bool isPaused = false;

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
    }
}
