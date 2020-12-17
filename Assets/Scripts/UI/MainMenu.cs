using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject fadeCoverPanel;
    public GameObject fadeUncoverPanel;

    private void Start()
    {
        Destroy(Instantiate(fadeUncoverPanel), 1f);
    }

    public void NewGame()
    {
        Instantiate(fadeCoverPanel);
        StartCoroutine(LoadGame());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadGame()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main");

        asyncOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(1f); // wait for fade to finish fading
        asyncOperation.allowSceneActivation = true;

        while (!asyncOperation.isDone)
            yield return null;
    }

}
