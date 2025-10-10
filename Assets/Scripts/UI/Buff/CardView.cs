using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image iconBg;
    [SerializeField] private Image icon;

    [SerializeField] private TextMeshProUGUI title;
    private string description;

    public void Setup(string _title, Sprite _icon, Material _frontMaterial, Color _iconColor, string _description = "")
    {
        title.text = _title;
        icon.sprite = _icon;
        icon.material = _frontMaterial;
        icon.color = _iconColor;
        description = _description;

        iconBg.sprite = _icon;
        iconBg.color = Color.gray;
    }
}