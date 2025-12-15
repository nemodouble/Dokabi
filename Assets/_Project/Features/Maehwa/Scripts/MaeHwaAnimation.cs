using System.Collections;
using _Project.Features.Boss.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Maehwa.Scripts
{
    // MaeHwa용 애니메이션 컨트롤러: FSM 상태 진입 이벤트를 구독해서 트리거를 쏜다.
    public class MaeHwaAnimation : BossAnimation
    {
        [SerializeField] private MaehwaContext maehwaContext;
        [SerializeField] private BossController<MaehwaStateId, MaehwaContext> maeHwaController;

        private new void Awake()
        {
            base.Awake();

            if (maeHwaController == null)
                maeHwaController = GetComponent<BossController<MaehwaStateId, MaehwaContext>>();

            if (maehwaContext == null)
                maehwaContext = GetComponent<MaehwaContext>();

            // 상위 클래스에서 사용하는 공통 컨텍스트 캐시에도 넣어준다.
            if (bossContext == null)
                bossContext = maehwaContext;
        }

        protected override void SubscribeStateEvents()
        {
            if (maehwaContext == null)
            {
                // BossAnimation에서 캐싱한 컨텍스트가 있다면 MaehwaContext로 캐스팅 시도
                if (bossContext == null)
                    bossContext = GetComponent<MaehwaContext>();

                maehwaContext = bossContext as MaehwaContext;
            }

            if (maehwaContext != null)
            {
                maehwaContext.OnStateEntered += OnStateEntered;
            }
        }

        protected override void UnsubscribeStateEvents()
        {
            if (maehwaContext != null)
            {
                maehwaContext.OnStateEntered -= OnStateEntered;
            }
        }

        private void OnStateEntered(MaehwaStateId stateId)
        {
            HandleTrigger(stateId);
        }
        
        private void HandleTrigger(MaehwaStateId stateId)
        {
            var trigger = stateId.ToString();
            if (trigger == "null" || string.IsNullOrEmpty(trigger))
                return;
            PlayTrigger(trigger);
        }
    }
}
