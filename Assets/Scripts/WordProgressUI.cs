
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordProgressUI : MonoBehaviour
{
    [SerializeField] private Image progressFill;
    [SerializeField] private TMP_Text progressText;

    public void SetProgress(int found, int total)
    {
        float progress = total > 0
            ? Mathf.Clamp01((float)found / total)
            : 0f;

        if (progressFill != null)
            progressFill.fillAmount = progress;

        if (progressText != null)
            progressText.text = $"{found}/{total}";
    }
}
