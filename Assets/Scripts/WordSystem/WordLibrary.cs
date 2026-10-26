using System;
using System.Collections.Generic;
using UnityEngine;

public class WordLibrary : MonoBehaviour
{
    [SerializeField] private TextAsset wordFile;

    private readonly HashSet<string> words =
        new HashSet<string>();

    public int WordCount => words.Count;

    private void Awake()
    {
        LoadWords();
    }

    public void LoadWords()
    {
        words.Clear();

        if (wordFile == null)
        {
            Debug.LogError("Word file has not been assigned!");
            return;
        }

        string[] lines = wordFile.text.Split(
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
        );

        foreach (string line in lines)
        {
            string word = line.Trim().ToUpperInvariant();

            if (word.Length < 2)
                continue;

            bool isValid = true;

            foreach (char character in word)
            {
                if (character < 'A' || character > 'Z')
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
                words.Add(word);
        }

        Debug.Log(
            $"Word Library loaded: {words.Count} words"
        );
    }

    public List<string> GetWordsByLength(
        int minLength,
        int maxLength)
    {
        List<string> result = new List<string>();

        foreach (string word in words)
        {
            if (word.Length >= minLength &&
                word.Length <= maxLength)
            {
                result.Add(word);
            }
        }

        return result;
    }

    public bool Contains(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        return words.Contains(
            word.Trim().ToUpperInvariant()
        );
    }
}
