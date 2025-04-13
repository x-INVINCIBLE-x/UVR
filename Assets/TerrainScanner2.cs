using System.Collections;
using UnityEngine;

public class TerrainScanner2 : MonoBehaviour
{

    public float scanDuration = 3f;
    public float scanSpeed = 2f;
    private Vector3 defaultLocalScale;
    
    //[SerializeField] private bool ScanStatus =false;
    public float lastTimeScanned = -10f;
    private void OnEnable()
    {
        lastTimeScanned = Time.time;
        StartCoroutine(TerrainScan());
    }

    private void Start()
    {
        defaultLocalScale = transform.localScale;
    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    lastTimeScanned = Time.time;
        //    StartCoroutine(TerrainScan());
        //}
    }
   

    private IEnumerator TerrainScan()
    {   
            //Debug.Log(lastTimeScanned + " <->" + Time.time);
        while (lastTimeScanned + scanDuration > Time.time)
        {
            yield return new WaitForEndOfFrame();

            Vector3 originalSize = transform.localScale;
            float spreadGrowth = scanSpeed * Time.deltaTime;
            transform.localScale = new Vector3(originalSize.x + spreadGrowth, originalSize.y + spreadGrowth, originalSize.z + spreadGrowth);
        }
        
        gameObject.SetActive(false);
        transform.localScale = defaultLocalScale;

    }

    

}
