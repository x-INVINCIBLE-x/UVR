using UnityEngine;

public class ActivateHands_ToRemove : MonoBehaviour
{
    public Transform LeftHand;
    public Transform RightHand;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            LeftHand.gameObject.SetActive(!LeftHand.gameObject.activeSelf);
            RightHand.gameObject.SetActive(!RightHand.gameObject.activeSelf);
        }
    }
}
