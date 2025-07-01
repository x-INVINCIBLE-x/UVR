using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DungeonBuffProvider : MonoBehaviour
{
    [SerializeField] private Buff buffToApply;
    private DungeonBuffHandler handler;
    private BuffMaterialInfo materialInfo;

    private void Start()
    {
        DungeonManager.Instance.RegisterBuffPortal(this);
    }

    public void Initialize(DungeonBuffHandler _handler, Buff _buff, BuffMaterialInfo _materialInfo)
    {
        handler = _handler;
        buffToApply = _buff;
        materialInfo = _materialInfo;

        GetComponent<Renderer>().material = materialInfo.baseMaterial;
        GetComponent<BoxCollider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TemporaryBuffs tempBuff = other.GetComponentInParent<TemporaryBuffs>();
        if (tempBuff != null)
        {
            tempBuff.AddBuff(buffToApply);
            handler.BuffPicked();
        }
    }

    public void Close()
    {
        StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        GetComponent<Renderer>().material = materialInfo.dissolveMaterial;
        GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(handler.Dissolve_Duration);
        gameObject.SetActive(false);
    }
}
