using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace FX
{
    public class PlanetDestroyFxManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> shatterPieces = new List<GameObject>();
        [SerializeField] private float forceMultiplier;
        
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }


        // ReSharper disable Unity.PerformanceAnalysis
        public void Explode(float force, float m)
        {
            foreach (var piece in shatterPieces)
            {
                GameObject g = Instantiate(piece, transform.position, Quaternion.identity);
                MassSimulatedObject mass = g.GetComponent<MassSimulatedObject>();
                g.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
                mass.velocity += new Vector2(Random.insideUnitSphere.x, Random.insideUnitSphere.y).normalized * (Mathf.Log(force + 1)  * forceMultiplier * _gameManager.planetDamageMultiplier);
                mass.mass = m / shatterPieces.Count;
            }
        }
    }
}
