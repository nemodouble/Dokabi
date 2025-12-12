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
            var id = stateId.ToString();

            switch (id)
            {
                // 걷기 시작: Walk 진입 시 이펙트 시작
                case "WalkLeft":
                case "WalkRight":
                    PlayParticle(walkPS);
                    break;

                // 대시류 시작 (바디태클/콤보 3타/난무 등 필요 시 확장)
                case "BodyDash":
                case "ComboThirdDash":
                case "RampageRise":
                    PlayParticle(dashPS);
                    break;

                // 난무 준비 시작
                case "RampageStart":
                    PlayParticle(rampagePS);
                    break;

                // 텔레포트 시작
                case "DownStart":
                    PlayParticle(teleportPS);
                    break;
            }
        }

        private void OnStateExited(MaehwaStateId stateId)
        {
            var id = stateId.ToString();

            switch (id)
            {
                // 걷기 종료: Walk에서 빠져나갈 때 이펙트 종료
                case "WalkLeft":
                case "WalkRight":
                    StopParticle(walkPS);
                    break;

                // 대시 종료
                case "BodyDash":
                case "ComboThirdDash":
                case "RampageRise":
                    StopParticle(dashPS);
                    break;

                // 난무 준비 종료
                case "RampageNotice":
                    StopParticle(rampagePS);
                    break;

                // 텔레포트 종료 (착지 등)
                case "DownStart":
                    StopParticle(teleportPS);
                    break;
            }
        }
    }
}
