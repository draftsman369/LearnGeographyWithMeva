using System;
using System.Collections.Generic;

[Serializable]
public class QuestionData
{
    public string question;
    public List<Answer> answers = new List<Answer>();
}