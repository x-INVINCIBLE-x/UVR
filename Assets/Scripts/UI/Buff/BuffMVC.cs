using UnityEngine;

public class BuffMVC : MonoBehaviour
{
    [SerializeField] private TemporaryBuffs model;
    [SerializeField] private BuffView view;

    private void OnEnable()
    {
        model = TemporaryBuffs.instance;

        if (model != null)
            SetupDisplay();
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            SetupDisplay();
    }
#endif
    public void SetupDisplay()
    {
        if (model == null || view == null)
        {
            Debug.LogError("Model or View is not assigned in BuffMVC.");
            return;
        }

        view.ClearBuffsUI();

        foreach (Buff buff in model.GetCurrentBuffs())
        {
            view.CreateBuffUI(buff.buffName, buff.icon, buff.frontMaterial, buff.iconColor, buff.flavourText);
        }
    }
}
