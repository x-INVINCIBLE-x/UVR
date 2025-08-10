using Unity.VisualScripting;
using UnityEngine;

public class CharacterStatusVfx : MonoBehaviour
{
    [Header("Status Effect VFXs")]
    [Space]
    [SerializeField] private GameObject BurnStatusEffect;
    [SerializeField] private GameObject FreezeStatusEffect;
    [SerializeField] private GameObject DrainStatusEffect;
    [SerializeField] private GameObject ShockStatusEffect;
    [SerializeField] private GameObject BlightStatusEffect;
    [SerializeField] private GameObject FrenzyStatusEffect;

    private GameObject currentStatusEffect;


    private void Awake()
    {
        BurnStatusEffect.SetActive(false);
        FreezeStatusEffect.SetActive(false);
        DrainStatusEffect.SetActive(false);
        ShockStatusEffect.SetActive(false);
        BlightStatusEffect.SetActive(false);
        FrenzyStatusEffect.SetActive(false);
    }

    public void SpawnStatusVFX(AilmentType type,bool Activate = true)
    {
        switch (type)
        {
            case AilmentType.Ignis:
                currentStatusEffect = BurnStatusEffect;
                break;
            case AilmentType.Frost:
                currentStatusEffect = FreezeStatusEffect;
                break;
            case AilmentType.Gaia:
                currentStatusEffect = DrainStatusEffect;
                break;
            case AilmentType.Blitz:
                currentStatusEffect = ShockStatusEffect;
                break;
            case AilmentType.Radiance:
                currentStatusEffect = BlightStatusEffect;
                break;
            case AilmentType.Hex:
                currentStatusEffect = FrenzyStatusEffect;
                break;
        }

        if (Activate)
        {            
            currentStatusEffect.SetActive(true);
        }
    
        else if(!Activate)
        {
            currentStatusEffect.SetActive(false);
        }

    }
         
}
