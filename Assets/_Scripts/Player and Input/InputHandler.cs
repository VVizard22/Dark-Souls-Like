using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class InputHandler : MonoBehaviour
    {
        public float _horizontal { get; private set; }
        public float _vertical { get; private set; }
        public float _moveAmount { get; private set; }
        public float _mouseX { get; private set; }
        public float _mouseY { get; private set; }

        PlayerControls _inputActions;
        CameraHandler _cameraHandler;

        Vector2 _movementInput;
        Vector2 _cameraInput;

        private void Awake()
        {
            _cameraHandler = CameraHandler._singleton;
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (_cameraHandler != null)
            {
                _cameraHandler.FollowTarget(delta);
                _cameraHandler.HandleCamaraRotation(delta, _mouseX, _mouseY);
            }
        }

        public void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new PlayerControls();
                _inputActions.PlayerMovement.Movement.performed += inputActions => _movementInput = inputActions.ReadValue<Vector2>();
                _inputActions.PlayerMovement.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();
            }

            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            _horizontal = _movementInput.x;
            _vertical = _movementInput.y;
            _moveAmount = Mathf.Clamp01(Mathf.Abs(_horizontal) + Mathf.Abs(_vertical));

            _mouseX = _cameraInput.x;
            _mouseY = _cameraInput.y;
        }
    }
}
