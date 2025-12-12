using _Project.Features.Boss.Scripts;
using FMODUnity;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaehwaSound : BossSound
    {
        [Header("MaeHwa Voice & SFX")] 
        public EventReference horizonAttackEvent;
        public EventReference bodyAttackEvent;
        public EventReference comboFirstAttackEvent;
        public EventReference comboSecondAttackEvent;
        public EventReference comboStingAttackEvent;
        public EventReference downSmashEvent;
        public EventReference rampageWindEvent;
        public EventReference rampageRiseEvent;
        public EventReference dashEvent;
        public EventReference walkEvent;
        public EventReference yell1;
        public EventReference yell2;
        public EventReference yell3;
        public EventReference jump;
        public EventReference land;
        public EventReference deadVoice;
        public EventReference outro;
        public EventReference teleportEvent;

        private MaehwaContext _context;

        private void Awake()
        {
            if (boss == null)
                boss = GetComponent(typeof(BossController<,>));
            if (_context == null && boss != null)
                boss.TryGetComponent(out _context);
        }

        private void OnEnable()
        {
            if (_context == null && boss != null)
                boss.TryGetComponent(out _context);

            if (_context != null)
            {
                _context.OnStateEntered += OnStateEntered;
                _context.OnStateExited += OnStateExited;
            }
        }

        private void OnDisable()
        {
            if (_context != null)
            {
                _context.OnStateEntered -= OnStateEntered;
                _context.OnStateExited -= OnStateExited;
            }
        }

        private void OnStateEntered(MaehwaStateId stateId)
        {
            var id = stateId.ToString();

            switch (id)
            {
                // ===== 가로베기 =====
                case "HorizonStep":
                    PlayHorizonAttack();
                    PlayYell2();
                    break;
                case "HorizonBeforeWait":
                    PlayYell3();
                    break;

                // ===== 몸통 박치기 =====
                case "BodyAfterDashWait":
                    PlayBodyAttack();
                    break;

                // ===== 콤보 =====
                case "ComboFirstDashOrWait":
                    PlayComboFirst();
                    PlayYell1();
                    break;
                case "ComboSecondDashOrWait":
                    PlayComboSecond();
                    PlayYell2();
                    break;
                case "ComboThirdBeforeWait":
                    PlayComboSting();
                    PlayYell3();
                    break;

                // ===== 난무 =====
                case "RampageStart":
                    PlayRampageRise();
                    PlayYell1();
                    PlayJump();
                    PlayRampageWind();
                    break;

                // ===== 다운 스매시 =====
                case "DownAirWait":
                    PlayDownSmash();
                    break;
                case "DownGetAccel":
                    PlayLand();
                    break;

                // ===== 텔레포트 =====
                case "DownStart":
                    PlayTeleport();
                    PlayYell1();
                    break;

                default:
                    break;
            }
        }

        private void OnStateExited(MaehwaStateId stateId)
        {
            // 현재는 입장 시점에만 SFX를 재생하고, 퇴장 시점에 맞춰 끌 루프형 사운드가 없다면 비워둔다.
            // 나중에 Walk 루프, Dash 루프 등을 추가하면 여기서 Stop을 처리하면 된다.
            var id = stateId.ToString();

            switch (id)
            {
                // 예시: 걷기 루프가 있다면
                // case "WalkLeft":
                // case "WalkRight":
                //     StopWalkLoop();
                //     break;

                default:
                    break;
            }
        }

        public void PlayHorizonAttack() => PlayOneShot(horizonAttackEvent);
        public void PlayBodyAttack() => PlayOneShot(bodyAttackEvent);
        public void PlayComboFirst() => PlayOneShot(comboFirstAttackEvent);
        public void PlayComboSecond() => PlayOneShot(comboSecondAttackEvent);
        public void PlayComboSting() => PlayOneShot(comboStingAttackEvent);
        public void PlayDownSmash() => PlayOneShot(downSmashEvent);
        public void PlayRampageWind() => PlayOneShot(rampageWindEvent);
        public void PlayRampageRise() => PlayOneShot(rampageRiseEvent);
        public void PlayYell1() => PlayOneShot(yell1);
        public void PlayYell2() => PlayOneShot(yell2);
        public void PlayYell3() => PlayOneShot(yell3);
        public void PlayJump() => PlayOneShot(jump);
        public void PlayLand() => PlayOneShot(land);
        public void PlayDeadVoice() => PlayOneShot(deadVoice);
        public void PlayOutro() => PlayOneShot(outro);
        public void PlayTeleport() => PlayOneShot(teleportEvent);

        private void PlayOneShot(EventReference evt)
        {
            if (evt.IsNull)
                return;
            RuntimeManager.PlayOneShot(evt, transform.position);
        }
    }
}
