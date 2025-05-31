using UnityEngine;

public class ForwardSlice : WeaponAbilitiesBase
{

    public GameObject SlashVFX;
    protected override void Start()
    {
        base.Start();

    }

    private void Update()
    {
        ActivateSlashEffect();
    }

    private void ActivateSlashEffect()
    {
        float velocity = velocityEstimator.GetVelocityEstimate().magnitude;

        if (velocity > 3)
        {
            Debug.Log(velocity);
            //Instantiate(SlashVFX, gameObject.transform.position,Quaternion.identity);
            //Destroy(SlashVFX,1f);*
            GameObject SlashPool = ObjectPool.instance.GetObject(SlashVFX, transform);
            SlashAudio();

        }

        ObjectPool.instance.ReturnObject(SlashVFX);
    }


}
