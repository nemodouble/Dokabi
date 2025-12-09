using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossAnimation : MonoBehaviour
    {
        public Animator Animator;
        public BossController boss;

        public void Initialize()
        {
            if (boss == null)
                boss = GetComponent<BossController>();

            if (Animator == null)
            {
                // BossController가 가지고 있는 Animator(또는 애니메이션용 컴포넌트)를 우선 사용
                if (boss != null && boss.TryGetComponent(out Animator bossAnimator))
                {
                    Animator = bossAnimator;
                }
                else
                {
                    Animator = GetComponent<Animator>();
                }
            }
        }

        public void PlayTrigger(string triggerName)
        {
            if (Animator == null || string.IsNullOrEmpty(triggerName))
                return;
            Animator.SetTrigger(triggerName);
        }

        public void SetBool(string paramName, bool value)
        {
            if (Animator == null || string.IsNullOrEmpty(paramName))
                return;
            Animator.SetBool(paramName, value);
        }

        public void SetFloat(string paramName, float value)
        {
            if (Animator == null || string.IsNullOrEmpty(paramName))
                return;
            Animator.SetFloat(paramName, value);
        }
    }
}
