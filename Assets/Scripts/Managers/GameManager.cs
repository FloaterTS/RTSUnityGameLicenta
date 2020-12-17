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

    [HideInInspector] public bool isPaused = false;

    private UnitSelection unitSelection;
    private UnitMovement unitMovement;
    private UnitInteraction unitInteraction;
    private ResourceManager resourceManager;
    private UIManager uiManager;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another game manager present.");

        unitSelection = GetComponent<UnitSelection>();
        unitMovement = GetComponent<UnitMovement>();
        unitInteraction = GetComponent<UnitInteraction>();
        resourceManager = GetComponent<ResourceManager>();
        uiManager = GetComponent<UIManager>();

        foreach (UnitStats unitStats in unitStatsList)
            unitStats.ResetStats();

        foreach (BuildingStats buildingStats in buildingStatsList)
            buildingStats.ResetStats();
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

}
