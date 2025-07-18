using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class BuffView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buffNameText;
    [SerializeField] private TextMeshProUGUI buffFlavourText;
    [SerializeField] private TextMeshProUGUI buffModifierText;
    [SerializeField] private TextMeshProUGUI buffEffectorText;
    [SerializeField] private TextMeshProUGUI buffActivationText;

    [SerializeField] private Image buffIcon;

    public void Setup(Buff buff)
    {
        if (buff == null) return;

        buffNameText.text = buff.GetName();
        buffFlavourText.text = buff.GetFlavourText();
        buffModifierText.text = buff.GetModifierInfo();
        buffEffectorText.text = buff.GetEffectorInfo();
        buffActivationText.text = buff.GetActivationInfo();

        //buffDescriptionText.text = buff.;
        //buffIcon.sprite = buff.icon;
    }
}
