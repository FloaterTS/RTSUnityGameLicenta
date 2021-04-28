using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject fadeCoverPanel;
    public GameObject fadeUncoverPanel;

    public LoadSaveBool continueSavedGame;
    public LoadSaveBool saveFileExists;

    public Button continueButton;
    public TextMeshProUGUI continueText;
    public Color continueTextDisabledColor;

    private void Start()
    {
        //Destroy(Instantiate(fadeUncoverPanel), 1f);

        if(!saveFileExists.saveToBeLoaded)
        {
            continueButton.enabled = false;
            continueText.color = continueTextDisabledColor;
        }
    }

    public void NewGame()
    {
        Instantiate(fadeCoverPanel);
        StartCoroutine(LoadGame());
    }

    public void ContinueGame()
    {
        if (saveFileExists.saveToBeLoaded)
        {
            continueSavedGame.saveToBeLoaded = true;
            NewGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(1f); // wait for fade to finish fading

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main");

        // uncomment this and comment first line for scene loading starting at the same time with the fade
        /* asyncOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(1f); // wait for fade to finish fading
        asyncOperation.allowSceneActivation = true;*/

        while (!asyncOperation.isDone)
            yield return null;
    }

}
