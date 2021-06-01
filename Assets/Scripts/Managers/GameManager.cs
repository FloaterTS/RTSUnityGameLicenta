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
    NONE
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

    public Material playerMaterial;
    public Material enemyMaterial;

    [HideInInspector]
    public bool isPaused = false;

    private bool gameOver = false;

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

    private void Update()
    {
        CheckGameEnd();
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

    public ResourceField GetClosestResourceFieldOfTypeFrom(ResourceRaw type, Vector3 from)
    {
        ResourceField closestResourceField = null;
        float minDistance = 10000;
        foreach (ResourceField resourseField in activeResourceFields)
        {
            if (resourseField.resourceInfo.resourceRaw == type)
            {
                float distance = Vector3.Distance(resourseField.transform.position, from);
                if (distance < minDistance)
                {
                    closestResourceField = resourseField;
                    minDistance = distance;
                }
            }
        }
        return closestResourceField;
    }

    private void CheckGameEnd()
    {
        if (gameOver)
            return;

        int playerUnits = 0;
        int enemyUnits = 0;
        foreach(Unit unit in activeUnits)
        {
            if (unit.unitStats.unitTeam == Team.PLAYER)
                playerUnits++;
            else if (unit.unitStats.unitTeam == Team.ENEMY1 || unit.unitStats.unitTeam == Team.ENEMY2 || unit.unitStats.unitTeam == Team.ENEMY3)
                enemyUnits++;
        }

        if (playerUnits == 0)
            StartCoroutine(GameOver(false));
        else if (enemyUnits == 0)
            StartCoroutine(GameOver(true));
    }

    private IEnumerator GameOver(bool playerWin)
    {
        if (gameOver)
            yield break;

        gameOver = true;

        yield return new WaitForSeconds(1f);

        Debug.Log("Game Over" + (playerWin ? " Player wins!" : " Enemy wins!"));

        UIManager.instance.GameOverUI(playerWin);
    }
}
