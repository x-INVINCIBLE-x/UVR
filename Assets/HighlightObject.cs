using System.Threading;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    public MeshRenderer[] Meshes;
    public Material[] defaultMaterial;
    private MeshRenderer m_MeshRenderer;
    
    //private Material defaultMaterial;
    public Material highlightmaterial;
    float materialUpTime = 3f;

    private void Awake()
    {   
        Meshes = GetComponentsInChildren<MeshRenderer>();
        defaultMaterial = new Material[Meshes.Length];
        for (int i = 0; i < Meshes.Length; i++)
        {
            defaultMaterial[i] = Meshes[i].material;
            Debug.Log("Mat" + Meshes[i].material.name);
        }
        //defaultMaterial = GetComponentsInChildren<Material>();
        //m_MeshRenderer = GetComponent<MeshRenderer>();
        //defaultMaterial = m_MeshRenderer.material;
    }
    private void Update()
    {
        //float Timer = Time.deltaTime;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hitscan");
        if (other.GetComponent<TerrainScanner2>() == null) return;
        foreach (MeshRenderer renderer in Meshes)
        {
            renderer.material = highlightmaterial;
        }

        //m_MeshRenderer.material = highlightmaterial;
        Invoke(nameof(ResetMaterial), materialUpTime);
    }

    void ResetMaterial()
    {   
       for(int i = 0; i < Meshes.Length; i++)
       {
            Meshes[i].material = defaultMaterial[i];
       }
    }
}
