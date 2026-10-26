
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlacedWordData
{
    public string Word;
    public Vector2Int Start;
    public Vector2Int End;
    public Vector2Int Direction;
    public List<Vector2Int> Cells = new List<Vector2Int>();
}

public static class WordPlacer
{
    private const int MaxPlacementAttempts = 200;
    private const int MaxBoardAttempts = 40;

    public static bool TryGenerate(
        List<string> selectedWords,
        DifficultyConfig config,
        out char[,] board,
        out List<PlacedWordData> placedWords)
    {
        board = null;
        placedWords = null;

        if (config == null ||
            selectedWords == null ||
            selectedWords.Count == 0 ||
            config.GridSize <= 0)
        {
            Debug.LogError("Invalid puzzle configuration or word list.");
            return false;
        }

        int size = config.GridSize;

        List<string> sortedWords =
            new List<string>(selectedWords);

        sortedWords.Sort(
            (a, b) => b.Length.CompareTo(a.Length)
        );

        List<Vector2Int> directions =
            GetAllowedDirections(config);

        for (int attempt = 0;
             attempt < MaxBoardAttempts;
             attempt++)
        {
            char[,] newBoard = new char[size, size];

            List<PlacedWordData> newPlacements =
                new List<PlacedWordData>();

            bool success = true;

            foreach (string word in sortedWords)
            {
                if (string.IsNullOrEmpty(word) ||
                    word.Length > size ||
                    !TryPlaceWord(
                        word,
                        newBoard,
                        directions,
                        newPlacements))
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                board = newBoard;
                placedWords = newPlacements;
                return true;
            }
        }

        Debug.LogWarning(
            "Cannot place all selected words. " +
            "Try selecting a different word set."
        );

        return false;
    }

    private static List<Vector2Int> GetAllowedDirections(
        DifficultyConfig config)
    {
        List<Vector2Int> directions =
            new List<Vector2Int>
            {
                new Vector2Int(1, 0), // Right
                new Vector2Int(0, 1)  // Down
            };

        if (config.AllowDiagonal)
        {
            directions.Add(new Vector2Int(1, 1));
            directions.Add(new Vector2Int(-1, 1));
        }

        if (config.AllowReverse)
        {
            directions.Add(new Vector2Int(-1, 0));
            directions.Add(new Vector2Int(0, -1));
            directions.Add(new Vector2Int(-1, -1));
            directions.Add(new Vector2Int(1, -1));
        }

        return directions;
    }

    private static bool TryPlaceWord(
        string word,
        char[,] board,
        List<Vector2Int> directions,
        List<PlacedWordData> placements)
    {
        int size = board.GetLength(0);

        for (int attempt = 0;
             attempt < MaxPlacementAttempts;
             attempt++)
        {
            Vector2Int direction =
                directions[UnityEngine.Random.Range(
                    0, directions.Count
                )];

            int startX = UnityEngine.Random.Range(0, size);
            int startY = UnityEngine.Random.Range(0, size);

            int endX = startX +
                (word.Length - 1) * direction.x;

            int endY = startY +
                (word.Length - 1) * direction.y;

            if (endX < 0 || endX >= size ||
                endY < 0 || endY >= size)
            {
                continue;
            }

            bool canPlace = true;
            int emptyCells = 0;

            for (int i = 0; i < word.Length; i++)
            {
                int x = startX + i * direction.x;
                int y = startY + i * direction.y;

                char existing = board[x, y];

                if (existing != '\0' &&
                    existing != word[i])
                {
                    canPlace = false;
                    break;
                }

                if (existing == '\0')
                    emptyCells++;
            }

            if (!canPlace || emptyCells == 0)
                continue;

            PlacedWordData placement =
                new PlacedWordData
                {
                    Word = word,
                    Start = new Vector2Int(startX, startY),
                    End = new Vector2Int(endX, endY),
                    Direction = direction
                };

            for (int i = 0; i < word.Length; i++)
            {
                int x = startX + i * direction.x;
                int y = startY + i * direction.y;

                board[x, y] = word[i];

                placement.Cells.Add(
                    new Vector2Int(x, y)
                );
            }

            placements.Add(placement);
            return true;
        }

        return false;
    }
}
