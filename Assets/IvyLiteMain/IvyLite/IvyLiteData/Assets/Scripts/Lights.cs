using UnityEngine;
namespace IvyLite
{
    public class Lights : MonoBehaviour
    {
        public float fadeDuration = 1.0f;
        public Transform lightBillBoard;        
        public Color lightColor = Color.white; // Default color
        public float emissionIntensityMultiplier = 4.0f;
       
        private new Renderer renderer;// new? 
        private MaterialPropertyBlock propBlock;
        private float currentLerpTime;

        private void Awake()
        {
            currentLerpTime = Random.Range(0.0f, fadeDuration * 2.0f);
            renderer = lightBillBoard.gameObject.GetComponent<Renderer>();
            propBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propBlock);         

        }
        void Update()
        {
            currentLerpTime += Time.deltaTime;
            float lerpFactor = Mathf.PingPong(currentLerpTime, fadeDuration) / fadeDuration;

            propBlock.SetColor("_EmissiveColor", lightColor * emissionIntensityMultiplier * lerpFactor);
            propBlock.SetColor("_UnlitColor", new Color(lightColor.r, lightColor.g, lightColor.b, lerpFactor));
            renderer.SetPropertyBlock(propBlock);
            // Billboard effect
            Vector3 dir = Camera.main.transform.position - lightBillBoard.position;
            lightBillBoard.forward = dir.normalized;
        }
    }
}