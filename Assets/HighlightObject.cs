using System.Threading;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    private MeshRenderer m_MeshRenderer;
    public Material defaultMaterial;
    public Material highlightmaterial;
    float materialUpTime = 3f;

    private void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        defaultMaterial = m_MeshRenderer.material;
    }
    private void Update()
    {
        //float Timer = Time.deltaTime;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hitscan");
        m_MeshRenderer.material = highlightmaterial;
        Invoke(nameof(ResetMaterial), materialUpTime);
    }

    void ResetMaterial()
    {
        m_MeshRenderer.material = defaultMaterial;
    }
}
