using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class TooltipManager : MonoBehaviour
    {
        private Camera _camera;
        public GameObject tooltip;
        public TMP_Text tooltipTitle;
        public TMP_Text tooltipSubTitle;
        public TMP_Text tooltipBody;
        private RectTransform _tooltipRect;
        public Canvas canvas;
        private GameObject _prevHit;
        private float _stickyTimer;
        public float stickyTimerMax;

        private void Awake()
        {
            _camera = Camera.main;
            _tooltipRect = tooltip.GetComponent<RectTransform>();
        }

        void Update()
        {
            Vector2 mouseScreenPos = Input.mousePosition;

            Vector2 mouseWorldPos = _camera.ScreenToWorldPoint(mouseScreenPos);

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
            
            if (hit != null && hit.TryGetComponent<Tooltip>(out var tt))
            {
                _stickyTimer = 0f;
                if (hit.gameObject != _prevHit)
                {
                    _prevHit = hit.gameObject;

                    tooltip.SetActive(true);
                    tooltipTitle.text = tt.title;
                    tooltipSubTitle.text = tt.subtitle;
                    tooltipBody.text = tt.message;
                }
            }
            else if (_stickyTimer >= stickyTimerMax)
            {
                tooltip.SetActive(false);
                _prevHit = null;
            }

            if (_prevHit != null && _stickyTimer <= stickyTimerMax)
            {
                
                Vector2 screenPos = _camera.WorldToScreenPoint(_prevHit.transform.position);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPos,
                    canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                    out Vector2 uiPos
                );

                _tooltipRect.localPosition = uiPos;
                _stickyTimer +=  Time.deltaTime;
            }
        }
        
    }
}