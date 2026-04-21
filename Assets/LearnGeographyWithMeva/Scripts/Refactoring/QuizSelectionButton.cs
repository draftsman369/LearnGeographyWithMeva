using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizSelectionButton : MonoBehaviour
{
    [Header("Quiz Data")]
    [SerializeField] private TextAsset quizCSV;
    [SerializeField] private string quizName;

    [Header("Scene To Load")]
    [SerializeField] private string quizSceneName = "QuizScene";

    public void SelectQuiz()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found.");
            return;
        }

        GameManager.Instance.SetSelectedQuiz(quizCSV, quizName);
        SceneManager.LoadScene(quizSceneName);
    }
}