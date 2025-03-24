using UnityEngine;

public class UIClose : MonoBehaviour
{
    [SerializeField] private GameObject panelToClose;

    public void Close()
    {
        RectTransform[] panelsToClose = GetComponentsInChildren<RectTransform>();
        Debug.Log(panelsToClose.Length);
        //foreach (ICloseable panel in panelsToClose)
        //{
        //    panel.OnClose();
        //}
    }
}
