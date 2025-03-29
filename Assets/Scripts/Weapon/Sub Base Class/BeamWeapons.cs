using UnityEngine;

public class BeamWeapons : Weapon
{
    // write logic for the beam weapons

    [Header("Beam Settings")]
    
    [SerializeField] protected Transform raySpawn;
    [SerializeField] protected float recoilForce;
    [SerializeField] protected float damage; 
    




    protected virtual void ShootRay()
    {
        ApplyRecoil();
    }



    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

   

    public float GetDamage()
    {
        return damage;
    }
}
