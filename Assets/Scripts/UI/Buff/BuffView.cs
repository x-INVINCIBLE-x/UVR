using UnityEngine;

public class BuffView : MonoBehaviour
{
    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardView cardPrefab;

    public void CreateCard(string _title, Sprite _icon, Material _frontMaterial, Material _backMaterial, string _description = "")
    {
        CardView newCardView = Instantiate(cardPrefab, cardContainer);
        newCardView.Setup(_title, _icon, _frontMaterial, _backMaterial, _description);
    }
}
