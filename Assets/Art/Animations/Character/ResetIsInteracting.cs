using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike
{
    public class ResetIsInteracting : StateMachineBehaviour
    {
        public delegate void AnimatorFinish(ActionFlag f);
        public static event AnimatorFinish finishedAnimation;
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isInteracting", false);
            finishedAnimation?.Invoke(ActionFlag.None);
        }
    }
}
