using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

enum InteractionPanelState
{
    NONE,
    UNIT,
    //BUILD,
    CAMP,
    INN
}

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI foodText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI woodText;
    [Space]
    public GameObject pauseMenu;
    public GameObject fadeCoverPanel;
    public GameObject fadeUncoverPanel;
    [Space]
    public GameObject unitInteractionPanel;
    public GameObject buildInteractionPanel;
    public GameObject campInteractionPanel;
    public GameObject innInteractionPanel;
    [Space]
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI victoryText;
    public string victoryMessage = "Victory!";
    public string defeatMessage = "Try again?";
    public float mainMenuTimeout = 5f;

    private InteractionPanelState currentInteractionState;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another UI manager present.");
    }

    private void Start()
    {
        currentInteractionState = InteractionPanelState.NONE;

        Destroy(Instantiate(fadeUncoverPanel), 1f);
    }

    private void Update()
    {
        UpdateResourceText();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.instance.isPaused)
                UnPauseGameTab();
            else
                PauseGameTab();
        }  
    }

    private void UpdateResourceText()
    {
        foodText.text = ResourceManager.instance.currentFoodAmount.ToString();
        woodText.text = ResourceManager.instance.currentWoodAmount.ToString();
        goldText.text = ResourceManager.instance.currentGoldAmount.ToString();
    }

    public bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void PauseGameTab()
    {
        GameManager.instance.PauseGameState();
        pauseMenu.SetActive(true);
    }

    public void UnPauseGameTab()
    {
        StartCoroutine(UnPauseCo());
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadMainMenuCo());
    }

    private IEnumerator UnPauseCo()
    {
        pauseMenu.GetComponent<Animator>().SetTrigger("fadeOut");
        yield return new WaitForSecondsRealtime(0.5f);
        pauseMenu.SetActive(false);
        GameManager.instance.UnPauseGameState();
    }

    private IEnumerator LoadMainMenuCo()
    {
        Instantiate(fadeCoverPanel);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainMenu");

        asyncOperation.allowSceneActivation = false;
        yield return new WaitForSecondsRealtime(1f); // wait for fade to finish fading
        asyncOperation.allowSceneActivation = true;

        Time.timeScale = 1f;
        while (!asyncOperation.isDone)
            yield return null;
    }

    public void UpdateSelectedInteractionUI()
    {
        DisableCurrentInteractionPanel();
        if (SelectionManager.instance.selectedUnits.Count > 0)
            EnableInteractionPanel(InteractionPanelState.UNIT);
        else if (SelectionManager.instance.selectedBuilding != null)
        {
            if (SelectionManager.instance.selectedBuilding.gameObject.GetComponent<ResourceCamp>() != null)
                EnableInteractionPanel(InteractionPanelState.CAMP);
            else if (SelectionManager.instance.selectedBuilding.gameObject.GetComponent<VillagerInn>() != null)
                EnableInteractionPanel(InteractionPanelState.INN);
            else
                Debug.Log("Unknown UI Panel for Selected Building");
        }
        // else: no panel will be enabled
    }

    // TO DO: use transition to state from unit panel to build panel to and use the functions from this script on button press?

    void DisableCurrentInteractionPanel()
    {
        if (currentInteractionState == InteractionPanelState.NONE)
            return;
        switch (currentInteractionState)
        {
            case InteractionPanelState.UNIT:
                {
                    unitInteractionPanel.SetActive(false);
                    buildInteractionPanel.SetActive(false); //we also disable the build panel in case the unit was on the build panel when it was deselected
                }
                break;
            case InteractionPanelState.CAMP:
                campInteractionPanel.SetActive(false);
                break;
            case InteractionPanelState.INN:
                innInteractionPanel.SetActive(false);
                break;
        }
        currentInteractionState = InteractionPanelState.NONE;
        return;
    }

    void EnableInteractionPanel(InteractionPanelState newInteractionState)
    {
        if (newInteractionState == InteractionPanelState.NONE)
            return;
        switch (newInteractionState)
        {
            case InteractionPanelState.UNIT:
                {
                    unitInteractionPanel.SetActive(true);
                    currentInteractionState = InteractionPanelState.UNIT;
                }
                return;
            case InteractionPanelState.CAMP:
                {
                    campInteractionPanel.SetActive(true);
                    currentInteractionState = InteractionPanelState.CAMP;
                }
                break;
            case InteractionPanelState.INN:
                {
                    innInteractionPanel.SetActive(true);
                    currentInteractionState = InteractionPanelState.INN;
                }
                return;
        }
        return;
    }

    public void BuildPanelConstructCamp()
    {
        ConstructionManager.instance.StartPreviewResourceCampConstruction();
    }

    public void BuildPanelConstructInn()
    {
        ConstructionManager.instance.StartPreviewVillagerInnConstruction();
    }

    public void BuildPanelCancelBuildPreview()
    {
        ConstructionManager.instance.StopPreviewBuildingGO();
        ConstructionManager.instance.StopPreviewBuildingBool();
    }

    public void UnitPanelStopAction()
    {
        foreach (Unit unit in SelectionManager.instance.selectedUnits)
            unit.StopAction();
    }

    public void AdjustCameraXRotationSetting(float valueXRotation)
    {
        CameraController.instance.AdjustXRotation(valueXRotation);
        GamePrefsManager.instance.SaveCameraXRotationPref(valueXRotation);
    }

    public void AdjustCameraFOVSetting(float valueFOV)
    {
        CameraController.instance.AdjustFieldOfView(valueFOV);
        GamePrefsManager.instance.SaveCameraFieldOfViewPref(valueFOV);
    }

    public void ToggleCameraSnapRotationSetting(bool snapRotationActive)
    {
        CameraController.instance.ToggleSnapRotation(snapRotationActive);
        GamePrefsManager.instance.SaveCameraSnapRotationPref(snapRotationActive);
    }

    public void ToggleCameraMouseMovementSetting(bool movementByMouseActive)
    {
        CameraController.instance.ToggleMovementByMouse(movementByMouseActive);
        GamePrefsManager.instance.SaveCameraMovementByMousePref(movementByMouseActive);
    }

    public void QuickSaveButton()
    {
        SaveLoadSystem.instance.SaveGame();
    }

    public void QuickLoadButton()
    {
        if (SaveLoadSystem.instance.saveFileExists.saveToBeLoaded)
            SaveLoadSystem.instance.LoadGame();
        else
            ShowScreenAlert("No save file found.");
    }

    public void ShowScreenAlert(string message, float seconds = 5f)
    {
        if(infoText.text != message)
            StartCoroutine(ShowScreenAlertCo(message, seconds));
    }

    public void GameOverUI(bool playerWin)
    {
        victoryText.gameObject.SetActive(true);

        if (playerWin)
        {
            victoryText.text = victoryMessage;
            victoryText.color = GameManager.instance.playerMaterial.color;
        }
        else
        {
            victoryText.text = defeatMessage;
            victoryText.color = GameManager.instance.enemyMaterial.color;
        }

        StartCoroutine(GameOverMainMenu());
    }

    private IEnumerator ShowScreenAlertCo(string message, float seconds)
    {
        infoText.gameObject.SetActive(true);
        infoText.text = message;

        yield return new WaitForSeconds(seconds);

        if (infoText.text == message)
        {
            infoText.text = "";
            infoText.gameObject.SetActive(false);
        }
    }

    private IEnumerator GameOverMainMenu()
    {
        yield return new WaitForSeconds(mainMenuTimeout);
        LoadMainMenu();
    }
}
