using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Scoring
{
    public abstract class ScoringTrigger : MonoBehaviour
    {
        [SerializeField] public long amount;
        [SerializeField] public long mult;
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        protected void AddScore()
        {
            _gameManager.AddScore(amount * mult);
        }

        protected void AddMult(long mult)
        {
            this.mult += mult;
        }
        protected void AddScoreAmount(long amount)
        {
            this.amount += amount;
        }
        
        public abstract void OnOrbit();
        public abstract void OnThisPlanetDestroy();
        public abstract void OnPlanetDestroyedThis();
        public abstract void OnCollideWall();
        public abstract void OnCollidePlanet();
        public abstract void OnCollideGhost();
    }
}