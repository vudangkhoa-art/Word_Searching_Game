
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Gameplay")]
    [SerializeField] private WordChecker wordChecker;
    [SerializeField] private WordProgressUI progressUI;

    [Header("Nightmare Timer")]
    [SerializeField] private GameTimer gameTimer;

    [SerializeField, Min(1f)]
    private float nightmareDuration = 600f;

    [Header("Nightmare Hints")]
    [SerializeField]
    private SearchingWordsList searchingWordsList;

    [SerializeField, Min(1f)]
    private float hintInterval = 60f;

    private float hintElapsedTime;

    private readonly HashSet<string> foundWords =
        new HashSet<string>();

    private int totalWords;
    private bool gameFinished;
    private bool initialized;

    public int FoundCount => foundWords.Count;
    public int TotalWords => totalWords;
    public bool GameFinished => gameFinished;

    private void OnEnable()
    {
        GameEvents.OnCorrectWord += HandleCorrectWord;
    }

    private void OnDisable()
    {
        GameEvents.OnCorrectWord -= HandleCorrectWord;
    }

    
    private void Update()
    {
        if (!initialized || gameFinished)
            return;

        if (DifficultyState.Selected !=
            GameDifficulty.Nightmare)
        {
            return;
        }

        if (gameTimer == null || !gameTimer.IsRunning)
            return;

        hintElapsedTime += Time.deltaTime;

        if (hintElapsedTime >= hintInterval)
        {
            hintElapsedTime -= hintInterval;

            if (searchingWordsList != null)
            {
                searchingWordsList.RevealRandomHint();
            }
        }
    }


    public void Initialize(int wordCount)
    {
        foundWords.Clear();
        hintElapsedTime = 0f;

        totalWords = wordCount;
        gameFinished = false;
        initialized = wordCount > 0;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (wordChecker != null)
            wordChecker.enabled = true;

        if (progressUI != null)
            progressUI.SetProgress(0, totalWords);

        DifficultyConfig config =
            DifficultyConfig.Get(DifficultyState.Selected);

        bool shouldUseTimer =
            initialized && config.UseTimer;

        if (gameTimer != null)
        {
            gameTimer.StopCountdown();

            gameTimer.gameObject.SetActive(shouldUseTimer);

            if (shouldUseTimer)
            {
                gameTimer.StartCountdown(
                    nightmareDuration,
                    HandleTimeUp
                );
            }
        }
        else if (shouldUseTimer)
        {
            Debug.LogError(
                "GameTimer has not been assigned!"
            );
        }

        Debug.Log(
            $"GameManager initialized: 0/{totalWords}"
        );
    }

    private void HandleCorrectWord(
        string word,
        List<int> squareIndexes)
    {
        if (!initialized || gameFinished)
            return;

        if (!foundWords.Add(word))
            return;

        if (progressUI != null)
        {
            progressUI.SetProgress(
                FoundCount,
                totalWords
            );
        }

        Debug.Log(
            $"Game Progress: {FoundCount}/{totalWords}"
        );

        if (FoundCount >= totalWords)
        {
            CompleteGame();
        }
    }

    private void CompleteGame()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        if (gameTimer != null)
            gameTimer.StopCountdown();

        if (wordChecker != null)
            wordChecker.enabled = false;

        Debug.Log("GAME WON!");

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    private void HandleTimeUp()
    {
        if (!initialized || gameFinished)
            return;

        gameFinished = true;

        if (gameTimer != null)
            gameTimer.StopCountdown();

        if (wordChecker != null)
            wordChecker.enabled = false;

        Debug.Log("TIME'S UP! GAME LOST!");

        if (losePanel != null)
            losePanel.SetActive(true);
        else
            Debug.LogWarning("LosePanel is missing!");
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void BackToDifficulty()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            "DifficultySelection"
        );
    }
}
