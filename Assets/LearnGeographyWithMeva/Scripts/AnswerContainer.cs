using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class AnswerContainer : MonoBehaviour
{
    public static event EventHandler OnChoiceSelect;

    [TextArea]
    public string answer;
    public TextMeshProUGUI answerText;
    public bool isRight;

    public void SetAnswer(Answer answer)
    {
        this.answer = answer.content;
        this.answerText.text = answer.content;
        this.isRight = answer.isRight;
    }

    public void CheckIfTrue()
    {
        if(isRight)
            WellDone();
        else Bad(); 
        StartCoroutine(NextQuestionDelay());
        //OnChoiceSelect?.Invoke(this, EventArgs.Empty);
    }

    private void WellDone()
    {
        Debug.LogWarning("Well Done !");
        Quiz.Instance.dialogueText.text = "Right";
    }

    private void Bad()
    {
        Quiz.Instance.dialogueText.text = "Wrong";
    }

    public IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(2);
        OnChoiceSelect?.Invoke(this, EventArgs.Empty);

    }
}
