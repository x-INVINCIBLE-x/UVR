using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 10f;

    private Coroutine weaponSpawnRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Item item))
        {
            if (item.data == itemData)
            {
                StopCoroutine(weaponSpawnRoutine);
                weaponSpawnRoutine = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Item item))
        {
            if (item.data != itemData)
            {
                return;
            }

            weaponSpawnRoutine ??= StartCoroutine(SpawnWeapon());
        }
    }

    private IEnumerator SpawnWeapon()
    {
        yield return new WaitForSeconds(spawnDelay);

        GameObject itemInstance = Instantiate(itemData.Model, spawnPoint.position, spawnPoint.rotation);

        if (itemInstance.TryGetComponent(out Rigidbody rb))
            rb.linearVelocity = Vector3.zero;

        weaponSpawnRoutine = null;
    }
}
