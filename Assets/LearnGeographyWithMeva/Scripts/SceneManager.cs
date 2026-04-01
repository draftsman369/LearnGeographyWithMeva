using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public Animator animator;
    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
