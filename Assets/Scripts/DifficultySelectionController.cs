
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard,
    Nightmare
}

public static class DifficultyState
{
    public static GameDifficulty Selected =
        GameDifficulty.Easy;
}

public class DifficultySelectionController : MonoBehaviour
{
    public void SelectEasy()
    {
        StartGame(GameDifficulty.Easy);
    }

    public void SelectMedium()
    {
        StartGame(GameDifficulty.Medium);
    }

    public void SelectHard()
    {
        StartGame(GameDifficulty.Hard);
    }

    public void SelectNightmare()
    {
        StartGame(GameDifficulty.Nightmare);
    }

    private void StartGame(GameDifficulty difficulty)
    {
        DifficultyState.Selected = difficulty;

        Debug.Log("Selected difficulty: " + difficulty);

        SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
