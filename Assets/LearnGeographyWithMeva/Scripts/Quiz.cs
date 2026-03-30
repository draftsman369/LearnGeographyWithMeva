using UnityEngine;
using System.Collections.Generic;

public class Quiz : MonoBehaviour
{
    public static Quiz Instance;
    public List<AnswerContainer> answers;
    
    public List<QuestionSO> questions;

    private void Awake()
    {
        if(Instance != null)
            Destroy(this.gameObject);
        Instance = this;
    }

    private void Start()
    {
        InitQuestion(0);
    }

    public void InitQuestion(int index)
    {
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
