using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private float _animationTimer;
        private Image _image;
        private bool _isHovering;
        private GameManager _gameManager;
        [SerializeField]private Vector2 speedMults;

        [SerializeField] private Sprite[] startSprites;
        [SerializeField] private Sprite[] endSprites;
        

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            if (_gameManager.gameState == GameManager.GameState.Shop) return;
            _animationTimer += Time.deltaTime * (_isHovering ? speedMults.x : speedMults.y);
            _image.sprite = _gameManager.gameState == GameManager.GameState.Setup ?  startSprites[(int)_animationTimer % startSprites.Length] :  endSprites[(int)_animationTimer % endSprites.Length];
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
        }
    
    }
}
