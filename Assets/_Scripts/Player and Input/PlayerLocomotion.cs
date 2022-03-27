using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager _playerManager;
        Transform _cameraObject;
        Vector3 _moveDirection;

        public Transform _myTransform { get; private set; }
        public AnimatorHandler _animatorHandler { get; private set; }

        public Rigidbody _rigidbody { get; private set; }
        public GameObject _normalCamera { get; private set; }

        [Header("Stats")]
        [SerializeField]
        float _movementSpeed = 5;
        [SerializeField]
        float _sprintSpeed = 7;
        [SerializeField]
        float _rotationSpeed = 10;

        float _moveAmount = 0;

        void Awake()
        {
            _playerManager = GetComponent<PlayerManager>();
            _rigidbody = GetComponent<Rigidbody>();
            _cameraObject = Camera.main.transform;
            _myTransform = transform;
            _animatorHandler = GetComponentInChildren<AnimatorHandler>();

            _animatorHandler.Initialize();
        
        }

        #region Movement

        Vector3 _normalVector;
        Vector3 _targetPosition;

        public void HandleMovement(float delta, float horMove, float verMove)
        {
            if (_playerManager._disableMovement.Contains(_playerManager.ActionState))
                return;

            _moveDirection = _cameraObject.forward * verMove;
            _moveDirection += _cameraObject.right * horMove;
            _moveDirection.Normalize();
            _moveDirection.y = 0;

            float speed = _movementSpeed;
            bool isSprinting = _playerManager.MovementState == MovementFlag.Sprint;

            if (isSprinting)
                speed = _sprintSpeed;

            _moveDirection *= speed;

            _moveAmount = Mathf.Clamp01(Mathf.Abs(horMove) + Mathf.Abs(verMove));

            Vector3 _projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
            _rigidbody.velocity = _projectedVelocity;

            _animatorHandler.UpdateAnimatorValues(_moveAmount, 0, isSprinting);

            if (_animatorHandler._canRotate)
                HandleRotation(delta, horMove, verMove);
        }

        private void HandleRotation(float delta, float horMove, float verMove)
        {
            if (_playerManager._disableRotation.Contains(_playerManager.ActionState))
                return;

            Vector3 _targetDir = Vector3.zero;
            float _moveOverride = _moveAmount;

            _targetDir = _cameraObject.forward * verMove;
            _targetDir += _cameraObject.right * horMove;

            _targetDir.Normalize();
            _targetDir.y = 0;

            if (_targetDir == Vector3.zero)
                _targetDir = _myTransform.forward;

            float rs = _rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(_targetDir);
            Quaternion _targetRotation = Quaternion.Slerp(_myTransform.rotation, tr, rs * delta);

            _myTransform.rotation = _targetRotation;
        }

        public void HandleRollingAndSprinting(float delta, float horMove, float verMove)
        {
            if (_animatorHandler._anim.GetBool("isInteracting") || _playerManager.ActionState == ActionFlag.None)
                return;


            _moveDirection = _cameraObject.forward * verMove;
            _moveDirection += _cameraObject.right * horMove;

            if (_playerManager.ActionState == ActionFlag.Roll)
            {
                _animatorHandler.PlayTargetAnimation("Rolling", true);
                _moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(_moveDirection);
                _myTransform.rotation = rollRotation;
            }

            if (_playerManager.ActionState == ActionFlag.Backstep)
                _animatorHandler.PlayTargetAnimation("Backstep", true);
        }
        #endregion
    }
}
