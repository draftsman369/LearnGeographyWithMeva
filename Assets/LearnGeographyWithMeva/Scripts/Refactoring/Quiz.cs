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
    [Header("Result Animation")]
    [SerializeField] private float xpCountDuration = 1.5f;
    [SerializeField] private float xpBarDuration = 1.5f;
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
    public int xpEarnedThisQuiz;

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
    public TextMeshProUGUI resultXpText;
    public TextMeshProUGUI resultLevelText;
    public TextMeshProUGUI resultTotalXpText;
    public Slider resultXpSlider;
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
        ShuffleQuestions();

        if (questions.Count == 0)
        {
            Debug.LogError("No questions loaded.");
            return;
        }

        score = 0;
        xpEarnedThisQuiz = 0;
        scoreText.text = $"{score}";
        currentQuestionIndex = 0;
        timer = timerValue;

        if (resultScreen != null)
            resultScreen.SetActive(false);

        InitQuestion(currentQuestionIndex);
        SetMevaReaction(MevaReaction.Neutral);
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

    private void ShuffleAnswers(List<Answer> answerList)
    {
        for (int i = 0; i < answerList.Count; i++)
        {
            Answer temp = answerList[i];
            int randomIndex = UnityEngine.Random.Range(i, answerList.Count);
            answerList[i] = answerList[randomIndex];
            answerList[randomIndex] = temp;
        }
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

        int endQuizBonusXP = score * 2;
        int totalXpGained = xpEarnedThisQuiz + endQuizBonusXP;

        if (PlayerStats.Instance != null)
        {
            int oldLevel = PlayerStats.Instance.currentLevel;
            int oldXP = PlayerStats.Instance.currentXP;
            int oldXPToNextLevel = PlayerStats.Instance.xpToNextLevel;

            PlayerStats.Instance.AddXP(endQuizBonusXP);

            int newLevel = PlayerStats.Instance.currentLevel;
            int newXP = PlayerStats.Instance.currentXP;
            int newXPToNextLevel = PlayerStats.Instance.xpToNextLevel;

            StartCoroutine(AnimateResultXP(
                totalXpGained,
                oldLevel,
                oldXP,
                oldXPToNextLevel,
                newLevel,
                newXP,
                newXPToNextLevel
            ));
        }
        else
        {
            if (resultXpText != null)
                resultXpText.text = $"+{totalXpGained} XP";
        }
    }
    private IEnumerator AnimateResultXP(
            int totalXpGained,
            int oldLevel,
            int oldXP,
            int oldXPToNextLevel,
            int newLevel,
            int newXP,
            int newXPToNextLevel)
        {
            if (resultXpText != null)
                resultXpText.text = "+0 XP";

            if (resultLevelText != null)
                resultLevelText.text = $"Niveau {oldLevel}";

            if (resultTotalXpText != null)
                resultTotalXpText.text = $"{oldXP} / {oldXPToNextLevel} XP";

            if (resultXpSlider != null)
            {
                resultXpSlider.maxValue = oldXPToNextLevel;
                resultXpSlider.value = oldXP;
            }

            float elapsed = 0f;

            while (elapsed < xpCountDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / xpCountDuration);

                int displayedXpGain = Mathf.RoundToInt(Mathf.Lerp(0, totalXpGained, t));

                if (resultXpText != null)
                    resultXpText.text = $"+{displayedXpGain} XP";

                yield return null;
            }

            if (resultXpText != null)
                resultXpText.text = $"+{totalXpGained} XP";

            yield return StartCoroutine(AnimateXpBarAcrossLevels(
                oldLevel,
                oldXP,
                oldXPToNextLevel,
                newLevel,
                newXP,
                newXPToNextLevel
            ));
    }
    private IEnumerator AnimateXpBarAcrossLevels(
        int oldLevel,
        int oldXP,
        int oldXPToNextLevel,
        int newLevel,
        int newXP,
        int newXPToNextLevel)
    {
        if (resultXpSlider == null)
            yield break;

        if (oldLevel == newLevel)
        {
            yield return StartCoroutine(AnimateSingleXpBar(
                oldLevel,
                oldXP,
                oldXPToNextLevel,
                newXP,
                oldXPToNextLevel
            ));
        }
        else
        {
            // Remplir la barre jusqu'à la fin de l'ancien niveau
            yield return StartCoroutine(AnimateSingleXpBar(
                oldLevel,
                oldXP,
                oldXPToNextLevel,
                oldXPToNextLevel,
                oldXPToNextLevel
            ));

            // Gérer les niveaux intermédiaires complets si besoin
            for (int level = oldLevel + 1; level < newLevel; level++)
            {
                int xpNeededForThisLevel = oldXPToNextLevel + ((level - oldLevel) * 50);

                if (resultLevelText != null)
                    resultLevelText.text = $"Niveau {level}";

                resultXpSlider.maxValue = xpNeededForThisLevel;
                resultXpSlider.value = 0;

                if (resultTotalXpText != null)
                    resultTotalXpText.text = $"0 / {xpNeededForThisLevel} XP";

                yield return StartCoroutine(AnimateSingleXpBar(
                    level,
                    0,
                    xpNeededForThisLevel,
                    xpNeededForThisLevel,
                    xpNeededForThisLevel
                ));
            }

            // Dernier niveau atteint
            yield return StartCoroutine(AnimateSingleXpBar(
                newLevel,
                0,
                newXPToNextLevel,
                newXP,
                newXPToNextLevel
            ));
        }
    }

        private IEnumerator AnimateSingleXpBar(
        int displayedLevel,
        int startXP,
        int maxXP,
        int targetXP,
        int displayedMaxXP)
    {
        if (resultLevelText != null)
            resultLevelText.text = $"Niveau {displayedLevel}";

        if (resultXpSlider != null)
        {
            resultXpSlider.maxValue = displayedMaxXP;
            resultXpSlider.value = startXP;
        }

        float elapsed = 0f;

        while (elapsed < xpBarDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / xpBarDuration);

            float currentXP = Mathf.Lerp(startXP, targetXP, t);

            if (resultXpSlider != null)
                resultXpSlider.value = currentXP;

            if (resultTotalXpText != null)
                resultTotalXpText.text = $"{Mathf.RoundToInt(currentXP)} / {displayedMaxXP} XP";

            yield return null;
        }

        if (resultXpSlider != null)
            resultXpSlider.value = targetXP;

        if (resultTotalXpText != null)
            resultTotalXpText.text = $"{targetXP} / {displayedMaxXP} XP";
    }
    public void AddQuizXP(int amount)
    {
        xpEarnedThisQuiz += amount;
    }

    public void InitQuestion(int index)
    {
        QuestionData currentQuestion = questions[index];

        ShuffleAnswers(currentQuestion.answers);

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