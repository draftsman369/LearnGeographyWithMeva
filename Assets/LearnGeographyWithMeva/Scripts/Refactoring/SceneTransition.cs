using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionInDuration = 0.8f;
    [SerializeField] private float transitionOutDuration = 0.8f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(PlayTransitionAndLoad(sceneName));
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        if (!isTransitioning)
            StartCoroutine(PlayTransitionAndLoad(sceneIndex));
    }

    private IEnumerator PlayTransitionAndLoad(string sceneName)
    {
        isTransitioning = true;

        transitionAnimator.SetTrigger("StartTransition");
        yield return new WaitForSeconds(transitionInDuration);

        SceneManager.LoadScene(sceneName);
        yield return null;

        transitionAnimator.SetTrigger("EndTransition");
        yield return new WaitForSeconds(transitionOutDuration);

        isTransitioning = false;
    }

    private IEnumerator PlayTransitionAndLoad(int sceneIndex)
    {
        isTransitioning = true;

        transitionAnimator.SetTrigger("StartTransition");
        yield return new WaitForSeconds(transitionInDuration);

        SceneManager.LoadScene(sceneIndex);
        yield return null;

        transitionAnimator.SetTrigger("EndTransition");
        yield return new WaitForSeconds(transitionOutDuration);

        isTransitioning = false;
    }
}