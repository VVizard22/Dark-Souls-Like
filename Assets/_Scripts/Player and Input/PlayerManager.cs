using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class PlayerManager : MonoBehaviour
    {
        CameraHandler _cameraHandler;
        InputHandler _inputHandler;
        PlayerLocomotion _playerLocomotion;

        public MovementFlag MovementState;
        public ActionFlag ActionState;

        public List<ActionFlag> _disableMovement = new List<ActionFlag>() { ActionFlag.Backstep };
        public List<ActionFlag> _disableRotation = new List<ActionFlag>() { ActionFlag.Backstep };

        #region Subscribe to events
        void OnEnable()
        {
            InputHandler.MovementFlagRaised += FlagHandle;
            InputHandler.ActionFlagRaised += FlagHandle;
            ResetIsInteracting.finishedAnimation += FlagHandle;
        }

        void OnDisable()
        {
            InputHandler.MovementFlagRaised -= FlagHandle;
            InputHandler.ActionFlagRaised -= FlagHandle;
            ResetIsInteracting.finishedAnimation -= FlagHandle;
        }

        #endregion

        void Awake()
        {
            _cameraHandler = CameraHandler._singleton;
            _inputHandler = GetComponent<InputHandler>();
            _playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (_cameraHandler != null)
            {
                _cameraHandler.FollowTarget(delta);
                _cameraHandler.HandleCameraRotation(delta, _inputHandler._mouseX, _inputHandler._mouseY);
            }
        }

        void Update()
        {
            float delta = Time.deltaTime;

            #region PlayerLocomotion Methods
            _inputHandler.TickInput(delta);
            _playerLocomotion.HandleMovement(delta, _inputHandler._horizontal, _inputHandler._vertical);
            _playerLocomotion.HandleRollingAndSprinting(_inputHandler._horizontal, _inputHandler._vertical);
            #endregion


        }

        public void FlagHandle(MovementFlag f)
        {
            if (_disableMovement.Contains(ActionState))
                return;
            
            MovementState = f;
        }
        public void FlagHandle(ActionFlag f)
        {
            if (f == ActionFlag.None)
            {
                ActionState = f;
                return;
            }
            else if (ActionState != ActionFlag.None)
                return;
            ActionState = f;
        }
    }
}
