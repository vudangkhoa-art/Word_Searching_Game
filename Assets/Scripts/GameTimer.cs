
using System;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("Timer UI")]
    [SerializeField] private TMP_Text timerText;

    private float remainingSeconds;
    private bool isRunning;

    private Action onTimeUp;

    public float RemainingSeconds => remainingSeconds;
    public bool IsRunning => isRunning;

    private void Update()
    {
        if (!isRunning)
            return;

        remainingSeconds -= Time.deltaTime;

        if (remainingSeconds <= 0f)
        {
            remainingSeconds = 0f;
            UpdateTimerText();

            isRunning = false;

            Action callback = onTimeUp;
            onTimeUp = null;
            callback?.Invoke();

            return;
        }

        UpdateTimerText();
    }

    public void StartCountdown(
        float duration,
        Action timeUpCallback)
    {
        remainingSeconds = Mathf.Max(1f, duration);
        onTimeUp = timeUpCallback;
        isRunning = true;

        UpdateTimerText();

        Debug.Log(
            $"Timer started: {remainingSeconds} seconds"
        );
    }

    public void StopCountdown()
    {
        isRunning = false;
        onTimeUp = null;

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        int totalSeconds =
            Mathf.CeilToInt(remainingSeconds);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text =
            $"{minutes:00}:{seconds:00}";

        timerText.color =
            totalSeconds <= 30
                ? Color.red
                : Color.black;
    }

    private void OnDisable()
    {
        isRunning = false;
    }
}
