using Unity.VisualScripting;
using UnityEngine;

public class ScannerActivater : MonoBehaviour
{
    public GameObject terrainScanner;
    [SerializeField] private float coolDownTimer = 8f;
    private float currentCoolDownTime = 0f;

    private float lastActivationTime = -10f;

    private void Start()
    {   
        
        terrainScanner.SetActive(false);
        InputManager.Instance.XHold.action.performed += ctx => TryActivateScanner();
    }


    void TryActivateScanner()
    {
        

        if (coolDownTimer + lastActivationTime < Time.time)
        {
            lastActivationTime = Time.time;
            terrainScanner.SetActive(true);
            
        }
    }


}
