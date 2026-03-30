using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerContainer : MonoBehaviour
{
    public static event EventHandler OnChoiceSelect;

    [TextArea]
    public string answer;
    public TextMeshProUGUI answerText;

    private Image image;
    Color initialColor;
    Color feedbackColor;
    public bool isRight;

    private void Awake()
    {
        image = this.GetComponent<Image>();
        initialColor = image.color;
    }

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
        AudioManager.Instance.PlayPositiveFeedback();
        Debug.LogWarning("Well Done !");
        Quiz.Instance.AddScore(5);
        Quiz.Instance.dialogueText.text = "Right";
        feedbackColor = Color.green;
    }

    private void Bad()
    {
        AudioManager.Instance.PlayNegativeFeedback();
        Quiz.Instance.dialogueText.text = "Wrong";
        feedbackColor = Color.red;
    }

    public IEnumerator NextQuestionDelay()
    {
        image.color = feedbackColor;
        yield return new WaitForSeconds(2);
        image.color = initialColor;
        OnChoiceSelect?.Invoke(this, EventArgs.Empty);

    }
}
