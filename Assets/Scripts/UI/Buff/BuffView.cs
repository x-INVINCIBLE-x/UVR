using UnityEngine;

public class BuffView : MonoBehaviour
{
    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardView cardPrefab;

    public void CreateBuffUI(string _title, Sprite _icon, Color _iconColor, string _description = "")
    {
        CardView newCardView = Instantiate(cardPrefab, cardContainer);
        newCardView.Setup(_title, _icon, _iconColor, _description);
    }
}
