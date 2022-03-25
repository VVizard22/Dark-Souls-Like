using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform _cameraObject;
        InputHandler _inputHandler;
        Vector3 _moveDirection;

        public Transform _myTransform { get; private set; }
        public AnimatorHandler _animatorHandler { get; private set; }

        public Rigidbody _rigidbody { get; private set; }
        public GameObject _normalCamera { get; private set; }

        [Header("Stats")]
        [SerializeField]
        float _movementSpeed = 5;
        [SerializeField]
        float _rotationSpeed = 10;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<InputHandler>();
            _cameraObject = Camera.main.transform;
            _myTransform = transform;
            _animatorHandler = GetComponentInChildren<AnimatorHandler>();

            _animatorHandler.Initialize();
        
        }

        public void Update()
        {
            float delta = Time.deltaTime;

            _inputHandler.TickInput(delta);
            HandleMovement(delta);
            HandleRollingAndSprinting(delta);
            HandleDance(delta);
            
        }

        #region Movement

        Vector3 _normalVector;
        Vector3 _targetPosition;

        public void HandleMovement(float delta)
        {
            _moveDirection = _cameraObject.forward * _inputHandler._vertical;
            _moveDirection += _cameraObject.right * _inputHandler._horizontal;
            _moveDirection.Normalize();
            _moveDirection.y = 0;

            float speed = _movementSpeed;
            _moveDirection *= speed;

            Vector3 _projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
            _rigidbody.velocity = _projectedVelocity;

            _animatorHandler.UpdateAnimatorValues(_inputHandler._moveAmount, 0);

            if (_animatorHandler._canRotate)
                HandleRotation(delta);
        }

        private void HandleRotation(float delta)
        {
            Vector3 _targetDir = Vector3.zero;
            float _moveOverride = _inputHandler._moveAmount;

            _targetDir = _cameraObject.forward * _inputHandler._vertical;
            _targetDir += _cameraObject.right * _inputHandler._horizontal;

            _targetDir.Normalize();
            _targetDir.y = 0;

            if (_targetDir == Vector3.zero)
                _targetDir = _myTransform.forward;

            float rs = _rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(_targetDir);
            Quaternion _targetRotation = Quaternion.Slerp(_myTransform.rotation, tr, rs * delta);

            _myTransform.rotation = _targetRotation;
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (_animatorHandler._anim.GetBool("isInteracting"))
                return;
            
            if (_inputHandler._rollFlag)
            {
                _moveDirection = _cameraObject.forward * _inputHandler._vertical;
                _moveDirection += _cameraObject.right * _inputHandler._horizontal;

                if (_inputHandler._moveAmount > 0)
                {
                    _animatorHandler.PlayTargetAnimation("Rolling", true);
                    _moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(_moveDirection);
                    _myTransform.rotation = rollRotation;
                }
                else
                {
                    _animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

        public void HandleDance(float delta)
        {
            if (_animatorHandler._anim.GetBool("isInteracting"))
                return;

            if (_inputHandler._danceFlag)
            {
                _animatorHandler.PlayTargetAnimation("Dance", true);
                _moveDirection.y = 0;
            }
        }

        #endregion
    }
}
