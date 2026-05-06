using System.Collections.Generic;
using Audio;
using FX;
using Gameplay.Scoring;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class PlanetObject : MassSimulatedObject
{
    public List<ScoringTrigger> scoringTriggers = new();
    
    [Header("Planet-Specific Settings")]
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float baseRotationSpeed;
    [SerializeField] protected float rotationSpeedRandomisation;
    [SerializeField] protected float planetHealth;
    [SerializeField] protected float planetHealthMax;
    
    private GameManager _gameManager = GameManager.Instance;
    protected PlanetDestroyFxManager DestroyFxManager;
    public GameObject planetSprite;
    public VectorArrowController planetArrowController;
    public HealthBar planetHealthBar;

    protected void Awake()
    {
        ResetAllValues();
        DestroyFxManager = GetComponent<PlanetDestroyFxManager>();
    }

    public void ResetAllValues()
    {
        ResetRotationSpeed();
        ResetHealth();
    }

    public override void OnDestroy()
    {
        _gameManager.RemovePlayerPlanet(this);
        base.OnDestroy();
    }
    protected void Update()
    {
        planetSprite.transform.rotation *= Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        damage = Mathf.Max(2, damage);
        planetHealth -= damage;
        planetHealthBar.health = planetHealth;
        planetHealthBar.maxHealth = planetHealthMax;
        planetHealthBar.UpdateHealthDisplay(planetHealth, planetHealthMax);
        print($"{gameObject.name} took  {damage} damage and is now on {planetHealth} health");
        if (planetHealth <= 0)
        {
            DestroyThis();
            DestroyFxManager.Explode(damage, mass);
        }
    }

    protected void DestroyThis()
    {
        GravityManager.AddDisabledMassSimulatedObject(this);
        GravityManager.RemoveMassSimulatedObject(this);
        planetHealthBar.gameObject.SetActive(false);
        planetSprite.SetActive(false);
    }

    public void RecreateThis()
    {
        planetHealthBar.gameObject.SetActive(true);
        GravityManager.AddMassSimulatedObject(this);
        GravityManager.RemoveDisabledMassSimulatedObject(this);
        planetSprite.SetActive(true);
        AddAllScoringTriggers();
    }

    protected override void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.AddPlayerPlanet(this);
        AddAllScoringTriggers();
        base.Start();
    }

    private void AddAllScoringTriggers()
    {
        scoringTriggers.Clear();
        scoringTriggers.AddRange(GetComponents<ScoringTrigger>());
    }

    public override void OnCollideWithObject(MassSimulatedObject massObject, float f)
    {
        rotationSpeed += Random.Range(-rotationSpeedRandomisation, rotationSpeedRandomisation);
        TakeDamage(f);
        if (massObject is PlanetObject)
        {
            PlanetCollisionSoundPlayer.Instance.PlaySoundFromPlanetTypes(this.planetSfx, massObject.planetSfx);
        }
        else
        {
            PlanetCollisionSoundPlayer.Instance.PlaySoundFromPlanetTypes(this.planetSfx);
        }
        base.OnCollideWithObject(massObject, f);
    }

    protected void ResetRotationSpeed()
    {
        rotationSpeed = baseRotationSpeed +  Random.Range(-rotationSpeedRandomisation, rotationSpeedRandomisation);
    }

    protected void ResetHealth()
    {
        planetHealth = planetHealthMax;
        planetHealthBar.UpdateHealthDisplay(planetHealth, planetHealthMax);
    }

    public override void AfterMovementApplied()
    {
        
    }

    public override void BeforeMovementApplied()
    {
        
    }

    public override void OnClicked()
    {
        
    }

    protected override void OnOrbited(MassSimulatedObject m)
    {
        foreach (var scoringTrigger in scoringTriggers)
        {
            scoringTrigger.OnOrbit();
        }
        base.OnOrbited(m);
    }
}