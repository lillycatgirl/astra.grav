using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectGravityManager : MonoBehaviour
{
    [SerializeField] private float gravityConstant;
    [SerializeField] public float bounceCollisionMultiplier;
    [SerializeField] public float damageMultiplier;
    [SerializeField] public int physicsSteps;
    [SerializeField] public float timeScale;
    public bool simulateGravity;
    
    
    public List<MassSimulatedObject> MassSimulatedObjects { get; private set; } = new List<MassSimulatedObject>();
    public List<MassSimulatedObject> DisabledMassSimulatedObjects { get; private set; } = new List<MassSimulatedObject>();
    private Dictionary<PlanetObject, Vector3> _originalPlanetPositions = new();
    public static ObjectGravityManager Instance;
    
    
    private GameManager _gameManager;
    private ObjectDraggableManager _objectDraggableManager;
    private Camera _cam;
    
    private void Awake()
    {
        _cam = Camera.main;
        _gameManager = GameManager.Instance;
        _objectDraggableManager = ObjectDraggableManager.Instance;
        if (Instance == null)
        {
            Instance = this;
            Application.targetFrameRate = 60;
        } else {
            Destroy(gameObject);
        }
    }
    private void RunSimulation()
    {
        for (var i = 0; i < physicsSteps; i++)
        {
            // TODO move this to game manager :3
            foreach (var mass in MassSimulatedObjects)
            {
                mass.force = Vector2.zero;
                if (mass is not PlanetObject p) continue;
            } 
            
            UpdateObjectVObjectCollisions();
            UpdateObjectVWallCollisions();
            
            UpdateObjectForces();
            foreach (var mass in MassSimulatedObjects)
            {
                mass.RecalculateAndApplyVelocity(Time.fixedDeltaTime * timeScale / physicsSteps);
            }
        }
        foreach (var massObject in MassSimulatedObjects)
        {
            massObject.DetectRotationDirection();
        }
    }

    public void OnRoundStart()
    {
        foreach (var mass in MassSimulatedObjects.ToList())
        {
            mass.OnRoundStart();
            
            
            if (mass is not PlanetObject planetObject) continue;
            if (!_originalPlanetPositions.TryAdd(planetObject, planetObject.transform.position))
            {
                _originalPlanetPositions[planetObject] = planetObject.transform.position;
            }
        }
    }

    public void OnRoundEnd()
    {
        foreach (var mass in DisabledMassSimulatedObjects.ToList())
        {
            ((PlanetObject)mass).RecreateThis();
        }

        foreach (var mass in MassSimulatedObjects)
        {
            if (mass is GhostMassSimulatedObject g)
            {
                g.Destroy();
            }
        }
        
        foreach (var originalPlanetPosition in _originalPlanetPositions)
        {
            if (originalPlanetPosition.Key == null)
            {
                UnityEngine.Debug.LogWarning("Planet could not be restored to previous position as reference is null");
                continue;
            }
            originalPlanetPosition.Key.transform.position = originalPlanetPosition.Value;
            originalPlanetPosition.Key.ResetAllValues();
        }
    }
    
    private void FixedUpdate()
    {
        if(simulateGravity) RunSimulation();
    }

    public void AddMassSimulatedObject(MassSimulatedObject massSimulatedObject)
    {
        MassSimulatedObjects.Add(massSimulatedObject);
        foreach (var massObject in MassSimulatedObjects)
        {
            massObject.UpdateTrackedMassOrbits();
        }
    }
    
    public void RemoveMassSimulatedObject(MassSimulatedObject massSimulatedObject)
    {
        MassSimulatedObjects.Remove(massSimulatedObject);
    }

    public void AddDisabledMassSimulatedObject(MassSimulatedObject massSimulatedObject)
    {
        DisabledMassSimulatedObjects.Add(massSimulatedObject);
    }
    
    public void RemoveDisabledMassSimulatedObject(MassSimulatedObject massSimulatedObject)
    {
        DisabledMassSimulatedObjects.Remove(massSimulatedObject);
    }
    
    private void UpdateObjectForces()
    {
        for (var i = 0; i < MassSimulatedObjects.Count; i++)
        {
            for (var j = i + 1; j < MassSimulatedObjects.Count; j++)
            {
                var mass1 = MassSimulatedObjects[i];
                var mass2 = MassSimulatedObjects[j];

                CalculateForceFromObjects(mass1, mass2, ref mass1.force, ref mass2.force);
            }
        }
    }

    private void UpdateObjectVWallCollisions()
    {
        foreach (var mass in MassSimulatedObjects.ToList())
        {
            if (mass is not PlanetObject m) continue;
            if (m.transform.position.y + m.radius > _gameManager.planetPositionLimit.y || m.transform.position.y - m.radius < -_gameManager.planetPositionLimit.y)
            {
                PlanetCollisionSoundPlayer.Instance.PlaySoundFromPlanetTypes(m.planetSfx);
                
                m.transform.position = new Vector3(m.transform.position.x, m.transform.position.y + m.radius > _gameManager.planetPositionLimit.y ?  
                        _gameManager.planetPositionLimit.y - m.radius: 
                        -_gameManager.planetPositionLimit.y + m.radius, 
                    m.transform.position.z);
                m.velocity.y *= -_gameManager.wallRestitution;
                m.TakeDamage(_gameManager.wallDamage);
            }
            if (m.transform.position.x + m.radius> _gameManager.planetPositionLimit.x || m.transform.position.x -m.radius < -_gameManager.planetPositionLimit.x)
            {
                PlanetCollisionSoundPlayer.Instance.PlaySoundFromPlanetTypes(m.planetSfx);
                
                m.transform.position = new Vector3(m.transform.position.x + m.radius > _gameManager.planetPositionLimit.x ?  
                        _gameManager.planetPositionLimit.x - m.radius: 
                        -_gameManager.planetPositionLimit.x + m.radius,
                    m.transform.position.y, 
                    m.transform.position.z);
                m.velocity.x *= -_gameManager.wallRestitution;
                m.TakeDamage(_gameManager.wallDamage);
            }
        }
    }
    
    private void UpdateObjectVObjectCollisions()
    {
        for (var i = 0; i < MassSimulatedObjects.Count; i++)
        {
            for (var j = i + 1; j < MassSimulatedObjects.Count; j++)
            {
                var m1 = MassSimulatedObjects[i];
                var m2 = MassSimulatedObjects[j];

                Vector2 pos1 = m1.transform.position;
                Vector2 pos2 = m2.transform.position;

                var delta = pos2 - pos1;
                var distance = delta.magnitude;
                var minDistance = m1.radius + m2.radius;

                if (distance == 0f) continue;

                if (distance < minDistance)
                {
                    var normal = delta / distance;
                    var penetration = minDistance - distance;
                    var correction = normal * (penetration / 2f);

                    if (m2 is not GhostMassSimulatedObject || m1 is GhostMassSimulatedObject) m1.transform.position -= (Vector3)correction;
                    if (m1 is not GhostMassSimulatedObject || m2 is GhostMassSimulatedObject) m2.transform.position += (Vector3)correction;

                    Vector2 relativeVelocity = m2.velocity - m1.velocity;

                    float velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

                    if (velocityAlongNormal > 0)
                        continue;

                    float restitution = bounceCollisionMultiplier * (m1.bounciness + 1) * (m2.bounciness + 1);

                    float impulseScalar =
                        -(1 + restitution) * velocityAlongNormal /
                        (1 / m1.mass + 1 / m2.mass);

                    Vector2 impulse = impulseScalar * normal;
                    
                    if (m2 is not GhostMassSimulatedObject || m1 is GhostMassSimulatedObject) m1.velocity -= impulse / m1.mass;
                    if (m1 is not GhostMassSimulatedObject || m2 is GhostMassSimulatedObject) m2.velocity += impulse / m2.mass;
                    
                    if (m1 is GhostMassSimulatedObject || m2 is GhostMassSimulatedObject) continue;
                    
                    float reducedMass = (m1.mass * m2.mass) / (m1.mass + m2.mass);
                    float impactSpeed = Mathf.Abs(velocityAlongNormal);
                    float collisionEnergy = reducedMass * impactSpeed * impactSpeed * damageMultiplier;
                    
                    m1.OnCollideWithObject(m2, collisionEnergy * (m2.mass / (m2.mass + m1.mass)));
                    m2.OnCollideWithObject(m1, collisionEnergy * (m1.mass / (m2.mass + m1.mass)));
                }
            }
        }
    }
    private void CalculateForceFromObjects(MassSimulatedObject m1, MassSimulatedObject m2, ref Vector2 f1, ref Vector2 f2)
    {
        var direction = m2.transform.position - m1.transform.position;
        var distanceSqr = direction.sqrMagnitude;
        if (distanceSqr == 0f || distanceSqr < (m1.radius + m2.radius) * (m1.radius + m2.radius))
        {
            f1 += Vector2.zero;
            f2 += Vector2.zero;
            return;
        }

        float forceMagnitude = gravityConstant * (m1.mass * m2.mass) / distanceSqr;

        var force = direction.normalized * forceMagnitude;

        // Equal and opposite forces
        var m1Force = force;
        var m2Force = -force;

        if (m1 is not GhostMassSimulatedObject)
        {
            f2 += (Vector2)m2Force;
        }
        if (m2 is not GhostMassSimulatedObject)
        {
            f1 += (Vector2)m1Force;
        }
    }

}
