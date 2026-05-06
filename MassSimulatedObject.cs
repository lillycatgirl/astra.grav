using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MassSimulatedObject : MonoBehaviour
{
    public enum PlanetSoundFX
    {
        Default,
        Metal,
        Glass
    }
    public enum PlanetTypes
    {
        Sun,
        Moon,
        Planet,
        Comet,
        Enemy,
        Debris
    }
    public enum Direction
    {
        None,
        Clockwise,
        CounterClockwise
    }
    
    public class RotationDirection
    {
        public float PreviousAngle;
        public Direction Direction;
        public float AccumulatedAngle;
        public bool Initialized = false;
    }
    
    [Header("Mass Simulated Object Settings")]
    [SerializeField] public float mass;
    [SerializeField] public Vector2 velocity;
    [HideInInspector] public Vector2 acceleration;
    [HideInInspector] public Vector2 force;
    [SerializeField] public PlanetSoundFX planetSfx;
    [SerializeField] public PlanetTypes[] planetTypes;
    [SerializeField] public float radius;
    [SerializeField] public float bounciness;
    [SerializeField] public bool isDestroyed = false;   
    [SerializeField] public Dictionary<MassSimulatedObject, RotationDirection> TrackedMassOrbitRotations = new Dictionary<MassSimulatedObject, RotationDirection>();
    
    protected ObjectGravityManager GravityManager;
    protected virtual void Start()
    {
        GravityManager = ObjectGravityManager.Instance;
        GravityManager.AddMassSimulatedObject(this);
    }

    public virtual void OnDestroy()
    {
        GravityManager.RemoveMassSimulatedObject(this);
        UpdateTrackedMassOrbits();
    }

    public virtual void RecalculateAndApplyVelocity(float delta)
    {
        BeforeMovementApplied();
        UpdateAccelerationAndVelocityFromForce(force, delta);
        ApplyVelocity(delta);
    }

    protected virtual void UpdateAccelerationAndVelocityFromForce(Vector2 appliedForce, float delta)
    {
        acceleration = appliedForce / mass;
        velocity += acceleration * (delta);
        
    }

    protected virtual void ApplyVelocity(float delta)
    {
        transform.position += new Vector3(velocity.x, velocity.y) * (delta);
        AfterMovementApplied();
    }
    
    public virtual void OnCollideWithObject(MassSimulatedObject massObject, float force)
    {
        return;
    }

    public abstract void AfterMovementApplied();
    
    public abstract void BeforeMovementApplied();
    
    public abstract void OnClicked();

    public virtual void DetectRotationDirection()
    {
        foreach (var key in TrackedMassOrbitRotations.Keys.ToList())
        {
            var trackedMass = TrackedMassOrbitRotations[key];
            
            // Calc if going clockwise or anti
            Vector2 dir = (transform.position - key.transform.position);
            var cross = dir.x * velocity.y - dir.y * velocity.x;
            Direction direction =
                cross > 0 ? Direction.CounterClockwise :
                cross < 0 ? Direction.Clockwise :
                Direction.None;
            
            // Current direction
            float currentDir = Mathf.Atan2(dir.y, dir.x)* Mathf.Rad2Deg;

            if (!trackedMass.Initialized)
            {
                trackedMass.PreviousAngle = currentDir;
                trackedMass.Direction = direction;
                trackedMass.AccumulatedAngle = 0f;
                trackedMass.Initialized = true;
            }
            
            //reset if it changed direction
            if (direction != trackedMass.Direction)
            {
                trackedMass.AccumulatedAngle = 0f;
                trackedMass.Direction = direction;
            }
            
            
            float delta = Mathf.DeltaAngle(trackedMass.PreviousAngle, currentDir);

            trackedMass.AccumulatedAngle += delta;
            trackedMass.PreviousAngle = currentDir;
            

            if (Mathf.Abs(trackedMass.AccumulatedAngle)  >= 360)
            {
                OnOrbited(key);
                trackedMass.AccumulatedAngle = 0f;
            }
        }
    }

    public virtual void OnRoundStart()
    {
        UpdateTrackedMassOrbits();
        DeInitializeTrackedMassOrbits();
    }

    private void DeInitializeTrackedMassOrbits()
    {
        foreach (var key in TrackedMassOrbitRotations.Keys.ToList())
        {
            var rotationDirection = TrackedMassOrbitRotations[key];
            rotationDirection.Initialized = false;
        }
    }
    
    public virtual void UpdateTrackedMassOrbits()
    {
        // If this is a sun, run the code to track orbits as sun
        if (this.planetTypes.Contains(PlanetTypes.Sun)) SunUpdateTrackedMassOrbits();
        // If this is a planet, run the code to track orbits as planet
        if (this.planetTypes.Contains(PlanetTypes.Planet)) PlanetUpdateTrackedMassOrbits();
    }
    public void ResetTrackedMassOrbits()
    {
        TrackedMassOrbitRotations.Clear();
    }
    public virtual void PlanetUpdateTrackedMassOrbits()
    {
        ResetTrackedMassOrbits();
        // Only track orbit around Suns
        foreach (var m in GravityManager.MassSimulatedObjects)
        {
            if (m.planetTypes.Contains(PlanetTypes.Sun))
            {
                TrackedMassOrbitRotations.TryAdd(m, new RotationDirection());
            }
        }
    }
    public virtual void SunUpdateTrackedMassOrbits()
    {
        
    }

    protected virtual void OnOrbited(MassSimulatedObject m)
    {
        
    }
    
}
