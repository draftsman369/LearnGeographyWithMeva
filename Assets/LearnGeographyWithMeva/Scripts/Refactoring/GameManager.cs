using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Selected Quiz")]
    public TextAsset selectedQuizCSV;
    public string selectedQuizName;

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

    public void SetSelectedQuiz(TextAsset csvFile, string quizName)
    {
        selectedQuizCSV = csvFile;
        selectedQuizName = quizName;
    }

    public void ClearSelectedQuiz()
    {
        selectedQuizCSV = null;
        selectedQuizName = string.Empty;
    }
}