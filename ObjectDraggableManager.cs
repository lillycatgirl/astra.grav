using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectDraggableManager : MonoBehaviour
{
    public enum DraggableType
    {
        Planet,
        PlanetVectorArrow,
        ShopItem,
        Interactable,
        InventoryItem
    }

    private readonly DraggableType[] _setupDraggables = {DraggableType.Planet, DraggableType.PlanetVectorArrow, DraggableType.InventoryItem };
    private readonly DraggableType[] _shopDraggables = {DraggableType.Interactable, DraggableType.ShopItem };
    
    
    private List<Draggable> _draggableObjects = new List<Draggable>();
    private Dictionary<Draggable, Vector3> _dragOffsets = new Dictionary<Draggable, Vector3>();
    public bool enableDragging;
    
    private Draggable _selectedObject;
    private bool _isObjectSelected = false;
    
    public static ObjectDraggableManager Instance;
    
    private static GameManager _gameManager;
    private static ObjectGravityManager _objectGravityManager;
    private Camera _cam;
    
    private void Awake()
    {
        _cam = Camera.main;
        _gameManager = GameManager.Instance;
        _objectGravityManager = ObjectGravityManager.Instance;
        if (Instance == null)
        {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    public void AddDraggableObject(Draggable draggable)
    {
        _draggableObjects.Add(draggable);
    }
    
    public void RemoveDraggableObject(Draggable draggable)
    {
        _draggableObjects.Remove(draggable);
    }
    
    private void MouseUpDuringSetup()
    {
        _dragOffsets[_selectedObject] = _cam.ScreenToWorldPoint(Input.mousePosition);
        _dragOffsets[_selectedObject] = new Vector3(_dragOffsets[_selectedObject].x, _dragOffsets[_selectedObject].y, 0);
        _selectedObject = null;
        _isObjectSelected = false;
    }

    private bool ValidDraggableObject(Draggable draggable) 
        // If the draggable is the right type
    {
        switch (_gameManager.gameState)
        {
            case GameManager.GameState.PostRound:
                return false;
            case GameManager.GameState.Shop:
                return draggable.draggableType.Any(draggableType => _shopDraggables.Contains(draggableType));
            case GameManager.GameState.Setup:
                return draggable.draggableType.Any(draggableType => _setupDraggables.Contains(draggableType));
            case GameManager.GameState.Round:
            default:
                return false;
        }
    }
    
    
    private void MouseDownDuringSetup()
    {
        Vector3 pos = _cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        foreach (var drag in _draggableObjects)
        {
            if(!ValidDraggableObject(drag)) continue;
            if ((pos - drag.transform.position).magnitude > drag.radius)
            {
                continue;
            }

            _selectedObject = drag;
            _isObjectSelected = true;
            if (!_dragOffsets.TryAdd(drag, pos))
            {
                _dragOffsets[drag] = pos;
            }
            break;
        }
    }

    private void Update()
    {
        if(enableDragging) HandleDraggableMovement();
    }

    private void HandleDraggableMovement()
    {
        _objectGravityManager ??= ObjectGravityManager.Instance;
        foreach (var pair in _dragOffsets)
        {
            pair.Key.transform.position = Vector3.Lerp(
                pair.Key.transform.position,
                pair.Value,
                1f - Mathf.Exp(-pair.Key.easing * Time.deltaTime)
            );
        }
            
        if (Input.GetMouseButton(0))
        {
            if (_isObjectSelected)
            {
                _dragOffsets[_selectedObject] = _cam.ScreenToWorldPoint(Input.mousePosition);
                _dragOffsets[_selectedObject] = new Vector3(_dragOffsets[_selectedObject].x, _dragOffsets[_selectedObject].y, 0);
            }
            else
            {
                MouseDownDuringSetup();
            }
        }

        if (!Input.GetMouseButton(0) && _isObjectSelected)
        {
            MouseUpDuringSetup();
        }
            
        foreach (var mass in _objectGravityManager.MassSimulatedObjects)
        {
            if (mass is not PlanetObject p) continue;
            p.velocity = p.planetArrowController.arrowVector;
        } 
    }
}
