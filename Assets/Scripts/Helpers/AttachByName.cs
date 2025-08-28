using UnityEngine;

public class AttachByName : MonoBehaviour
{
    [SerializeField] private bool isInPlayer = false;
    [SerializeField] private string toAttachName = null;

    private void Start()
    {
        if (isInPlayer)
        {
            Transform playerTransform = PlayerManager.instance.PlayerOrigin.transform;

            if (playerTransform != null)
            {
                Transform attatchTransform = playerTransform.Find(toAttachName);

                if (attatchTransform != null)
                {
                    gameObject.transform.parent = attatchTransform;
                    gameObject.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}