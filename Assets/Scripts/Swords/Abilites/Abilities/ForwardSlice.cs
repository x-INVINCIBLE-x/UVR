using UnityEngine;

public class ForwardSlice : WeaponAbilitiesBase
{

    public GameObject SlashVFX;
    public float velovityThreshold = 3f;
    [SerializeField] private Transform slashSpawn;
    private GameObject slashInstance;
    [SerializeField] private float force;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

    }

    private void Update()
    {   
        if(AbilityEnable == true)
        {
            ActivateSlashEffect();
        }
        
    }

    private void ActivateSlashEffect()
    {
        float velocity = velocityEstimator.GetVelocityEstimate().magnitude;

        if (velocity > velovityThreshold)
        {
            Debug.Log(velocity);
            //Instantiate(SlashVFX, gameObject.transform.position,Quaternion.identity);
            //Destroy(SlashVFX,1f);*
            GameObject newSlashVFX = ObjectPool.instance.GetObject(SlashVFX, slashSpawn);
            Rigidbody slashBody = newSlashVFX.GetComponent<Rigidbody>();

            slashBody.linearVelocity = Camera.main.transform.forward * force;

            SlashAudio();
            ObjectPool.instance.ReturnObject(SlashVFX,2f);

        }

        
    }


}
