using System;
using System.Collections.Generic;
using UnityEngine;

public static class QuizCSVLoader
{
    public static List<QuestionData> LoadFromTextAsset(TextAsset csvFile)
    {
        List<QuestionData> questions = new List<QuestionData>();

        if (csvFile == null)
        {
            Debug.LogError("QuizCSVLoader: CSV file is null.");
            return questions;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1)
        {
            Debug.LogWarning("QuizCSVLoader: CSV file is empty or only contains a header.");
            return questions;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] columns = line.Split(';');

            if (columns.Length < 9)
            {
                Debug.LogWarning($"QuizCSVLoader: Invalid line {i + 1}: {line}");
                continue;
            }

            QuestionData questionData = new QuestionData();
            questionData.question = columns[0].Trim();

            for (int j = 1; j < 9; j += 2)
            {
                Answer answer = new Answer
                {
                    content = columns[j].Trim(),
                    isRight = bool.TryParse(columns[j + 1].Trim(), out bool parsedValue) && parsedValue
                };

                questionData.answers.Add(answer);
            }

            questions.Add(questionData);
        }

        return questions;
    }
}