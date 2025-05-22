using UnityEngine;

public class BombExplode : MonoBehaviour
{   
    public GameObject explosionVFX;
    private GameObject currentexplosionVFX;
    private void OnCollisionEnter(Collision collision)
    {   

        currentexplosionVFX = Instantiate(explosionVFX,gameObject.transform);

        Destroy(currentexplosionVFX,2f);
        Destroy(gameObject,0.1f);
    }
}
