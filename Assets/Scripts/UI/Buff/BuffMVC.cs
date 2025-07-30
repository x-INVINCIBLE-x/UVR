using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffMVC : MonoBehaviour
{
    [SerializeField] private TemporaryBuffs model;
    [SerializeField] private BuffView view;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            SetupDisplay();
    }

    public void SetupDisplay()
    {
        if (model == null || view == null)
        {
            Debug.LogError("Model or View is not assigned in BuffMVC.");
            return;
        }

        foreach (Buff buff in model.GetCurrentBuffs())
        {
            view.CreateCard(buff.buffName, buff.icon, buff.frontMaterial, buff.backMaterial, buff.flavourText);
        }
    }
}
