
using System.Collections.Generic;
using UnityEngine;

public class WordChecker : MonoBehaviour
{
    public GameData currentGameData;

    private List<PlacedWordData> placedWords =
        new List<PlacedWordData>();

    private HashSet<string> completedWords =
        new HashSet<string>();

    private List<int> selectedIndexes =
        new List<int>();

    private string selectedWord = "";

    private int rowCount;
    private Vector2Int selectionDirection;
    private bool selectionFinished;
    private bool initialized;

    public int FoundCount => completedWords.Count;
    public int TotalCount => placedWords.Count;

    private void OnEnable()
    {
        GameEvents.OnCheckSquare += SquareSelected;
        GameEvents.OnClearSelection += ClearSelection;
    }

    private void OnDisable()
    {
        GameEvents.OnCheckSquare -= SquareSelected;
        GameEvents.OnClearSelection -= ClearSelection;
    }

    public void SetPlacedWords(
        List<PlacedWordData> words,
        int rows)
    {
        placedWords = new List<PlacedWordData>(words);
        rowCount = rows;

        completedWords.Clear();
        ClearSelection();

        initialized = rowCount > 0 &&
                      placedWords.Count > 0;

        Debug.Log(
            $"WordChecker ready: {placedWords.Count} words"
        );
    }

    private void SquareSelected(
        string letter,
        Vector3 squarePosition,
        int squareIndex)
    {
        if (!initialized || selectionFinished)
            return;

        if (selectedIndexes.Contains(squareIndex))
            return;

        if (selectedIndexes.Count > 0)
        {
            if (!IsValidNextSquare(squareIndex))
                return;
        }

        selectedIndexes.Add(squareIndex);
        selectedWord += letter;

        GameEvents.SelectSquareMethod(squarePosition);

        CheckWord();
    }

    private bool IsValidNextSquare(int nextIndex)
    {
        int previousIndex =
            selectedIndexes[selectedIndexes.Count - 1];

        Vector2Int previous =
            IndexToCell(previousIndex);

        Vector2Int next =
            IndexToCell(nextIndex);

        Vector2Int direction = next - previous;

        if (direction == Vector2Int.zero ||
            Mathf.Abs(direction.x) > 1 ||
            Mathf.Abs(direction.y) > 1)
        {
            return false;
        }

        if (selectedIndexes.Count >= 2)
        {
            if (direction != selectionDirection)
                return false;
        }
        else
        {
            selectionDirection = direction;
        }

        return true;
    }

    private Vector2Int IndexToCell(int index)
    {
        int x = index / rowCount;
        int y = index % rowCount;

        return new Vector2Int(x, y);
    }

    private void CheckWord()
    {
        foreach (PlacedWordData placed in placedWords)
        {
            if (completedWords.Contains(placed.Word))
                continue;

            if (selectedWord != placed.Word)
                continue;

            if (!MatchesCoordinates(placed))
                continue;

            completedWords.Add(placed.Word);
            selectionFinished = true;

            GameEvents.CorrectWordMethod(
                placed.Word,
                new List<int>(selectedIndexes)
            );

            Debug.Log(
                $"Correct Word: {placed.Word} | " +
                $"Progress: {FoundCount}/{TotalCount}"
            );

            if (FoundCount == TotalCount)
            {
                Debug.Log("WIN! All words found!");
            }

            return;
        }
    }

    private bool MatchesCoordinates(PlacedWordData placed)
    {
        if (selectedIndexes.Count != placed.Cells.Count)
            return false;

        for (int i = 0; i < placed.Cells.Count; i++)
        {
            Vector2Int expected = placed.Cells[i];

            int expectedIndex =
                expected.x * rowCount + expected.y;

            if (selectedIndexes[i] != expectedIndex)
                return false;
        }

        return true;
    }

    private void ClearSelection()
    {
        selectedIndexes.Clear();
        selectedWord = "";
        selectionDirection = Vector2Int.zero;
        selectionFinished = false;
    }
}
