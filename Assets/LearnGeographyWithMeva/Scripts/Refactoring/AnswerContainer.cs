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
    private Color initialColor;
    private Color feedbackColor;

    public bool isRight;

    private void Awake()
    {
        image = GetComponent<Image>();
        initialColor = image.color;
    }

    public void SetAnswer(Answer answer)
    {
        this.answer = answer.content;
        answerText.text = answer.content;
        isRight = answer.isRight;
        image.color = initialColor;
    }

    public void CheckIfTrue()
    {
        if (Quiz.Instance == null || Quiz.Instance.questionPassed)
            return;

        if (isRight)
            WellDone();
        else
            Bad();

        StartCoroutine(NextQuestionDelay());
    }

    private void WellDone()
    {
        Quiz.Instance.SetMevaReaction(MevaReaction.Happy);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPositiveFeedback();

        Quiz.Instance.AddScore(5);

        int totalXP = Quiz.Instance.GetXPRewardForCorrectAnswer();
        int timeBonusXP = Mathf.FloorToInt(Quiz.Instance.timer);

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddXP(totalXP);
        }

        Quiz.Instance.dialogueText.text =
            $"Bonne réponse ! +10 XP + {timeBonusXP} bonus vitesse";

        feedbackColor = Color.green;
    }

    private void Bad()
    {
        Quiz.Instance.SetMevaReaction(MevaReaction.Sad);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayNegativeFeedback();

        Quiz.Instance.dialogueText.text = "Mauvaise réponse";
        feedbackColor = Color.red;
    }

    public IEnumerator NextQuestionDelay()
    {
        image.color = feedbackColor;
        Quiz.Instance.questionPassed = true;

        yield return new WaitForSeconds(3);

        Quiz.Instance.timer = Quiz.Instance.timerValue;
        Quiz.Instance.questionPassed = false;
        Quiz.Instance.SetMevaReaction(MevaReaction.Neutral);
        image.color = initialColor;

        OnChoiceSelect?.Invoke(this, EventArgs.Empty);
    }
}