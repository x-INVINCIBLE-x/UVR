using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private MeshRenderer front;
    [SerializeField] private MeshRenderer back;

    [SerializeField] private Image iconBg;
    [SerializeField] private Image icon;

    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    public void Setup(string _title, Sprite _icon, Material _frontMaterial, Material _backMaterial, string _description = "")
    {
        title.text = _title;
        icon.sprite = _icon;
        front.material = _frontMaterial;
        back.material = _backMaterial;
        description.text = _description;
        
        iconBg.sprite = _icon;
        iconBg.color = Color.gray;
    }
}
