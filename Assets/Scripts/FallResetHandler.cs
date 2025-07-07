 using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class FallResetHandler : MonoBehaviour
{
    private Fader fader;
    private LayerMask layerMask;

    private void Awake()
    {
        fader = GameObject.Find("Fader_Canvas").GetComponentInChildren<Fader>();
        layerMask = LayerMask.GetMask("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            StartCoroutine(ApplyFade());
        }
    }

    private IEnumerator ApplyFade()
    {
        yield return fader.FadeOut(0.5f);
        yield return null;
        PlayerManager.instance.Player.SetPlayerToSafePosition();
        yield return fader.FadeIn(0.3f);
    }
}
