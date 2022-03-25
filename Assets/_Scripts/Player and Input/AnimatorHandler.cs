using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator _anim { get; private set; }
        public InputHandler _inputHandler { get; private set; }
        public PlayerLocomotion _playerLocomotion { get; private set; }

        int _vertical;
        int _horizontal;
        public bool _canRotate;


        public void Initialize()
        {
            _anim = GetComponent<Animator>();
            _inputHandler = GetComponentInParent<InputHandler>();
            _playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            _vertical = Animator.StringToHash("Vertical");
            _horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            #region Vertical
            float v = 0;

            if (verticalMovement > 0 && verticalMovement < 0.55f)
                v = 0.5f;
            else if (verticalMovement > 0.55f)
                v = 1;
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
                v = -0.5f;
            else if (verticalMovement < -0.55)
                v = 0;
            #endregion
            #region Horizontal
            float h = 0;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
                h = 0.5f;
            else if (horizontalMovement > 0.55f)
                h = 1;
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
                h = -0.5f;
            else if (horizontalMovement < -0.55)
                h = 0;
            #endregion
            _anim.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
            _anim.SetFloat(_horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            _anim.applyRootMotion = isInteracting;
            _anim.SetBool("isInteracting", isInteracting);
            _anim.CrossFade(targetAnim, 0.2f);
        }

        public void CanRotate() => _canRotate = true;

        public void StopRotation() => _canRotate = false;

        public void OnAnimatorMove()
        {
            if (_inputHandler._isInteracting == false)
                return;

            float delta = Time.deltaTime;
            _playerLocomotion._rigidbody.drag = 0;
            Vector3 deltaPosition = _anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            _playerLocomotion._rigidbody.velocity = velocity;

        }

    }
}
