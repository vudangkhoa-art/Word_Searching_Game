
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WordPlacerTest : MonoBehaviour
{
    [SerializeField] private WordSelector wordSelector;

    private IEnumerator Start()
    {
        yield return null;

        if (wordSelector == null)
            wordSelector = GetComponent<WordSelector>();

        if (wordSelector == null)
        {
            Debug.LogError("WordSelector is missing!");
            yield break;
        }

        DifficultyConfig config =
            DifficultyConfig.Get(DifficultyState.Selected);

        List<string> words =
            wordSelector.GetSelectedWords();

        if (words.Count == 0)
        {
            Debug.LogError("No words were selected!");
            yield break;
        }

        bool success = WordPlacer.TryGenerate(
            words,
            config,
            out char[,] board,
            out List<PlacedWordData> placements
        );

        if (!success)
        {
            Debug.LogError("Puzzle generation failed!");
            yield break;
        }

        StringBuilder output = new StringBuilder();

        output.AppendLine(
            "Generated board: " +
            config.GridSize + "x" + config.GridSize
        );

        for (int y = 0; y < config.GridSize; y++)
        {
            for (int x = 0; x < config.GridSize; x++)
            {
                char letter = board[x, y];

                output.Append(letter == '\0' ? '.' : letter);
                output.Append(' ');
            }

            output.AppendLine();
        }

        Debug.Log(output.ToString());

        foreach (PlacedWordData word in placements)
        {
            Debug.Log(
                $"{word.Word}: " +
                $"{word.Start} -> {word.End}"
            );
        }
    }
}
