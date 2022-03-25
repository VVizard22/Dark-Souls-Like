using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform _targetTransform;
        public Transform _cameraTransform;
        public Transform _cameraPivotTransform;
        private Transform _myTransform;
        private Vector3 _cameraTransformPosition;
        private LayerMask _ignoreLayers;
        private Vector3 _cameraFollowVelocity = Vector3.zero;

        public static CameraHandler _singleton;

        public float _lookSpeed = 0.1f;
        public float _followSpeed = 0.1f;
        public float _pivotSpeed = 0.03f;

        private float _targetPosition;
        private float _defaultPosition;
        private float _lookAngle;
        private float _pivotAngle;
        public float _minimumPivot = -35;
        public float _maximumPivot = 35;

        public float _cameraSphereRadius = 0.2f;
        public float _cameraCollisionOffset = 0.2f;
        public float _minimumCollisionOffset = 0.2f;

        private void Awake()
        {
            _singleton = this;
            _myTransform = transform;
            _defaultPosition = _cameraTransform.localPosition.z;
            _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        public void FollowTarget(float delta)
        {
            Vector3 targetPosition = Vector3.SmoothDamp
                (_myTransform.position, _targetTransform.position, ref _cameraFollowVelocity, delta / _followSpeed);
            _myTransform.position = targetPosition;

            HandleCameraCollision(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            _lookAngle += (mouseXInput * _lookSpeed) / delta;
            _pivotAngle -= (mouseYInput * _pivotSpeed) / delta;

            _pivotAngle = Mathf.Clamp(_pivotAngle, _minimumPivot, _maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = _lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            _myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = _pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            _cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCameraCollision(float delta)
        {
            _targetPosition = _defaultPosition;
            RaycastHit hit;
            Vector3 direction = _cameraTransform.position - _cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast
                (_cameraPivotTransform.position, _cameraSphereRadius, direction, out hit
                , Mathf.Abs(_targetPosition), _ignoreLayers))
            {
                float dis = Vector3.Distance(_cameraPivotTransform.position, hit.point);
                _targetPosition = -(dis - _cameraCollisionOffset);
            }

            if (Mathf.Abs(_targetPosition) < _minimumCollisionOffset)
            {
                _targetPosition = -_minimumCollisionOffset;
            }

            _cameraTransformPosition.z = Mathf.Lerp(_cameraTransform.localPosition.z, _targetPosition, delta / 0.2f);
            _cameraTransform.localPosition = _cameraTransformPosition;
        }
    }
}
