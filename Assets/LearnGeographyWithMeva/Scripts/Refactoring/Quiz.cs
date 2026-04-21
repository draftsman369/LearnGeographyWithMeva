using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;

public enum MevaReaction
{
    Happy,
    Sad,
    Neutral
}

[System.Serializable]
public class MevaState
{
    [InspectorName("Reaction")]
    public MevaReaction Mood;
    public Sprite sprite;
}

public class Quiz : MonoBehaviour
{
    public bool questionPassed;

    public static EventHandler OnQuizDone;
    public static EventHandler OnTimerUp;

    public static Quiz Instance;

    [Header("Quiz Data")]
    [SerializeField] private TextAsset questionsCSV;
    public List<QuestionData> questions = new List<QuestionData>();
    public int questionsToAnswer = 10;

    [Header("Answers UI")]
    public List<AnswerContainer> answers;

    [Header("Meva reactions")]
    public List<MevaState> mevaStates = new List<MevaState>();
    public Image currentMeveReaction;

    [Header("Score")]
    public int score;

    [Header("Texts")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI questionCountText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI quizTitleText;

    [Header("Timer")]
    public float timerValue = 7f;
    public float timer;

    public int currentQuestionIndex;

    [Header("Results")]
    public GameObject resultScreen;
    public TextMeshProUGUI resultScoreText;
    public TextMeshProUGUI feedbackText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        AnswerContainer.OnChoiceSelect += GoToNextQuestion;
    }

    private void OnDestroy()
    {
        AnswerContainer.OnChoiceSelect -= GoToNextQuestion;
    }

    private void Start()
    {
        LoadSelectedQuiz();

        if (questions.Count == 0)
        {
            Debug.LogError("No questions loaded.");
            return;
        }

        score = 0;
        scoreText.text = $"{score}";
        currentQuestionIndex = 0;
        timer = timerValue;

        InitQuestion(currentQuestionIndex);
        SetMevaReaction(MevaReaction.Neutral);
    }

    private void LoadSelectedQuiz()
    {
        if (GameManager.Instance != null && GameManager.Instance.selectedQuizCSV != null)
        {
            questionsCSV = GameManager.Instance.selectedQuizCSV;

            if (quizTitleText != null)
                quizTitleText.text = GameManager.Instance.selectedQuizName;
        }

        questions = QuizCSVLoader.LoadFromTextAsset(questionsCSV);
    }

    public void GoToNextQuestion(object sender, EventArgs e)
    {
        scoreText.text = $"{score}";
        currentQuestionIndex++;

        int maxQuestions = Mathf.Min(questions.Count, questionsToAnswer);

        if (currentQuestionIndex >= maxQuestions)
        {
            SetResultScreen();
            OnQuizDone?.Invoke(this, EventArgs.Empty);
            Debug.LogWarning("All questions have been answered.");
        }
        else
        {
            InitQuestion(currentQuestionIndex);
        }
    }

    public void SetResultScreen()
    {
        resultScreen.SetActive(true);
        resultScoreText.text = $"{score}";

        if (PlayerStats.Instance != null)
        {
            int xpBonus = score * 2;
            PlayerStats.Instance.AddXP(xpBonus);
        }
    }

    public void InitQuestion(int index)
    {
        QuestionData currentQuestion = questions[index];

        int maxQuestions = Mathf.Min(questions.Count, questionsToAnswer);
        questionCountText.text = $"Question {index + 1}/{maxQuestions}";
        dialogueText.text = currentQuestion.question;

        for (int i = 0; i < answers.Count; i++)
        {
            if (i < currentQuestion.answers.Count)
            {
                answers[i].gameObject.SetActive(true);
                answers[i].SetAnswer(currentQuestion.answers[i]);
            }
            else
            {
                answers[i].gameObject.SetActive(false);
            }
        }

        timer = timerValue;
        questionPassed = false;
    }

    public void AddScore(int point)
    {
        score += point;
        scoreText.text = $"{score}";
    }

    public int GetXPRewardForCorrectAnswer()
    {
        int baseXP = 10;
        int timeBonusXP = Mathf.FloorToInt(timer);
        return baseXP + timeBonusXP;
    }

    public void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            QuestionData temp = questions[i];
            int randomIndex = UnityEngine.Random.Range(i, questions.Count);
            questions[i] = questions[randomIndex];
            questions[randomIndex] = temp;
        }
    }

    public void SetMevaReaction(MevaReaction reaction)
    {
        MevaState state = mevaStates.Find(s => s.Mood == reaction);
        if (state != null)
        {
            currentMeveReaction.sprite = state.sprite;
        }
    }

    private void Update()
    {
        if (questions.Count == 0 || questionPassed)
            return;

        timer -= Time.deltaTime;

        if (timer < 0f)
            timer = 0f;

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";

        if (timer <= 0f && !questionPassed)
        {
            StartCoroutine(NextQuestionDelay());
        }
    }

    public IEnumerator NextQuestionDelay()
    {
        questionPassed = true;
        SetMevaReaction(MevaReaction.Sad);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayNegativeFeedback();

        dialogueText.text = "Time's up !";

        yield return new WaitForSeconds(3);

        SetMevaReaction(MevaReaction.Neutral);
        GoToNextQuestion(this, EventArgs.Empty);
    }
}