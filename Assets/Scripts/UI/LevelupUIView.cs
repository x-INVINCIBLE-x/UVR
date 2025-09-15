using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelupUIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI oldLevelText;
    [SerializeField] private TextMeshProUGUI newLevelText;

    public void SetLevelText(string oldLevel, string newLevel)
    {
        oldLevelText.text = oldLevel;
        newLevelText.text = newLevel;
    }
}
