using Unity.Mathematics;
using UnityEngine;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        public float maxHealth;
        public float health;
        [SerializeField] private GameObject healthBarMask;
        [SerializeField] private GameObject healthBar;
        public Vector2 positionRangeMin;
        public Vector2 positionRangeMax;

        public void UpdateHealthDisplay(float healthCurr,  float healthMax)
        {
            var healthFloat = Mathf.Clamp01(healthCurr / healthMax);
            healthBarMask.transform.localPosition = Vector2.Lerp(positionRangeMax, positionRangeMin, healthFloat);
        }
    }
}