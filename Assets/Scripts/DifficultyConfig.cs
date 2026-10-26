
public class DifficultyConfig
{
    public int GridSize { get; private set; }
    public int WordCount { get; private set; }

    public int MinWordLength { get; private set; }
    public int MaxWordLength { get; private set; }

    public bool AllowDiagonal { get; private set; }
    public bool AllowReverse { get; private set; }

    public bool ShowTargetWords { get; private set; }
    public bool UseTimer { get; private set; }

    public static DifficultyConfig Get(GameDifficulty difficulty)
    {
        DifficultyConfig config = new DifficultyConfig();

        switch (difficulty)
        {
            case GameDifficulty.Easy:
                config.GridSize = 7;
                config.WordCount = 5;
                config.MinWordLength = 3;
                config.MaxWordLength = 4;
                config.AllowDiagonal = false;
                config.AllowReverse = false;
                config.ShowTargetWords = true;
                config.UseTimer = false;
                break;

            case GameDifficulty.Medium:
                config.GridSize = 10;
                config.WordCount = 8;
                config.MinWordLength = 5;
                config.MaxWordLength = 6;
                config.AllowDiagonal = true;
                config.AllowReverse = false;
                config.ShowTargetWords = true;
                config.UseTimer = false;
                break;

            case GameDifficulty.Hard:
                config.GridSize = 12;
                config.WordCount = 10;
                config.MinWordLength = 7;
                config.MaxWordLength = 8;
                config.AllowDiagonal = true;
                config.AllowReverse = true;
                config.ShowTargetWords = false;
                config.UseTimer = false;
                break;

            case GameDifficulty.Nightmare:
                config.GridSize = 15;
                config.WordCount = 12;
                config.MinWordLength = 8;
                config.MaxWordLength = 10;
                config.AllowDiagonal = true;
                config.AllowReverse = true;
                config.ShowTargetWords = false;
                config.UseTimer = true;
                break;
        }

        return config;
    }
}
