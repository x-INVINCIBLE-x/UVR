using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;

    [SerializeField] private ChallengeUIDataRegistry uiRegistry;

    private StringBuilder sb = new();

    private void Start()
    {
        Challenge challenge = ChallengeManager.instance.CurrentChallenge;

        ChallengeManager.instance.OnChallengeChoosen += UpdateChallengeUI;
        if (challenge != null)
            UpdateChallengeUI(challenge);
    }

    private void UpdateChallengeUI(Challenge challenge)
    {
        ChallengeUIData data = uiRegistry.GetUIData(challenge.GetID());

        if (data == null)
        {
            Debug.LogWarning($"No UI data found for challenge ID: {challenge.GetID()}");
            return;
        }

        nameText.text = data.displayName;

        sb.AppendLine(data.description);

        if (data.flavourText != null && data.flavourText.Length > 0)
        {
            sb.AppendLine(data.flavourText);
        }
        sb.AppendLine(challenge.GetTechnicalDetail());


        descriptionText.text = sb.ToString();
        iconImage.sprite = data.icon;
    }

    private void OnDestroy()
    {
        ChallengeManager.instance.OnChallengeChoosen -= UpdateChallengeUI;
    }
}