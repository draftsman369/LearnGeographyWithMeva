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
    public List<AnswerContainer> answers;


    //Meva reactions
    public List<MevaState> mevaStates = new List<MevaState>();
    public Image currentMeveReaction;
    public int score;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI questionCountText;
    public TextMeshProUGUI timerText;

    public float timerValue = 7f;
    public float timer;

    public List<QuestionSO> questions;
    public int currentQuestionIndex;

    // <summary>
    public GameObject resultScreen;

    public TextMeshProUGUI resultScoreText;
    public TextMeshProUGUI feedbackText;


    private void Awake()
    {
        if(Instance != null)
            Destroy(this.gameObject);
        Instance = this;

        AnswerContainer.OnChoiceSelect += GoToNextQuestion;
    }

    private void Start()
    {
        scoreText.text = $"{score}";
        currentQuestionIndex = 0;
        timer = timerValue;
        InitQuestion(currentQuestionIndex);
        SetMevaReaction(MevaReaction.Neutral);
    }
    public void GoToNextQuestion(object sender, EventArgs e)
    {
        scoreText.text = $"{score}";
        currentQuestionIndex++;
        if(currentQuestionIndex == questions.Count)
        {
            SetResultScreen();
            OnQuizDone?.Invoke(this, EventArgs.Empty);
            Debug.LogWarning("all of the questions have been answered");         
        }
        else
            InitQuestion(currentQuestionIndex);
    }
    public void SetResultScreen()
    {
        resultScreen.SetActive(true);
        resultScoreText.text = $"{score}";
    }
    public void InitQuestion(int index)
    {
        questionCountText.text = $"Question {currentQuestionIndex + 1}/{questions.Count}";
        dialogueText.text = questions[index].question;
        for(int i = 0; i < answers.Count; i++)
        {        
            answers[i].SetAnswer(questions[index].answers[i]);
        }
    }

    public void AddScore(int point)
    {
        score += 5;
    }

    public void ShuffleQuestions()
    {
        //LogicToShuffleList
    }

    public void SetMevaReaction(MevaReaction reaction)
    {
        currentMeveReaction.sprite = mevaStates.Find(state => state.Mood == reaction).sprite;
    }

    private void Update()
    {
        if(!questionPassed)
            timer -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer/60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";

        if(timerText.text == "00:00" && !questionPassed)
        {
            StartCoroutine(NextQuestionDelay());
        }
    }

    public void ResetTimer()
    {

            GoToNextQuestion(this, EventArgs.Empty);
    }

    public IEnumerator NextQuestionDelay()
    {
        questionPassed = true;
        SetMevaReaction(MevaReaction.Sad);
        AudioManager.Instance.PlayNegativeFeedback();
        dialogueText.text = "Time's up !";
        yield return new WaitForSeconds(3);
        timer = timerValue;
        SetMevaReaction(MevaReaction.Neutral);
        GoToNextQuestion(this, EventArgs.Empty);
        questionPassed = false;
    }

}
