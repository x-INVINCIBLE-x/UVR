using UnityEngine;
namespace IvyPro
{
    public class LightFadeInOut : MonoBehaviour
    {
        public float fadeDuration = 1.0f;
        public Transform lightBillBoard;
        public Light pointLight;
        public Color lightColor = Color.white; // Default color
        public float emissionIntensityMultiplier = 4.0f;
        public float trueLightIntensity;
        public bool trueLight;
        private new Renderer renderer;
        private MaterialPropertyBlock propBlock;
        private float currentLerpTime;

        private void Awake()
        {
            currentLerpTime = Random.Range(0.0f, fadeDuration * 2.0f);
            pointLight.gameObject.SetActive(trueLight);

            renderer = lightBillBoard.gameObject.GetComponent<Renderer>();
            propBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propBlock);
            // Set the initial color of the light and the emissive material
            if (trueLight) pointLight.color = lightColor;

        }

        void Update()
        {
            currentLerpTime += Time.deltaTime;
            float lerpFactor = Mathf.PingPong(currentLerpTime, fadeDuration) / fadeDuration;

            propBlock.SetColor("_EmissiveColor", lightColor * emissionIntensityMultiplier * lerpFactor);
            propBlock.SetColor("_UnlitColor", new Color(lightColor.r, lightColor.g, lightColor.b, lerpFactor));
            renderer.SetPropertyBlock(propBlock);

            if (trueLight)
            {
                pointLight.intensity = trueLightIntensity * lerpFactor;
                pointLight.color = lightColor;
            }

            // Billboard effect
            Vector3 dir = Camera.main.transform.position - lightBillBoard.position;
            lightBillBoard.forward = dir.normalized;
        }
    }
}