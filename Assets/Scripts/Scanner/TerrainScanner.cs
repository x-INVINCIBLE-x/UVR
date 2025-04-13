using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TerrainScanner : MonoBehaviour
{

   public AnimationCurve animationCurve;

    public GameObject TerrainScannerPrefab;
    public float scanDuration = 3f;
    public float scanSize = 100f;

    private void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TerrainScan();
        }
    }

    private void TerrainScan()
    {
        GameObject terrainScanner = ObjectPool.instance.GetObject(TerrainScannerPrefab,gameObject.transform);
        ParticleSystem terrainScannerPS = terrainScanner.transform.GetChild(0).GetComponent<ParticleSystem>();

        if(terrainScannerPS != null)
        {
            var main = terrainScannerPS.main;
            main.startSize = scanSize;
            main.startLifetime = scanDuration;
            
            
        }

        ObjectPool.instance.ReturnObject(terrainScanner, scanDuration + 1 );
    }

    


}
