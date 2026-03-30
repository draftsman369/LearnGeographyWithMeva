using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionSO", menuName = "Scriptable Objects/QuestionSO")]
public class QuestionSO : ScriptableObject
{
    [TextArea]
    public string question;
    public List<Answer> answers;
}
