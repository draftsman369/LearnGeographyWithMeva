using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneByName(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneByIndex(sceneIndex);
        else
            SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}