using NUnit.Framework;
using UnityEngine;

namespace FX
{
    public class BackgroundParallaxEffect : MonoBehaviour
    {
        [SerializeField] public float parallaxEffectMultiplier;
        public Transform target;
        private Vector3 _initialOffset;
        void Start()
        {
            if (target == null)
            {
                target = Camera.main.transform;
            }
            _initialOffset = transform.position - target.position;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            transform.position = (target.position *  parallaxEffectMultiplier)  + _initialOffset;
        }
    }

}