using UnityEngine;

namespace  WeirdBrothers.AnimationHelper
{
    public static class AnimationHelper
    {
        public static bool AnimatorIsPlaying(this Animator animator, string stateName, int layer = 0)
        {
            return AnimatorIsPlaying(animator, layer) && animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
        }

        private static bool AnimatorIsPlaying(Animator animator, int layer = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layer).length >
                   animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }
    }
}

