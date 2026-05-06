using UnityEngine;

namespace UI
{
    public class VectorArrowController : MonoBehaviour
    {
        [SerializeField] private GameObject arrowGrabPoint;
        [SerializeField] private SpriteRenderer arrowSprite;
        public Vector2 arrowVector;

        private void OnDisable()
        {
            arrowSprite.enabled = false;
            arrowGrabPoint.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnEnable()
        {
            arrowSprite.enabled = true;
            arrowGrabPoint.GetComponent<SpriteRenderer>().enabled = true;
        }
    
        private void Update()
        {
            Vector3 direction = arrowGrabPoint.transform.localPosition;

            arrowVector = direction;

            if (arrowVector.magnitude > 8)
            {
                arrowVector.Normalize();
                arrowVector *= 8;
            }
        
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            float scale = arrowVector.magnitude / 2.9f;
        
        
            transform.localRotation = Quaternion.Euler(0, 0, angle);

            transform.localScale = new Vector3(scale, scale, 1) ;
        }
    }
}
