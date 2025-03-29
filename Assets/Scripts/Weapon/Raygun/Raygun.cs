using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Raygun : BeamWeapons
{
    [SerializeField] private LineRenderer RayPrefab;
    [SerializeField] private float maxRayDistance = 5;
    [SerializeField] private float RayLifeTimer = 10f;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootRay();
        }
        
    }

    protected override void ActivateWeapon(ActivateEventArgs args)
    {
        Debug.Log("rbt");
        base.ActivateWeapon(args);
        ShootRay();

    }

    protected override void DeactivateWeapon(DeactivateEventArgs args)
    {
        base.DeactivateWeapon(args);

    }

    protected override void ShootRay()
    {
        base.ShootRay();

        LineRenderer Ray = Instantiate(RayPrefab);
        Ray.positionCount = 2;
        Ray.SetPosition(0, raySpawn.position);

        Vector3 endPoint = raySpawn.position + raySpawn.forward * maxRayDistance;

        Ray.SetPosition(1, endPoint);

        Destroy(Ray.gameObject, RayLifeTimer);

        
    }
}
