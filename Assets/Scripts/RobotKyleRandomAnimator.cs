using System.Collections;
using UnityEngine;

namespace Amax.QuantumDemo
{
    public class RobotKyleRandomAnimator : MonoBehaviour
    {
        [SerializeField] private Animator kyleAnimator;
        
        private static readonly int AnimatorParameterSpeed = Animator.StringToHash("Speed");
        private static readonly int AnimatorParameterJump = Animator.StringToHash("Jump");
        private static readonly int AnimatorParameterGrounded = Animator.StringToHash("Grounded");
        private static readonly int AnimatorParameterMotionSpeed = Animator.StringToHash("MotionSpeed");

        private float RandomSpeed => 3f + 3f * Random.value;
        private float RandomWaitTime => 0.5f + Random.value * 2f;
        private bool RandomJump => Random.value > 0.66f;
        
        private float JumpDuration => 0.5f + Random.value * 0.5f;
        
        private void Start()
        {
            kyleAnimator.fireEvents = false;
            StartCoroutine(RandomAnimationCoroutine());
        }

        private IEnumerator RandomAnimationCoroutine()
        {
            kyleAnimator.SetFloat(AnimatorParameterMotionSpeed, 1f);
            
            while (true)
            {
                
                kyleAnimator.SetFloat(AnimatorParameterSpeed, RandomSpeed);

                if (RandomJump)
                {
                    kyleAnimator.SetBool(AnimatorParameterJump, true);
                    kyleAnimator.SetBool(AnimatorParameterGrounded, false);
                    
                    yield return null;
                    kyleAnimator.SetBool(AnimatorParameterJump, false);
                    
                    yield return new WaitForSeconds(JumpDuration);
                    
                    kyleAnimator.SetBool(AnimatorParameterGrounded, true);
                }
                
                yield return new WaitForSeconds(RandomWaitTime);
            }
        }
    }

}
