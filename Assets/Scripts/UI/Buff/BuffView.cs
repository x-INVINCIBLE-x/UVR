using UnityEngine;

public class BuffView : MonoBehaviour
{
    [SerializeField] private Transform cardContainer;
    [SerializeField] private CardView cardPrefab;

    public void CreateBuffUI(string _title, Sprite _icon, Material _frontMaterial, Color _iconColor, string _description = "")
    {
        CardView newCardView = Instantiate(cardPrefab, cardContainer);
        newCardView.Setup(_title, _icon, _frontMaterial, _iconColor, _description);
    }

    public void ClearBuffsUI()
    {
        int n = cardContainer.childCount;
        for (int i = n-1; i >= 0; i--)
        {
            Destroy(cardContainer.GetChild(i).gameObject);
        }
    }
}
