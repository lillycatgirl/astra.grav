using UnityEngine;

namespace Gameplay.Scoring
{
    public class OrbitScoringTrigger : ScoringTrigger
    {
        public override void OnOrbit()
        {
            AddScore();
        }

        public override void OnCollideGhost()
        {
            
        }

        public override void OnCollidePlanet()
        {
            
        }

        public override void OnCollideWall()
        {
            
        }

        public override void OnPlanetDestroyedThis()
        {
            
        }

        public override void OnThisPlanetDestroy()
        {
            
        }
    }
}
