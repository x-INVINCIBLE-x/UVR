using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public int currentMoney = 10000;

    public bool TrytoPurchase(int itemCost)
    {
        if(currentMoney > itemCost)
        {   
            currentMoney = currentMoney - itemCost;
            return true; 
        }
        else
        {
            return false;
        }
        
    }
}
