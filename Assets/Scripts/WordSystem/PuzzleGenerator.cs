
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    [Header("Word System")]
    [SerializeField] private WordSelector wordSelector;
    [SerializeField] private WordLibrary wordLibrary;

    [Header("Game Data")]
    [SerializeField] private GameData gameDataAsset;

    [Header("GameScene Components")]
    [SerializeField] private WordsGrid wordsGrid;
    [SerializeField] private SearchingWordsList searchingWordsList;
    [SerializeField] private WordChecker wordChecker;

    [SerializeField] private GameManager gameManager;

    private GameData runtimeGameData;
    private BoardData runtimeBoard;

    private List<PlacedWordData> placedWords =
        new List<PlacedWordData>();

    public IReadOnlyList<PlacedWordData> PlacedWords =>
        placedWords;

    private IEnumerator Start()
    {
        yield return null;

        GeneratePuzzle();
    }

    public void GeneratePuzzle()
    {
        if (wordSelector == null)
            wordSelector = GetComponent<WordSelector>();

        if (wordLibrary == null)
        {
            wordLibrary = GetComponent<WordLibrary>();
        }

        if (wordSelector == null || wordLibrary == null ||
            gameDataAsset == null ||
            wordsGrid == null ||
            wordChecker == null)
        {
            Debug.LogError(
                "PuzzleGenerator: Missing references!"
            );
            return;
        }

        DifficultyConfig config =
            DifficultyConfig.Get(DifficultyState.Selected);


        char[,] board;
        List<PlacedWordData> placements;

        bool success = TryGenerateCleanBoard(
            config,
            out board,
            out placements
        );

        if (!success)
        {
            Debug.LogError(
                "Could not generate a clean puzzle. " +
                "Try expanding or reviewing the word library."
            );
            return;
        }


        runtimeGameData = Instantiate(gameDataAsset);

        runtimeBoard =
            ScriptableObject.CreateInstance<BoardData>();

        runtimeBoard.Columns = config.GridSize;
        runtimeBoard.Rows = config.GridSize;

        runtimeBoard.CreateNewBoard();

        for (int x = 0; x < config.GridSize; x++)
        {
            for (int y = 0; y < config.GridSize; y++)
            {
                runtimeBoard.Board[x].Row[y] =
                    board[x, y].ToString();
            }
        }

        runtimeBoard.SearchWords.Clear();

        foreach (PlacedWordData placed in placements)
        {
            runtimeBoard.SearchWords.Add(
                new BoardData.SearchingWord
                {
                    Word = placed.Word
                }
            );
        }

        placedWords = placements;

        runtimeGameData.selectedBoardData = runtimeBoard;

        wordsGrid.currentGameData = runtimeGameData;
        wordChecker.currentGameData = runtimeGameData;

        wordChecker.SetPlacedWords(placedWords, config.GridSize);

        if (gameManager != null)
        {
            gameManager.Initialize(placedWords.Count);
        }
        else
        {
            Debug.LogWarning("GameManager has not been assigned!");
        }

        if (searchingWordsList != null)
        {
            searchingWordsList.currentGameData =
                runtimeGameData;
        }

        wordsGrid.BuildGrid();

        
        if (searchingWordsList != null)
        {
            searchingWordsList.gameObject.SetActive(true);

            searchingWordsList.BuildList();

            searchingWordsList.ConfigureHints(
                config.ShowTargetWords
            );
        }


        Debug.Log(
            $"Puzzle generated successfully! " +
            $"Difficulty: {DifficultyState.Selected}, " +
            $"Grid: {config.GridSize}x{config.GridSize}, " +
            $"Words: {placedWords.Count}"
        );
    }

    
    private bool TryGenerateCleanBoard(
        DifficultyConfig config,
        out char[,] resultBoard,
        out List<PlacedWordData> resultPlacements)
    {
        resultBoard = null;
        resultPlacements = null;

        List<string> dictionary =
            wordLibrary.GetWordsByLength(
                3,
                config.GridSize
            );

        const int maxWordSets = 12;
        const int maxLayoutsPerSet = 6;
        const int maxFillAttempts = 25;

        for (int wordSetTry = 0;
            wordSetTry < maxWordSets;
            wordSetTry++)
        {
            List<string> selectedWords =
                wordSelector.GetSelectedWords();

            if (selectedWords == null ||
                selectedWords.Count != config.WordCount)
            {
                wordSelector.SelectWords(config);
                continue;
            }

            for (int layoutTry = 0;
                layoutTry < maxLayoutsPerSet;
                layoutTry++)
            {
                bool placed = WordPlacer.TryGenerate(
                    selectedWords,
                    config,
                    out char[,] placedBoard,
                    out List<PlacedWordData> placements
                );

                if (!placed)
                    continue;

                for (int fillTry = 0;
                    fillTry < maxFillAttempts;
                    fillTry++)
                {
                    char[,] candidate =
                        (char[,])placedBoard.Clone();

                    FillEmptyCells(candidate);

                    bool valid = PuzzleBoardValidator.IsValid(
                        candidate,
                        placements,
                        dictionary,
                        out string unexpectedWord
                    );

                    if (valid)
                    {
                        resultBoard = candidate;
                        resultPlacements = placements;

                        Debug.Log(
                            "Clean puzzle generated. " +
                            "All dictionary words validated."
                        );

                        return true;
                    }
                }
            }

            wordSelector.SelectWords(config);
        }

        return false;
    }


    private void FillEmptyCells(char[,] board)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        int width = board.GetLength(0);
        int height = board.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] == '\0')
                {
                    int index = Random.Range(
                        0,
                        letters.Length
                    );

                    board[x, y] = letters[index];
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (runtimeGameData != null)
            Destroy(runtimeGameData);

        if (runtimeBoard != null)
            Destroy(runtimeBoard);
    }
}
