using System.Collections.Generic;
using UnityEngine;

public class ScatterWeapons : Weapon
{
    [Header("Shotgun Settings")]
    [SerializeField] protected List<Transform> bulletSpawns;
    [SerializeField] protected float shootingForce;
    [SerializeField] protected float recoilForce;
    [SerializeField] protected float damage;
    [SerializeField] protected float spreadAngle = 5f; // Spread angle in degrees


    private void Start()
    {
        InputManager.Instance.activate.action.performed += ctx => ScatterShot();

    }
    protected virtual void ScatterShot()
    {
        ApplyRecoil();
    }



    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    public float GetShootingForce()
    {
        return shootingForce;
    }

    public float GetDamage()
    {
        return damage;
    }
}
