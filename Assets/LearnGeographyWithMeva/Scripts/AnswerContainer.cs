using TMPro;
using UnityEngine;

public class AnswerContainer : MonoBehaviour
{

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
            Debug.LogWarning("Well Done");
        else Debug.LogWarning("Nahh"); 
    }
}
