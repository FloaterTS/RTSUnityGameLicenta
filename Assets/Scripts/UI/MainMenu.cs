using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;


public class MainMenu : MonoBehaviour
{
    [Header("Start Menu Settings")]
    public GameObject fadeCoverPanel;
    public GameObject fadeUncoverPanel;

    public TMP_Dropdown playerColorDropdown;
    public TMP_Dropdown enemyColorDropdown;

    public Material playerColorMaterial;
    public Material enemyColorMaterial;
    public Material selectionMaterial;

    private float selectionColorAlphaValue = 0.25f;

    [Space]
    [Header("Saved Game Settings")]
    public LoadSaveBool continueSavedGame;
    public LoadSaveBool saveFileExists;

    public Button continueButton;
    public TextMeshProUGUI continueText;
    public Color continueTextDisabledColor;

    [Space]
    [Header("Options Menu Settings")]
    public Toggle fullScreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Slider volumeSlider;
    public AudioMixer audioMixer;

    private Resolution[] resolutions;


    private void Start()
    {
        if(!saveFileExists.saveToBeLoaded)
        {
            continueButton.enabled = false;
            continueText.color = continueTextDisabledColor;
        }

        InitSettings();
    }

    public void NewGame()
    {
        saveFileExists.saveToBeLoaded = false;
        AssignColors();
        StartGame();
    }

    public void ContinueGame()
    {
        if (saveFileExists.saveToBeLoaded)
        {
            continueSavedGame.saveToBeLoaded = true;
            StartGame();
        }
    }

    private void StartGame()
    {
        InitSettings();
        Instantiate(fadeCoverPanel);
        StartCoroutine(LoadGame());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(1f); // wait for fade to finish fading

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main");

        //asyncOperation.allowSceneActivation = false;
        //asyncOperation.allowSceneActivation = true;

        while (!asyncOperation.isDone)
            yield return null;
    }

    private void AssignColors()
    {
        Color playerColor = IndexToColor(playerColorDropdown.value);
        Color enemyColor = IndexToColor(enemyColorDropdown.value);

        playerColorMaterial.color = playerColor;
        enemyColorMaterial.color = enemyColor;
        
        Color selectionColor = new Color(playerColor.r, playerColor.g, playerColor.b, selectionColorAlphaValue);
        selectionMaterial.color = selectionColor;
    }

    private void InitSettings()
    {
        InitResolutionsDropdown();
        InitFullScreenToggle();
        InitGraphicsDropdown();
        InitVolumeSlider();
    }

    private void InitResolutionsDropdown()
    {
        int currResIndex = 0;
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> resOptionStrings = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resOption = resolutions[i].width + " x " + resolutions[i].height;
            resOptionStrings.Add(resOption);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currResIndex = i;
        }

        resolutionDropdown.AddOptions(resOptionStrings);

        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex");
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutionIndex;
        }
        else
            resolutionDropdown.value = currResIndex;

        resolutionDropdown.RefreshShownValue();
    }

    private void InitFullScreenToggle()
    {
        if (PlayerPrefs.HasKey("fullScreen"))
        {
            bool isFullScreen = PlayerPrefs.GetInt("fullScreen") == 1;
            Screen.fullScreen = isFullScreen;
            fullScreenToggle.isOn = isFullScreen;
        }
    }

    private void InitGraphicsDropdown()
    {
        string[] qualityNames = QualitySettings.names;

        List<string> qualityNamesList = new List<string>();

        for (int i = 0; i < qualityNames.Length; i++)
            qualityNamesList.Add(qualityNames[i]);

        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(qualityNamesList);

        if (PlayerPrefs.HasKey("qualityIndex"))
        {
            int qualityIndex = PlayerPrefs.GetInt("qualityIndex");
            QualitySettings.SetQualityLevel(qualityIndex);
            graphicsDropdown.value = qualityIndex;
        }
        else
            graphicsDropdown.value = QualitySettings.GetQualityLevel();
    }

    private void InitVolumeSlider()
    {
        if (PlayerPrefs.HasKey("volume"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            audioMixer.SetFloat("volume", volume);
            volumeSlider.value = volume;
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);

        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("qualityIndex", qualityIndex);
    }


    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        PlayerPrefs.SetInt("fullScreen", isFullScreen ? 1 : 0);
    }

    public void CheckPlayerColorMatch(int colorIndex)
    {
        if (colorIndex == playerColorDropdown.value)
        {
            if (playerColorDropdown.value == playerColorDropdown.options.Count - 1)
                playerColorDropdown.value = 0;
            else
                playerColorDropdown.value++;
        }
    }

    public void CheckEnemyColorMatch(int colorIndex)
    {
        if (colorIndex == enemyColorDropdown.value)
        {
            if (enemyColorDropdown.value == enemyColorDropdown.options.Count - 1)
                enemyColorDropdown.value = 0;
            else
                enemyColorDropdown.value++;
        }
    }

    private Color IndexToColor(int index)
    {
        /*Colors legend:
        0 = Black
        1 = Blue
        2 = Cyan
        3 = Green
        4 = Orange
        5 = Red
        6 = Yellow*/
        switch(index)
        {
            case 0: return Color.black;
            case 1: return new Color(0f, 0f, 0.5f);
            case 2: return new Color(0f, 0.5f, 0.5f);
            case 3: return new Color(0f, 0.5f, 0f);
            case 4: return new Color(0.7f, 0.25f, 0f);
            case 5: return new Color(0.5f, 0f, 0f);
            case 6: return new Color(0.55f, 0.5f, 0f);
            default: return Color.white;
        }
    }
}
