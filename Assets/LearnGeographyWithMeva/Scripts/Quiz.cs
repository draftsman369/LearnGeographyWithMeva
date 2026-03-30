using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class Quiz : MonoBehaviour
{
    public static Quiz Instance;
    public List<AnswerContainer> answers;
    

    public TextMeshProUGUI dialogueText;
    public List<QuestionSO> questions;
    public int currentQuestionIndex;

    private void Awake()
    {
        if(Instance != null)
            Destroy(this.gameObject);
        Instance = this;

        AnswerContainer.OnChoiceSelect += GoToNextQuestion;
    }

    private void Start()
    {
        currentQuestionIndex = 0;
        InitQuestion(currentQuestionIndex);
    }
    public void GoToNextQuestion(object sender, EventArgs e)
    {
        currentQuestionIndex++;
        if(currentQuestionIndex == questions.Count)
            Debug.LogWarning("all of the questions have been answered");
        else
            InitQuestion(currentQuestionIndex);
    }

    public void InitQuestion(int index)
    {
        dialogueText.text = questions[index].question;
        for(int i = 0; i < answers.Count; i++)
        {        
            answers[i].SetAnswer(questions[index].answers[i]);
        }
    }



    public void ShuffleQuestions()
    {
        //LogicToShuffleList
    }
}
