using System.Collections;
using UnityEngine;

public class FallResetHandler : MonoBehaviour
{
    private Fader fader;
    private LayerMask layerMask;
    [SerializeField] private LayerMask weaponLayer;
    private float radius = 3f;
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
        int objLayer = other.gameObject.layer;

        if ((layerMask & (1 << objLayer)) != 0 && !isFading && fader != null && PlayerManager.instance != null)
        {
            StartCoroutine(ApplyFade());
        }

        else if ((weaponLayer & (1 << objLayer)) != 0)
        {
            RespawnWeapon(other.gameObject);
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

    private void RespawnWeapon(GameObject weapon)
    {
        Vector3 safePos = PlayerManager.instance.Player.GetSafePosition();
        Vector3 spawnPos = FindValidPositionNear(safePos, radius);

        weapon.transform.position = spawnPos;
        weapon.GetComponent<Rigidbody>()?.WakeUp();

        Debug.Log($"Weapon '{weapon.name}' respawned at {spawnPos}");
    }

    private Vector3 FindValidPositionNear(Vector3 center, float radius)
    {
        int maxAttempts = 10;
        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 testPos = center + offset + Vector3.up * 5f;

            if (Physics.Raycast(testPos, Vector3.down, out RaycastHit hit, 20f, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    return hit.point + Vector3.up * 0.2f;
                }
            }
        }

        Debug.LogWarning("Could not find valid ground position for weapon respawn.");
        return center + Vector3.up * 1f;
    }
}
