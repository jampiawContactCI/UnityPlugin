using UnityEngine;

namespace Leap.Unity.Preview.HandRays
{
    public class HandRayRenderer : MonoBehaviour
    {
        public HandRayInteractor handRayInteractor;
        public LineRenderer lineRenderer;
        public bool hideRayOnNoPoints = false;

        private bool _isRayEnabled = true;
        private bool _isActive = true;
        private bool _isValid = true;

        public bool overrideRayInteractor = false;

        [SerializeField]
        public Gradient validColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        [SerializeField]
        public Gradient invalidColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.red, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };

        public virtual void SetActive(bool isActive = true)
        {
            _isActive = isActive;

            if (overrideRayInteractor)
            {
                lineRenderer.enabled = _isActive;
            }
            else
            {
                lineRenderer.enabled = _isActive && _isRayEnabled;
            }
        }

        private void OnRaycastStateChange(HandRayDirection direction, bool enabled)
        {
            _isRayEnabled = enabled;

            if (overrideRayInteractor)
            {
                lineRenderer.enabled = _isActive;
            }
            else
            {
                lineRenderer.enabled = _isActive && _isRayEnabled;
            }
        }

        public virtual void SetValid(bool isValid = true)
        {
            _isValid = isValid;
            lineRenderer.colorGradient = _isValid ? validColorGradient : invalidColorGradient;
        }

        protected virtual void OnEnable()
        {
            handRayInteractor.OnRaycastUpdate += OnRaycastUpdate;
            handRayInteractor.handRay.OnHandRayStateChange += OnRaycastStateChange;
        }

        protected virtual void OnDisable()
        {
            handRayInteractor.OnRaycastUpdate -= OnRaycastUpdate;
            handRayInteractor.handRay.OnHandRayStateChange -= OnRaycastStateChange;
        }

        protected virtual void OnRaycastUpdate(RaycastHit[] results, RaycastHit primaryHit)
        {
            if (overrideRayInteractor)
            {
                return;
            }

            if (!_isActive || !_isRayEnabled)
            {
                return;
            }

            if(hideRayOnNoPoints && (results == null || results.Length == 0 && hideRayOnNoPoints))
            {
                lineRenderer.positionCount = 0;
                return;
            }

            lineRenderer.positionCount = handRayInteractor.numPoints;
            lineRenderer.SetPositions(handRayInteractor.linePoints);
        }
    }
}