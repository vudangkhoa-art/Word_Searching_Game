
using System.Collections.Generic;
using UnityEngine;

public class WordSelector : MonoBehaviour
{
    [SerializeField] private WordLibrary wordLibrary;

    private List<string> selectedWords = new List<string>();

    public List<string> GetSelectedWords()
    {
        return new List<string>(selectedWords);
    }

    private void Start()
    {
        if (wordLibrary == null)
            wordLibrary = GetComponent<WordLibrary>();

        if (wordLibrary == null)
        {
            Debug.LogError("WordLibrary is missing!");
            return;
        }

        DifficultyConfig config =
            DifficultyConfig.Get(DifficultyState.Selected);

        SelectWords(config);
    }

    public void SelectWords(DifficultyConfig config)
    {
        selectedWords.Clear();

        List<string> availableWords = GetSafeCandidates(config);

        if (availableWords.Count < config.WordCount)
        {
            Debug.LogError(
                "Not enough words for this difficulty! " +
                $"Need: {config.WordCount}, " +
                $"Available: {availableWords.Count}"
            );
            return;
        }

        for (int i = availableWords.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            string temp = availableWords[i];
            availableWords[i] = availableWords[randomIndex];
            availableWords[randomIndex] = temp;
        }

        for (int i = 0; i < config.WordCount; i++)
        {
            selectedWords.Add(availableWords[i]);
        }

        Debug.Log(
            $"Selected {selectedWords.Count} words for " +
            DifficultyState.Selected
        );

        foreach (string word in selectedWords)
        {
            Debug.Log("Selected Word: " + word);
        }
    }

    
    private List<string> GetSafeCandidates(
        DifficultyConfig config)
    {
        List<string> dictionary =
            wordLibrary.GetWordsByLength(
                3,
                config.GridSize
            );

        List<string> candidates =
            wordLibrary.GetWordsByLength(
                config.MinWordLength,
                config.MaxWordLength
            );

        List<string> safeWords = new List<string>();

        foreach (string candidate in candidates)
        {
            bool hasConflict = false;

            foreach (string otherWord in dictionary)
            {
                if (candidate == otherWord)
                    continue;

                if (candidate.Contains(otherWord) ||
                    candidate.Contains(ReverseWord(otherWord)))
                {
                    hasConflict = true;
                    break;
                }
            }

            if (!hasConflict)
                safeWords.Add(candidate);
        }

        Debug.Log(
            $"Safe word candidates: {safeWords.Count} " +
            $"for {DifficultyState.Selected}"
        );

        return safeWords;
    }

    private static string ReverseWord(string word)
    {
        char[] chars = word.ToCharArray();
        System.Array.Reverse(chars);
        return new string(chars);
    }

}
