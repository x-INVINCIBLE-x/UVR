using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FallResetHandler : MonoBehaviour
{
    public Fader fader;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
