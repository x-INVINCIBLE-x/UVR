using System.Collections;
using UnityEngine;

public class FallResetHandler : MonoBehaviour
{
    private Fader fader;
    private LayerMask layerMask;
    private bool isFading = false;

    private void Awake()
    {
        GameObject faderCanvas = GameObject.Find("Fader_Canvas");
        if (faderCanvas == null)
        {
            Debug.LogError("Fader_Canvas not found in scene!");
            return;
        }

        fader = faderCanvas.GetComponentInChildren<Fader>();
        if (fader == null)
        {
            Debug.LogError("Fader component not found on Fader_Canvas!");
        }

        layerMask = LayerMask.GetMask("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask & (1 << other.gameObject.layer)) != 0 && !isFading && fader != null && PlayerManager.instance != null)
        {
            StartCoroutine(ApplyFade());
        }
    }

    private IEnumerator ApplyFade()
    {
        if (isFading) yield break;

        isFading = true;

        yield return fader.FadeOut(0.5f);

        yield return new WaitForEndOfFrame(); 

        PlayerManager.instance.Player.SetPlayerToSafePosition();
        Debug.Log("Player has fallen and is being reset.");

        yield return new WaitForSeconds(0.05f);
        yield return fader.FadeIn(0.3f);

        isFading = false;
    }
}
