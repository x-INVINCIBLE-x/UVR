using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPanelControls : MonoBehaviour
{
    [SerializeField] private GameObject inventoryCanvas;
    private bool inventortStatus = false;

    public void Menu()
    {

    }

    public void ToggleInventory()
    {
        inventortStatus = !inventortStatus;
        inventoryCanvas.SetActive(inventortStatus);
    }

    public void CloseInventory()
    {
        inventoryCanvas.SetActive(false);
    }

    public void Exit()
    {
        // Go to Home Page or Starting Page
        Application.Quit();
    }
}
