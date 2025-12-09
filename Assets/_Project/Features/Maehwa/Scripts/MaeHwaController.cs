using System.Collections;
using System.Collections.Generic;
using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using _Project.Features.Boss.Scripts.State.Dead;
using _Project.Features.Boss.Scripts.State.Moving;
using Boss.MaeHwa;
using Character.Enemy.Boss.MaeHwa;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaeHwaController : BossController
    {
        private enum LookingDir
        {
            RightDir = 1,
            LeftDir = -1
        }
        private LookingDir lookingDir = LookingDir.LeftDir;
        private Vector3 centerPos;

        // 패턴 대기
        private readonly BossState startingWait = new WaitState("Start-Wait", 5.5f, false);
        private readonly BossState selectAttack = new EmptyState("Select-Attack");

        private BossState _endState;
        private ParticleSystem dashPS;

        #region 걷기

        private BossState walkLeft;
        private BossState walkRight;
        private float walkDistance;
        private ParticleSystem walkPS;

        #endregion

        #region 가로베기

        // horizon attack
        private GameObject horizonAttackRange;
        private Vector3 leftEdgePos;
        private Vector3 rightEdgePos;

        private readonly BossState horizonAttackStart = new EmptyState("Horizon-Start", 3);
        private BossState horizonStep;
        private BossState horizonBeforeWait;
        private BossState horizonAttack;
        private BossState horizonAfterWait;

        #endregion

        #region 바디태클

        // body attack
        private GameObject bodyWall;
        private GameObject bodyStrongAttack;
        private GameObject bossDangerRange;

        private readonly BossState bodyAttackStart = new EmptyState("Body-Start");
        private BossState bodyLeftDash;
        private BossState bodyRightDash;
        private BossState bodyAfterDashWait;
        private BossState bodyAttack;
        private BossState bodyAfterAttackWait;

        #endregion

        #region 콤보

        // combo attack
        private GameObject comboNormalAttack;
        private GameObject comboStingAttack;

        private readonly BossState comboAttackStart = new EmptyState("Combo-Start");
        private BossState comboFirstAttackStart;
        private BossState comboFirstBeforeWait;
        private BossState comboFirstNoDash;
        private BossState comboFirstLeftDash;
        private BossState comboFirstRightDash;
        private BossState comboFirstAttack;
        private BossState comboFirstAfterWait;
        private BossState comboSecondWait;
        private BossState comboSecondLeftDash;
        private BossState comboSecondRightDash;
        private BossState comboSecondNoDash;
        private BossState comboSecondAttack;
        private BossState comboSecondAfterWait;
        private BossState comboThirdBeforeWait;
        private BossState comboThirdAttack;
        private BossState comboThirdLeftDash;
        private BossState comboThirdRightDash;
        private BossState comboThirdAfterWait;



        #endregion

        #region 난무

        //Rampage
        private ParticleSystem rampagePS;
        private GameObject rampageRange;
        private float originGravity;

        private readonly BossState rampageAttackStart = new EmptyState("Rampage-Start", 10);
        private BossState rampageRise;
        private BossState rampageRiseWait;
        private BossState rampageBeforeNoticeWait;
        private BossState rampageBlink;
        private BossState rampageNotice;
        private BossState rampageToDown;

        #endregion

        #region 다운스매쉬

        //downSmash
        private bool haveMoreStagger;
        private GameObject downEffect;
        private ParticleSystem teleportPS;
        private ParticleSystem landPS;

        private readonly BossState downStart = new EmptyState("Down-Start", 7);
        private BossState downPlayPS = new EmptyState("Down-PlayPS");
        private BossState downBlink = new EmptyState("Down-Blink");
        private BossState downAirWait;
        private BossState downGetAccel;
        private BossState downSmashWait;
        private BossState downSmashRampageWait;

        #endregion

        public bool isBackStep;


        protected override void Start()
        {
            // transform, gameObject 할당
            var parent = gameObject.transform.parent;

            bodyWall = transform.Find("PushWall").gameObject;
            bossDangerRange = transform.Find("DangerRange").gameObject;
            bodyStrongAttack = transform.Find("StrongAttack").gameObject;
            comboNormalAttack = transform.Find("NormalAttack").gameObject;
            comboStingAttack = transform.Find("StingAttack").gameObject;
            downEffect = transform.Find("DownEffect").gameObject;

            teleportPS = transform.Find("TeleportPS").GetComponent<ParticleSystem>();
            hitPS = parent.Find("HitPS").GetComponent<ParticleSystem>();
            deadPS = transform.Find("DeadPs").GetComponent<ParticleSystem>();
            dashPS = transform.Find("PlatDashPS").GetComponent<ParticleSystem>();
            walkPS = transform.Find("WalkPS").GetComponent<ParticleSystem>();
            landPS = transform.Find("LandPS").GetComponent<ParticleSystem>();

            rampageRange = parent.Find("RampageRange").gameObject;
            horizonAttackRange = parent.Find("HorizonAttack").gameObject;

            leftEdgePos = parent.Find("LeftEdge").transform.position;
            rightEdgePos = parent.Find("RightEdge").transform.position;

            rampagePS = parent.Find("RampagePS").GetComponent<ParticleSystem>();
            centerPos = parent.position;

            #region 패턴할당

            // 패턴 할당

            // 걷기
            walkLeft = new MoveByVelocity("Walk", Vector2.left, walkSpeed, walkTime);
            walkRight = new MoveByVelocity("Walk",Vector2.right, walkSpeed, walkTime);

            // 가로 베기
            horizonBeforeWait = new WaitState("Horizon-BeforeWait", horizonBeforeWaitTime, true);
            horizonAttack = new AttackFixedRange("Horizon-Attack", horizonAttackRange);
            horizonAfterWait = new WaitState("Horizon-AfterWait", horizonAfterWaitTime, true);

            // 바디태클
            bodyAfterDashWait = new WaitState("Body-AfterDashWait", bodyAfterDashWaitTime);
            bodyLeftDash = new MoveByVelocity("Body-Dash", Vector2.left, bodyDashSpeed, bodyDashTime);
            bodyRightDash = new MoveByVelocity("Body-Dash", Vector2.right, bodyDashSpeed, bodyDashTime);
            bodyAttack = new AttackFixedRange("Body-Attack", bodyStrongAttack);
            bodyAfterAttackWait = new WaitState("Body-AfterAttackWait", bodyAfterAttackWaitTime);

            // 콤보 공격
            comboFirstAttackStart = new EmptyState("Combo-FirstAttackStart");
            comboFirstBeforeWait = new WaitState("Combo-First-BeforeWait", comboFirstBeforeWaitTime);
            comboFirstAttack = new AttackFixedRange("Combo-First-Attack", comboNormalAttack);
            comboFirstNoDash = new WaitState("Combo-First-DashOrWait", comboNormalLength);
            comboFirstLeftDash = new MoveByVelocity("Combo-First-DashOrWait", Vector2.left, comboNormalSpeed, comboNormalLength);
            comboFirstRightDash = new MoveByVelocity("Combo-First-DashOrWait", Vector2.right, comboNormalSpeed, comboNormalLength);
            comboFirstAfterWait = new WaitState("Combo-First-AfterWait", comboAfterFirstWaitTime);
            comboSecondWait = new WaitState("Combo-Second-BeforeWait", comboBeforeSecondWaitTime);
            comboSecondNoDash = new WaitState("Combo-Second-DashOrWait", comboNormalLength);
            comboSecondLeftDash = new MoveByVelocity("Combo-Second-DashOrWait", Vector2.left, comboNormalSpeed, comboNormalLength);
            comboSecondRightDash = new MoveByVelocity("Combo-Second-DashOrWait", Vector2.right, comboNormalSpeed, comboNormalLength);
            comboSecondAttack = new AttackFixedRange("Combo-Second-Attack", comboNormalAttack);
            comboSecondAfterWait = new WaitState("Combo-Second-AfterWait", comboAfterSecondWaitTime);
            comboThirdBeforeWait = new WaitState("Combo-Third-BeforeWait", comboBeforeThirdWaitTime);
            comboThirdAttack = new AttackFixedRange("Combo-Third-Attack", comboStingAttack);
            comboThirdLeftDash = new MoveByVelocity("Combo-Third-Dash", Vector2.left, comboStingSpeed, comboStingTime);
            comboThirdRightDash = new MoveByVelocity("Combo-Third-Dash", Vector2.right, comboStingSpeed, comboStingTime);
            comboThirdAfterWait = new WaitState("Combo-AfterWait", comboAfterThirdWaitTime);

            // 난무
            rampageRise = new MoveLikeJump("Rampage-Rise", rampageRiseSpeed, rampageRiseTime);
            rampageRiseWait = new WaitState("Rampage-RiseWait", rampageRiseWaitTime, true);
            rampageBeforeNoticeWait = new WaitState("Rampage-BeforeNoticeWait", rampageBeforeNoticeWaitTime, true);
            rampageBlink = new WaitState("Rampage-Blink", rampageBlinkWait, true);
            rampageNotice = new RampageAttackState("Rampage-Notice", rampageNoticeInterval, rampageBeforeAttackTime, rampageAttackTime, rampageAttackAfterWaitTime);
            rampageToDown = new EmptyState("Down-AirWait");

            // 다운어택
            downAirWait = new WaitState("Down-AirWait", downAirWaitTime, true);
            downGetAccel = new MoveByVelocity("Down-GetAccel", Vector2.down, downAccel, downAccelTime);
            downSmashWait = new WaitState("Down-SmashWait", downAfterSmashTime);
            downSmashRampageWait = new WaitState("Down-SmashWait", rampageStaggerTime);

            // 사망
            DeadState = new DeadNormal("Dead");

            _endState = new WaitState("EndPhase", betweenPhaseWaitTime);

            #endregion

            base.Start();
        }

        private void Update()
        {
            transform.localScale = new Vector3((int)lookingDir, 1, 1);
            if (prevPhase == "Walk")
            {
                walkDistance += Time.deltaTime;
                if(walkDistance >= 0.3f)
                {
                    RuntimeManager.PlayOneShot(walkEvent);
                    walkDistance = 0f;
                }
            }
        }

        protected override IEnumerator Dead()
        {
            RuntimeManager.PlayOneShot(deadVoice);
            yield return base.Dead();
        }

        protected override List<BossState> GetAblePhaseList()
        {
            var ablePhaseList = new List<BossState>();
            if (prevPhase == "")
            {
                SetLookingDir();
                ablePhaseList.Add(startingWait);
            }
            else switch (prevPhase)
            {
                case "Start-Wait":
                case "EndPhase":
                    SetLookingDir();
                    ablePhaseList.Add(new EmptyState("Select-Walk"));
                    if(IsInDistance(0f,3f))
                        ablePhaseList.Add(new EmptyState("Select-Step", 2));
                    ablePhaseList.Add(selectAttack);
                    break;

                case "Select-Walk":
                    lookingDir = IsInDistance(0f, 4f) ^ (Player.transform.position.x > transform.position.x)
                        ? LookingDir.RightDir
                        : LookingDir.LeftDir;
                    walkPS.Play();
                    ablePhaseList.Add(lookingDir == LookingDir.RightDir ? walkRight : walkLeft);
                    break;
                case "Walk":
                    walkPS.Stop();
                    ablePhaseList.Add(selectAttack);
                    break;

                case "Select-Step":
                    RuntimeManager.PlayOneShot(dashEvent);
                    ablePhaseList.Add(IsInDistance(0f, 3f)
                        ? GetStepToKeepDistance("BackStep", 6f, out isBackStep)
                        : GetStepToKeepDistance("FrontStep", 6f, out isBackStep));
                    break;
                case "FrontStep":
                case "BackStep":
                    ablePhaseList.Add(selectAttack);
                    break;

                case "Select-Attack":
                    SetLookingDir();
                    Rigid2D.velocity = Vector2.zero;
                    ablePhaseList.Add(comboAttackStart);
                    ablePhaseList.Add(bodyAttackStart);
                    if(Mathf.Abs(Player.transform.position.x - transform.parent.position.x) <= Mathf.Abs(rightEdgePos.x - transform.parent.position.x))
                        ablePhaseList.Add(horizonAttackStart);
                    ablePhaseList.Add(rampageAttackStart);
                    ablePhaseList.Add(downStart);
                    break;


                // 가로베기
                case "Horizon-Start":
                    SetHorizonStep();
                    RuntimeManager.PlayOneShot(dashEvent);
                    dashPS.Play();
                    ablePhaseList.Add(horizonStep);
                    break;
                case "Horizon-Step":
                    var bossTransform = transform;
                    SetLookingDir(bossTransform.position.x > bossTransform.parent.position.x
                        ? LookingDir.LeftDir
                        : LookingDir.RightDir);
                    RuntimeManager.PlayOneShot(horizonAttackEvent);
                    RuntimeManager.PlayOneShot(yell2);
                    dashPS.Stop();
                    ablePhaseList.Add(horizonBeforeWait);
                    break;
                case "Horizon-BeforeWait":
                    horizonAttackRange.transform.localScale = transform.localScale;
                    RuntimeManager.PlayOneShot(yell3);
                    ablePhaseList.Add(horizonAttack);
                    break;
                case "Horizon-Attack":
                    ablePhaseList.Add(horizonAfterWait);
                    break;
                case "Horizon-AfterWait":
                    ablePhaseList.Add(_endState);
                    break;

                // 몸통 박치기
                case "Body-Start":
                    bossDangerRange.SetActive(false);
                    bodyWall.SetActive(true);
                    RuntimeManager.PlayOneShot(dashEvent);
                    dashPS.Play();
                    ablePhaseList.Add(Player.transform.position.x > transform.position.x
                        ? bodyRightDash
                        : bodyLeftDash);
                    break;
                case "Body-Dash":
                    Rigid2D.velocity = Vector2.zero;
                    dashPS.Stop();
                    ablePhaseList.Add(bodyAfterDashWait);
                    break;
                case "Body-AfterDashWait":
                    bossDangerRange.SetActive(true);
                    bodyWall.SetActive(false);
                    RuntimeManager.PlayOneShot(bodyAttackEvent);
                    ablePhaseList.Add(bodyAttack);
                    break;
                case "Body-Attack":
                    ablePhaseList.Add(bodyAfterAttackWait);
                    break;
                case "Body-AfterAttackWait":
                    ablePhaseList.Add(_endState);
                    break;

                // 3연격
                case "Combo-Start":
                case "Combo-Step":
                    ablePhaseList.Add(comboFirstAttackStart);
                    break;
                case "Combo-FirstAttackStart":
                    ablePhaseList.Add(comboFirstBeforeWait);
                    break;
                case "Combo-First-BeforeWait":
                    ablePhaseList.Add(IsInDistance(0f, 2.5f) ? comboFirstNoDash :
                        lookingDir == LookingDir.LeftDir ? comboFirstLeftDash : comboFirstRightDash);
                    break;
                case "Combo-First-DashOrWait":
                    Rigid2D.velocity = Vector2.zero;
                    RuntimeManager.PlayOneShot(comboFirstAttackEvent);
                    RuntimeManager.PlayOneShot(yell1);
                    ablePhaseList.Add(comboFirstAttack);
                    break;
                case "Combo-First-Attack":
                    ablePhaseList.Add(comboFirstAfterWait);
                    break;
                case "Combo-First-AfterWait":
                    SetLookingDir();
                    ablePhaseList.Add(IsInDistance(0f, 5f) ? comboSecondWait : comboThirdBeforeWait);
                    break;
                case "Combo-Second-BeforeWait":
                    ablePhaseList.Add(IsInDistance(0f, 2.5f) ? comboSecondNoDash :
                        lookingDir == LookingDir.LeftDir ? comboSecondLeftDash : comboSecondRightDash);
                    break;
                case "Combo-Second-DashOrWait":
                    Rigid2D.velocity = Vector2.zero;
                    RuntimeManager.PlayOneShot(comboSecondAttackEvent);
                    RuntimeManager.PlayOneShot(yell2);
                    ablePhaseList.Add(comboSecondAttack);
                    break;
                case "Combo-Second-Attack":
                    ablePhaseList.Add(comboSecondAfterWait);
                    break;
                case "Combo-Second-AfterWait":
                    SetLookingDir();
                    ablePhaseList.Add(comboThirdBeforeWait);
                    break;
                case "Combo-Third-BeforeWait":
                    RuntimeManager.PlayOneShot(comboStingAttackEvent);
                    RuntimeManager.PlayOneShot(yell3);
                    dashPS.Play();
                    ablePhaseList.Add(comboThirdAttack);
                    break;
                case "Combo-Third-Attack":
                    ablePhaseList.Add(lookingDir == LookingDir.RightDir? comboThirdRightDash : comboThirdLeftDash);
                    break;
                case "Combo-Third-Dash":
                    Rigid2D.velocity = Vector2.zero;
                    dashPS.Stop();
                    ablePhaseList.Add(comboThirdAfterWait);
                    break;
                case "Combo-AfterWait":
                    ablePhaseList.Add(_endState);
                    break;


                // 난무
                case "Rampage-Start":
                    Rigid2D.velocity = Vector2.zero;
                    RuntimeManager.PlayOneShot(rampageRiseEvent);
                    RuntimeManager.PlayOneShot(yell1);
                    RuntimeManager.PlayOneShot(jump);
                    RuntimeManager.PlayOneShot(rampageWindEvent);
                    rampagePS.Play();
                    ablePhaseList.Add(rampageRise);
                    break;
                case "Rampage-Rise":
                    ablePhaseList.Add(rampageRiseWait);
                    break;
                case "Rampage-RiseWait":
                    originGravity = Rigid2D.gravityScale;
                    Rigid2D.gravityScale = 0;
                    ablePhaseList.Add(rampageBeforeNoticeWait);
                    break;
                case "Rampage-BeforeNoticeWait":
                    // ablePhaseList.Add(rampageBlink);
                    // break;
                case "Rampage-Blink":
                    transform.position += new Vector3(0, 10000);
                    ablePhaseList.Add(rampageNotice);
                    break;
                case "Rampage-Notice":
                    rampagePS.Stop();
                    Rigid2D.gravityScale = originGravity;
                    transform.position = new Vector2(Player.transform.position.x, centerPos.y + 7f);
                    haveMoreStagger = true;
                    ablePhaseList.Add(rampageToDown);
                    break;

                // 상단 내려찍기
                case "Down-Start":
                    teleportPS.Play();
                    RuntimeManager.PlayOneShot(teleportEvent);
                    RuntimeManager.PlayOneShot(yell1);
                    originGravity = Rigid2D.gravityScale;
                    Rigid2D.gravityScale = 0;
                    transform.position = new Vector2(Player.transform.position.x, centerPos.y + 7f);
                    ablePhaseList.Add(downBlink);
                    break;
                case "Down-Blink":
                    ablePhaseList.Add(downAirWait);
                    break;
                case "Down-AirWait":
                    downEffect.SetActive(true);
                    RuntimeManager.PlayOneShot(downSmashEvent);
                    GetComponent<ParticleTrigger>().Reset();
                    ablePhaseList.Add(downGetAccel);
                    break;
                case "Down-GetAccel":
                    RuntimeManager.PlayOneShot(land);
                    // landPS.Play();
                    if (haveMoreStagger)
                    {
                        haveMoreStagger = false;
                        ablePhaseList.Add(downSmashRampageWait);
                    }
                    else
                        ablePhaseList.Add(downSmashWait);
                    break;
                case "Down-SmashWait":
                    ablePhaseList.Add(_endState);
                    break;


                default:
                    Debug.Log("보스 상태 지정 안됨");
                    ablePhaseList.Add(startingWait);
                    break;
            }

            return ablePhaseList;
        }

        private void SetHorizonStep()
        {
            var position = transform.position;
            Vector3 targetEdgePosition;
            Vector2 stepDir;
            if (Player.transform.position.x < transform.position.x)
            {
                targetEdgePosition = rightEdgePos;
                stepDir = transform.position.x > rightEdgePos.x ? Vector2.left : Vector2.right;
            }
            else
            {
                targetEdgePosition = leftEdgePos;
                stepDir = transform.position.x < leftEdgePos.x ? Vector2.right : Vector2.left;
            }
            horizonStep = new MoveByVelocityToPos("Horizon-Step", stepDir, horizonStepSpeed, 10, targetEdgePosition);
            lookingDir = stepDir == Vector2.right? LookingDir.RightDir : LookingDir.LeftDir;
        }

        private bool IsInDistance(float minDistance, float maxDistance)
        {
            var distance = Mathf.Abs(transform.position.x - Player.transform.position.x);
            return distance >= minDistance && distance <= maxDistance;
        }
        private BossState GetStepToKeepDistance(string stepName, float targetDistance, out bool isBackStep)
        {

            BossState rightStep = new Step(stepName, Vector2.right * 3f, 20f, 10, 0.3f);
            BossState leftStep = new Step(stepName, Vector2.left * 3f, 20f, 10, 0.3f);
            if (Mathf.Abs(transform.position.x - Player.transform.position.x) >= targetDistance)
            {
                isBackStep = false;
                return transform.position.x > Player.transform.position.x ? leftStep : rightStep;
            }
            else
            {
                isBackStep = true;
                return transform.position.x > Player.transform.position.x ? rightStep : leftStep;
            }
        }

        public MaeHwaRampageRange InstantiateRampageRange(Vector2 rampagePos, Vector3 rotation)
        {
            return Instantiate(rampageRange, rampagePos,Quaternion.Euler(rotation)).GetComponent<MaeHwaRampageRange>();
        }
        /// <summary>
        /// Set Boss's LookingDir to Player direction
        /// </summary>
        private void SetLookingDir()
        {
            lookingDir = transform.position.x > Player.transform.position.x ? LookingDir.LeftDir : LookingDir.RightDir;
        }

        private void SetLookingDir(LookingDir dir)
        {
            lookingDir = dir;
        }
    }
}