using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Audio
{
    
    public class PlanetCollisionSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource collisionDefault;
        [SerializeField] private AudioSource collisionMetal;
        [SerializeField] private AudioSource collisionGlass;

        public static PlanetCollisionSoundPlayer Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public void PlaySoundFromPlanetTypes(MassSimulatedObject.PlanetSoundFX p1, MassSimulatedObject.PlanetSoundFX p2)
        {
            if (p1 == MassSimulatedObject.PlanetSoundFX.Default && p2 == MassSimulatedObject.PlanetSoundFX.Default)
            {
                collisionDefault.Play();
                
            }
        }
        public void PlaySoundFromPlanetTypes(MassSimulatedObject.PlanetSoundFX p1, MassSimulatedObject.PlanetSoundFX p2, float volume)
        {
            if (p1 == MassSimulatedObject.PlanetSoundFX.Default && p2 == MassSimulatedObject.PlanetSoundFX.Default)
            {
                collisionDefault.volume = volume;
                collisionDefault.Play();
            }
        }
        
        public void PlaySoundFromPlanetTypes(MassSimulatedObject.PlanetSoundFX p1)
        {
            if (p1 == MassSimulatedObject.PlanetSoundFX.Default)
            {
                collisionDefault.Play();
            }
        }
    }

}
