using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class InputHandler : MonoBehaviour
    {
        public delegate void RaiseMovementFlag(MovementFlag flag);
        public static event RaiseMovementFlag MovementFlagRaised;

        public delegate void RaiseActionFlag(ActionFlag flag);
        public static event RaiseActionFlag ActionFlagRaised;

        public float _horizontal { get; private set; }
        public float _vertical { get; private set; }
        public float _moveAmount { get; private set; }
        public float _mouseX { get; private set; }
        public float _mouseY { get; private set; }

        public bool _rollInput { get; private set; }
        public bool _danceInput { get; private set; }

        float _rollInputTimer = 0.0f;

        PlayerControls _inputActions;

        Vector2 _movementInput;
        Vector2 _cameraInput;

        #region Input System Setup
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
        #endregion

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
            HandleDanceInput();
        }

        private void MoveInput(float delta)
        {
            _horizontal = _movementInput.x;
            _vertical = _movementInput.y;
            _moveAmount = Mathf.Clamp01(Mathf.Abs(_horizontal) + Mathf.Abs(_vertical));

            _mouseX = _cameraInput.x;
            _mouseY = _cameraInput.y;

            if(_moveAmount != 0)
            {
                InvokeFlag(MovementFlag.Move);
            }
            else
            {
                InvokeFlag(MovementFlag.Idle);
            }
        }

        private void HandleRollInput(float delta)
        {
            _rollInput = _inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

            if (_rollInput)
            {
                _rollInputTimer += delta;
            }
            else
            {
                if (_rollInputTimer > 0 && _rollInputTimer < 0.5f)
                {
                    if (IsMoving())
                    {
                        InvokeFlag(ActionFlag.Roll);
                    }
                    else
                    {
                        InvokeFlag(ActionFlag.Backstep);
                    }
                }

                _rollInputTimer = 0;
            }

            if (_rollInputTimer >= 0.5f)
            {
                InvokeFlag(MovementFlag.Sprint);
            }
        }

        private void HandleDanceInput()
        {
            _danceInput = _inputActions.PlayerActions.Dance.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

            if (_danceInput)
                InvokeFlag(ActionFlag.Dance);
        }

        private bool IsMoving()
        {
            return _moveAmount != 0; 
        }

        public void InvokeFlag(MovementFlag f) => MovementFlagRaised?.Invoke(f);
        public void InvokeFlag(ActionFlag f) => ActionFlagRaised?.Invoke(f);
    }

    public enum MovementFlag
    {
        Idle,
        Move,
        Sprint
    }

    public enum ActionFlag
    {
        None,
        Roll,
        Backstep,
        Dance
    }
}
