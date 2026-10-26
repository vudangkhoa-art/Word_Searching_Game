
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class PuzzleBoardValidator
{
    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1)
    };

    public static bool IsValid(
        char[,] board,
        List<PlacedWordData> placements,
        List<string> dictionary,
        out string unexpectedWord)
    {
        unexpectedWord = "";

        if (board == null ||
            placements == null ||
            dictionary == null)
        {
            return false;
        }

        int width = board.GetLength(0);
        int height = board.GetLength(1);
        int maxLength = Mathf.Max(width, height);

        var validWords = new HashSet<string>(
            StringComparer.Ordinal
        );

        foreach (string rawWord in dictionary)
        {
            if (string.IsNullOrWhiteSpace(rawWord))
                continue;

            string word = rawWord.Trim().ToUpperInvariant();

            if (word.Length >= 3 &&
                word.Length <= maxLength)
            {
                validWords.Add(word);
            }
        }

        var expectedOccurrences = new HashSet<string>();
        var actualOccurrences = new HashSet<string>();

        foreach (PlacedWordData placed in placements)
        {
            if (placed.Cells == null ||
                placed.Cells.Count == 0)
            {
                return false;
            }

            string word = placed.Word.ToUpperInvariant();

            validWords.Add(word);

            Vector2Int start = placed.Cells[0];
            Vector2Int end =
                placed.Cells[placed.Cells.Count - 1];

            expectedOccurrences.Add(
                CreateOccurrenceKey(word, start, end)
            );
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foreach (Vector2Int direction in Directions)
                {
                    var builder = new StringBuilder();

                    for (int length = 1;
                         length <= maxLength;
                         length++)
                    {
                        int currentX =
                            x + direction.x * (length - 1);

                        int currentY =
                            y + direction.y * (length - 1);

                        if (currentX < 0 ||
                            currentX >= width ||
                            currentY < 0 ||
                            currentY >= height)
                        {
                            break;
                        }

                        builder.Append(
                            board[currentX, currentY]
                        );

                        if (length < 3)
                            continue;

                        string word = builder.ToString();

                        if (!validWords.Contains(word))
                            continue;

                        var start = new Vector2Int(x, y);
                        var end = new Vector2Int(
                            currentX,
                            currentY
                        );

                        string key = CreateOccurrenceKey(
                            word, start, end
                        );

                        actualOccurrences.Add(key);

                        if (!expectedOccurrences.Contains(key))
                        {
                            unexpectedWord = word;
                            return false;
                        }
                    }
                }
            }
        }

        return actualOccurrences.SetEquals(
            expectedOccurrences
        );
    }

    private static string CreateOccurrenceKey(
        string word,
        Vector2Int start,
        Vector2Int end)
    {

        if (start.x > end.x ||
            (start.x == end.x && start.y > end.y))
        {
            Vector2Int temp = start;
            start = end;
            end = temp;
        }

        return $"{word}|{start.x},{start.y}|" +
               $"{end.x},{end.y}";
    }
}
