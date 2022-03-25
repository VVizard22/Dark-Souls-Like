using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class PlayerManager : MonoBehaviour
    {
        InputHandler _inputHandler;
        Animator _anim;

        void Start()
        {
            _inputHandler = GetComponent<InputHandler>();
            _anim = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            _inputHandler.SetIsInteracting(_anim.GetBool("isInteracting"));
            _inputHandler.ResetRollFlag();
            _inputHandler.ResetDanceFlag();
        }
    }
}
