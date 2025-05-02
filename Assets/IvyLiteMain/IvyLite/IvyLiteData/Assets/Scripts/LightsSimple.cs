
using UnityEngine;

namespace IvyLite
{
    public class LightsSimple : MonoBehaviour
    {
        public Transform lightBillBoard;

        void Update()
        {
            Vector3 dir = Camera.main.transform.position - lightBillBoard.position;
            lightBillBoard.forward = dir.normalized;
        }
    }
}