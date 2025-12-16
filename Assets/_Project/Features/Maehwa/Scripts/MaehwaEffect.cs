using _Project.Features.Boss.Scripts;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaehwaEffect : BossEffect
    {
        [SerializeField] private MaehwaContext maehwaContext;
        [Header("Particles")] 
        [SerializeField] private ParticleSystem walkPS;
        [SerializeField] private ParticleSystem dashPS;
        [SerializeField] private ParticleSystem rampagePS;
        [SerializeField] private ParticleSystem teleportPS;
        [SerializeField] private GameObject downEffect;
        /// <summary>
        /// 다운 스매시 추가 경직 시 사용하는 이펙트/히트 오브젝트
        /// </summary>
        public GameObject DownEffect => downEffect;

        private void Awake()
        {
            if (maehwaContext == null)
                maehwaContext = GetComponent<MaehwaContext>();
        }

        private void OnEnable()
        {
            if (maehwaContext == null)
                maehwaContext = GetComponent<MaehwaContext>();

            if (maehwaContext != null)
            {
                maehwaContext.OnStateEntered += OnStateEntered;
                maehwaContext.OnStateExited += OnStateExited;
            }
        }

        private void OnDisable()
        {
            if (maehwaContext != null)
            {
                maehwaContext.OnStateEntered -= OnStateEntered;
                maehwaContext.OnStateExited -= OnStateExited;
            }
        }

        private void OnStateEntered(MaehwaStateId stateId)
        {
            switch (stateId)
            {
                // 패턴(또는 패턴 선택 구간) 시작 시 플레이어를 바라보도록 처리
                // 첫 진입(초기 시작): prevPhase == ""
                case MaehwaStateId.StartWait:
                // 한 사이클 끝나고 다음 행동 고르기 직전
                case MaehwaStateId.EndAttack:
                // 공격 패턴 시작 직전(공격 후보 뽑는 순간)
                case MaehwaStateId.SelectAttack:
                // 콤보 중간에도 다시 플레이어를 바라보는 지점
                case MaehwaStateId.ComboFirstBeforeWait:
                case MaehwaStateId.ComboSecondBeforeWait:
                case MaehwaStateId.ComboThirdBeforeWait:
                    if (maehwaContext != null)
                    {
                        maehwaContext.SetToLookPlayer();
                    }
                    break;

                // 걷기 시작: Walk 진입 시 이펙트 시작
                case MaehwaStateId.WalkLeft:
                    maehwaContext.SetLookingDir(BossContext<MaehwaStateId>.LookingDir.Left);
                    PlayParticle(walkPS);
                    break;
                case MaehwaStateId.WalkRight:
                    maehwaContext.SetLookingDir(BossContext<MaehwaStateId>.LookingDir.Right);
                    PlayParticle(walkPS);
                    break;

                // 대시류 시작 (바디태클/콤보 3타/난무 등 필요 시 확장)
                case MaehwaStateId.BodyDash:
                case MaehwaStateId.HorizonRun:
                    PlayParticle(dashPS);
                    break;

                // 난무 준비 시작
                case MaehwaStateId.RampageStart:
                    PlayParticle(rampagePS);
                    break;

                // 텔레포트 시작
                case MaehwaStateId.DownBlink:
                    PlayParticle(teleportPS);
                    break;
                // 다운 가속 시작
                case MaehwaStateId.DownGetAccel:
                    downEffect.SetActive(true);
                    break;
            }
        }

        private void OnStateExited(MaehwaStateId stateId)
        {
            switch (stateId)
            {
                // 걷기 종료: Walk에서 빠져나갈 때 이펙트 종료
                case MaehwaStateId.WalkLeft:
                case MaehwaStateId.WalkRight:
                    StopParticle(walkPS);
                    break;

                // 대시 종료
                case MaehwaStateId.BodyDash:
                case MaehwaStateId.HorizonRun:
                    StopParticle(dashPS);
                    break;

                // 난무 준비 종료
                case MaehwaStateId.RampageBeforeNoticeWait:
                    StopParticle(rampagePS);
                    break;

                // 텔레포트 종료 (착지 등)
                case MaehwaStateId.DownSmashRampageWait:
                case MaehwaStateId.DownSmashWait:
                    StopParticle(teleportPS);
                    break;
                case MaehwaStateId.DownGetAccel:
                    downEffect.SetActive(true);
                    break;
            }
        }
        
    }
}
