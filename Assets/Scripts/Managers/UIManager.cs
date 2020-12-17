using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

enum InteractionPanelState
{
    None,
    Unit,
    //Build,
    Camp
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
    InteractionPanelState currentInteractionState;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another UI manager present.");

        currentInteractionState = InteractionPanelState.None;

        Destroy(Instantiate(fadeUncoverPanel), 1f);
    }

    private void Update()
    {
        UpdateResourceText();
    }

    private void UpdateResourceText()
    {
        foodText.text = ResourceManager.instance.currentFoodAmount.ToString();
        woodText.text = ResourceManager.instance.currentWoodAmount.ToString();
        goldText.text = ResourceManager.instance.currentGoldAmount.ToString();
    }

    public bool IsMouseOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return true;
        else
            return false;
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

    IEnumerator UnPauseCo()
    {
        pauseMenu.GetComponent<Animator>().SetTrigger("fadeOut");
        yield return new WaitForSecondsRealtime(0.5f);
        pauseMenu.SetActive(false);
        GameManager.instance.UnPauseGameState();
    }

    IEnumerator LoadMainMenuCo()
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
        if (UnitSelection.instance.selectedUnits.Count > 0)
        {
            if (currentInteractionState != InteractionPanelState.Unit)
                DisableCurrentInteractionPanel();
            EnableInteractionPanel(InteractionPanelState.Unit);
        }
        else if (UnitSelection.instance.selectedBuilding != null)
        {
            if (UnitSelection.instance.selectedBuilding.gameObject.GetComponent<ResourceCamp>() != null)
            {
                if (currentInteractionState != InteractionPanelState.Camp)
                    DisableCurrentInteractionPanel();
                EnableInteractionPanel(InteractionPanelState.Camp);
            }
            else
                Debug.Log("Unknown UI Panel for Selected Building");
        }
        else
            DisableCurrentInteractionPanel();
    }

    // TO DO: use transition to state from unit panel to build panel to and use the functions from this script on button press?

    void DisableCurrentInteractionPanel()
    {
        if (currentInteractionState == InteractionPanelState.None)
            return;
        switch (currentInteractionState)
        {
            case InteractionPanelState.Unit:
                {
                    unitInteractionPanel.SetActive(false);
                    buildInteractionPanel.SetActive(false); //we also disable the build panel in case the unit was on the build panel when it was deselected
                }
                break;
            case InteractionPanelState.Camp:
                campInteractionPanel.SetActive(false);
                break;
        }
        currentInteractionState = InteractionPanelState.None;
        return;
    }

    void EnableInteractionPanel(InteractionPanelState newInteractionState)
    {
        if (newInteractionState == InteractionPanelState.None)
            return;
        switch (newInteractionState)
        {
            case InteractionPanelState.Unit:
                {
                    unitInteractionPanel.SetActive(true);
                    currentInteractionState = InteractionPanelState.Unit;
                }
                return;
            case InteractionPanelState.Camp:
                {
                    campInteractionPanel.SetActive(true);
                    currentInteractionState = InteractionPanelState.Camp;
                }
                return;
        }
        return;
    }
}
