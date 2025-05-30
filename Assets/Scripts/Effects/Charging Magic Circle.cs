using UnityEngine;
using System.Collections;

public class ChargingMagicCircle : MonoBehaviour
{
    public GameObject[] MagicCircles;
    public float ChargeTimeElapsed;
    public bool isHeld = false;
    

    // Calculation Variables
    private float TotalMagicCircles;    // Total number of circles in the list
    public float MaxChargeTime = 5f;   //  Max Charge Time for the  weapon
    private float circlePerSecond;

    private void Awake()
    {
        TotalMagicCircles = MagicCircles.Length; // Number of Circle in the list that we want to show
        circlePerSecond = MaxChargeTime/TotalMagicCircles; // Circle to be spawned (Time for spawning circle)
      
    }


    private void Update()
    {
        if (isHeld)
        {
            ChargeTimeElapsed += Time.deltaTime;
            EnableMagicCircles();
        }

        else
        {
            ChargeTimeElapsed = 0f;
        }
        
    }

    public void EnableMagicCircles()
    {
        int circlesToEnable = Mathf.FloorToInt(ChargeTimeElapsed / circlePerSecond); // converts float to int and gives which circle to enable in calculation

        for (int i = 0; i < MagicCircles.Length; i++)   
        {
            MagicCircles[i].SetActive(i < circlesToEnable); // As long as the value of the current element of the array is less than circlesToEnable then it will activate
        }
    }


    
}
