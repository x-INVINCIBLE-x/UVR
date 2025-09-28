#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class QuitOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
        }
    }
}
