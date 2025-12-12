using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using _Project.Features.Boss.Scripts.State.Dead;
using _Project.Features.Boss.Scripts.State.Moving;
using _Project.Features.Maehwa.Scripts.Phase;
using Boss.MaeHwa;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaehwaStateMachine : BossStateMachine<MaehwaStateId, MaehwaContext>
    {
        public override void Initialize()
        {
            Context = GetComponent<MaehwaContext>();
            base.Initialize();

            var s = Context.stats;

            // 시작 대기 상태
            var startWait = new WaitState<MaehwaStateId>(MaehwaStateId.StartWait, s.startWaitTime, false);
            StateMachine.AddState(MaehwaStateId.StartWait, startWait);
            StateMachine.SetInitialState(MaehwaStateId.StartWait);

            // 사망 상태
            var deadState = new DeadNormal<MaehwaStateId>(MaehwaStateId.Dead);
            StateMachine.AddState(MaehwaStateId.Dead, deadState);
            StateMachine.AddAnyTransition(MaehwaStateId.Dead, ctx => ctx.IsDead());

            // Select-Pattern
            var selectPattern = new WaitState<MaehwaStateId>(MaehwaStateId.SelectPattern, s.betweenPhaseWaitTime, false);
            StateMachine.AddState(MaehwaStateId.SelectPattern, selectPattern);
            StateMachine.AddTransition(MaehwaStateId.StartWait, MaehwaStateId.SelectPattern, _ => startWait.IsFinished);

            // Select-Walk
            var selectWalk = new EmptyState<MaehwaStateId>(MaehwaStateId.SelectWalk);
            StateMachine.AddState(MaehwaStateId.SelectWalk, selectWalk);

            var walkLeft = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.WalkLeft, Vector2.left, s.walkSpeed, s.walkTime, 0f);
            var walkRight = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.WalkRight, Vector2.right, s.walkSpeed, s.walkTime, 0f);
            StateMachine.AddState(MaehwaStateId.WalkLeft, walkLeft);
            StateMachine.AddState(MaehwaStateId.WalkRight, walkRight);

            // Select-Step
            var selectStep = new EmptyState<MaehwaStateId>(MaehwaStateId.SelectStep);
            StateMachine.AddState(MaehwaStateId.SelectStep, selectStep);

            var frontStep = new Step<MaehwaStateId>(MaehwaStateId.FrontStep, Vector2.right * 3f, 20f, 10, 0.3f);
            var backStep = new Step<MaehwaStateId>(MaehwaStateId.BackStep, Vector2.right * 3f, 20f, 10, 0.3f);
            StateMachine.AddState(MaehwaStateId.FrontStep, frontStep);
            StateMachine.AddState(MaehwaStateId.BackStep, backStep);

            // Select-Attack
            var selectAttack = new EmptyState<MaehwaStateId>(MaehwaStateId.SelectAttack);
            StateMachine.AddState(MaehwaStateId.SelectAttack, selectAttack);

            // 패턴 선택 전이
            StateMachine.AddTransition(MaehwaStateId.SelectPattern, MaehwaStateId.SelectWalk, _ => true);
            StateMachine.AddTransition(MaehwaStateId.SelectPattern, MaehwaStateId.SelectStep, ctx => ctx.IsInDistance(0f, 3f), 0, 2f);
            StateMachine.AddTransition(MaehwaStateId.SelectPattern, MaehwaStateId.SelectAttack, _ => true);

            // Select-Walk -> WalkLeft / WalkRight
            StateMachine.AddTransition(MaehwaStateId.SelectWalk, MaehwaStateId.WalkRight, ctx => ctx.CanSelectMoveRight());
            StateMachine.AddTransition(MaehwaStateId.SelectWalk, MaehwaStateId.WalkLeft, ctx => !ctx.CanSelectMoveRight());

            // Walk 종료 후 Select-Attack
            StateMachine.AddTransition(MaehwaStateId.WalkLeft, MaehwaStateId.SelectAttack, _ => walkLeft.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.WalkRight, MaehwaStateId.SelectAttack, _ => walkRight.IsFinished);

            // Select-Step -> Front / Back
            StateMachine.AddTransition(MaehwaStateId.SelectStep, MaehwaStateId.BackStep, ctx => ctx.IsInDistance(0f, 3f));
            StateMachine.AddTransition(MaehwaStateId.SelectStep, MaehwaStateId.FrontStep, ctx => !ctx.IsInDistance(0f, 3f));

            // Step 종료 후 Select-Attack
            StateMachine.AddTransition(MaehwaStateId.FrontStep, MaehwaStateId.SelectAttack, _ => frontStep.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.BackStep, MaehwaStateId.SelectAttack, _ => backStep.IsFinished);

            // === Horizon / EndPhase ===
            var horizonBeforeWait = new WaitState<MaehwaStateId>(MaehwaStateId.HorizonBeforeWait, s.horizonBeforeWaitTime, true);
            var horizonAttack = new AttackFixedRange<MaehwaStateId>(MaehwaStateId.HorizonAttack, null);
            var horizonAfterWait = new WaitState<MaehwaStateId>(MaehwaStateId.HorizonAfterWait, s.horizonAfterWaitTime, true);

            StateMachine.AddState(MaehwaStateId.HorizonBeforeWait, horizonBeforeWait);
            StateMachine.AddState(MaehwaStateId.HorizonAttack, horizonAttack);
            StateMachine.AddState(MaehwaStateId.HorizonAfterWait, horizonAfterWait);

            var endPhase = new WaitState<MaehwaStateId>(MaehwaStateId.EndPhase, s.betweenPhaseWaitTime, true);
            StateMachine.AddState(MaehwaStateId.EndPhase, endPhase);

            var horizonStart = new EmptyState<MaehwaStateId>(MaehwaStateId.HorizonStart);
            StateMachine.AddState(MaehwaStateId.HorizonStart, horizonStart);

            var horizonStep = new MoveByVelocityToPos<MaehwaStateId>(MaehwaStateId.HorizonStep, Vector2.right, s.horizonStepSpeed, 10f, Vector2.zero, 0f);
            StateMachine.AddState(MaehwaStateId.HorizonStep, horizonStep);

            StateMachine.AddTransition(MaehwaStateId.HorizonStart, MaehwaStateId.HorizonStep, _ => true);
            StateMachine.AddTransition(MaehwaStateId.HorizonStep, MaehwaStateId.HorizonBeforeWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.HorizonBeforeWait, MaehwaStateId.HorizonAttack, _ => horizonBeforeWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.HorizonAttack, MaehwaStateId.HorizonAfterWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.HorizonAfterWait, MaehwaStateId.EndPhase, _ => true);
            StateMachine.AddTransition(MaehwaStateId.EndPhase, MaehwaStateId.SelectPattern, _ => endPhase.IsFinished);

            // === Body ===
            var bodyAfterDashWait = new WaitState<MaehwaStateId>(MaehwaStateId.BodyAfterDashWait, s.bodyAfterDashWaitTime, false);
            var bodyLeftDash = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.BodyDash, Vector2.left, s.bodyDashSpeed, s.bodyDashTime, 0f);
            var bodyRightDash = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.BodyDash, Vector2.right, s.bodyDashSpeed, s.bodyDashTime, 0f);
            var bodyAttack = new AttackFixedRange<MaehwaStateId>(MaehwaStateId.BodyAttack, null);
            var bodyAfterAttackWait = new WaitState<MaehwaStateId>(MaehwaStateId.BodyAfterAttackWait, s.bodyAfterAttackWaitTime, false);

            var bodyStart = new EmptyState<MaehwaStateId>(MaehwaStateId.BodyStart);
            StateMachine.AddState(MaehwaStateId.BodyStart, bodyStart);
            StateMachine.AddState(MaehwaStateId.BodyAfterDashWait, bodyAfterDashWait);
            StateMachine.AddState(MaehwaStateId.BodyDash, bodyLeftDash);
            StateMachine.AddState(MaehwaStateId.BodyAttack, bodyAttack);
            StateMachine.AddState(MaehwaStateId.BodyAfterAttackWait, bodyAfterAttackWait);

            StateMachine.AddTransition(MaehwaStateId.BodyDash, MaehwaStateId.BodyAfterDashWait, _ => bodyLeftDash.IsFinished || bodyRightDash.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.BodyAfterDashWait, MaehwaStateId.BodyAttack, _ => bodyAfterDashWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.BodyAttack, MaehwaStateId.BodyAfterAttackWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.BodyAfterAttackWait, MaehwaStateId.EndPhase, _ => true);

            // === Combo ===
            var comboFirstAttackStart = new EmptyState<MaehwaStateId>(MaehwaStateId.ComboFirstAttackStart);
            var comboFirstBeforeWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboFirstBeforeWait, s.comboFirstBeforeWaitTime, false);
            var comboFirstAttack = new AttackFixedRange<MaehwaStateId>(MaehwaStateId.ComboFirstAttack, null);
            var comboFirstNoDash = new WaitState<MaehwaStateId>(MaehwaStateId.ComboFirstDashOrWait, s.comboNormalLength, false);
            var comboFirstLeftDash = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.ComboFirstDashOrWait, Vector2.left, s.comboNormalSpeed, s.comboNormalLength, 0f);
            var comboFirstRightDash = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.ComboFirstDashOrWait, Vector2.right, s.comboNormalSpeed, s.comboNormalLength, 0f);
            var comboFirstAfterWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboFirstAfterWait, s.comboAfterFirstWaitTime, false);

            var comboSecondBeforeWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboSecondBeforeWait, s.comboBeforeSecondWaitTime, false);
            var comboSecondNoDash = new WaitState<MaehwaStateId>(MaehwaStateId.ComboSecondDashOrWait, s.comboNormalLength, false);
            var comboSecondLeftDash = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.ComboSecondDashOrWait, Vector2.left, s.comboNormalSpeed, s.comboNormalLength, 0f);
            var comboSecondRightDash = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.ComboSecondDashOrWait, Vector2.right, s.comboNormalSpeed, s.comboNormalLength, 0f);
            var comboSecondAttack = new AttackFixedRange<MaehwaStateId>(MaehwaStateId.ComboSecondAttack, null);
            var comboSecondAfterWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboSecondAfterWait, s.comboAfterSecondWaitTime, false);

            var comboThirdBeforeWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboThirdBeforeWait, s.comboBeforeThirdWaitTime, false);
            var comboThirdAttack = new AttackFixedRange<MaehwaStateId>(MaehwaStateId.ComboThirdAttack, null);
            var comboThirdLeftDash = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.ComboThirdDash, Vector2.left, s.comboStingSpeed, s.comboStingTime, 0f);
            var comboThirdRightDash = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.ComboThirdDash, Vector2.right, s.comboStingSpeed, s.comboStingTime, 0f);
            var comboThirdAfterWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboAfterWait, s.comboAfterThirdWaitTime, false);

            var comboStart = new EmptyState<MaehwaStateId>(MaehwaStateId.ComboStart);
            var comboStep = new EmptyState<MaehwaStateId>(MaehwaStateId.ComboStep);

            StateMachine.AddState(MaehwaStateId.ComboStart, comboStart);
            StateMachine.AddState(MaehwaStateId.ComboStep, comboStep);
            StateMachine.AddState(MaehwaStateId.ComboFirstAttackStart, comboFirstAttackStart);
            StateMachine.AddState(MaehwaStateId.ComboFirstBeforeWait, comboFirstBeforeWait);
            StateMachine.AddState(MaehwaStateId.ComboFirstDashOrWait, comboFirstNoDash);
            StateMachine.AddState(MaehwaStateId.ComboFirstAttack, comboFirstAttack);
            StateMachine.AddState(MaehwaStateId.ComboFirstAfterWait, comboFirstAfterWait);
            StateMachine.AddState(MaehwaStateId.ComboSecondBeforeWait, comboSecondBeforeWait);
            StateMachine.AddState(MaehwaStateId.ComboSecondDashOrWait, comboSecondNoDash);
            StateMachine.AddState(MaehwaStateId.ComboSecondAttack, comboSecondAttack);
            StateMachine.AddState(MaehwaStateId.ComboSecondAfterWait, comboSecondAfterWait);
            StateMachine.AddState(MaehwaStateId.ComboThirdBeforeWait, comboThirdBeforeWait);
            StateMachine.AddState(MaehwaStateId.ComboThirdAttack, comboThirdAttack);
            StateMachine.AddState(MaehwaStateId.ComboThirdDash, comboThirdLeftDash);
            StateMachine.AddState(MaehwaStateId.ComboAfterWait, comboThirdAfterWait);

            StateMachine.AddTransition(MaehwaStateId.ComboStart, MaehwaStateId.ComboFirstAttackStart, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboStep, MaehwaStateId.ComboFirstAttackStart, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstAttackStart, MaehwaStateId.ComboFirstBeforeWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstBeforeWait, MaehwaStateId.ComboFirstDashOrWait, _ => comboFirstBeforeWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstDashOrWait, MaehwaStateId.ComboFirstAttack, _ => comboFirstNoDash.IsFinished || comboFirstLeftDash.IsFinished || comboFirstRightDash.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstAttack, MaehwaStateId.ComboFirstAfterWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstAfterWait, MaehwaStateId.ComboSecondBeforeWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboSecondBeforeWait, MaehwaStateId.ComboSecondDashOrWait, _ => comboSecondBeforeWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboSecondDashOrWait, MaehwaStateId.ComboSecondAttack, _ => comboSecondNoDash.IsFinished || comboSecondLeftDash.IsFinished || comboSecondRightDash.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboSecondAttack, MaehwaStateId.ComboSecondAfterWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboSecondAfterWait, MaehwaStateId.ComboThirdBeforeWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboThirdBeforeWait, MaehwaStateId.ComboThirdAttack, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboThirdAttack, MaehwaStateId.ComboThirdDash, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboThirdDash, MaehwaStateId.ComboAfterWait, _ => comboThirdLeftDash.IsFinished || comboThirdRightDash.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboAfterWait, MaehwaStateId.EndPhase, _ => true);

            // === Rampage ===
            var rampageRise = new MoveLikeJump<MaehwaStateId>(MaehwaStateId.RampageRise, s.rampageRiseSpeed, s.rampageRiseTime);
            var rampageRiseWait = new WaitState<MaehwaStateId>(MaehwaStateId.RampageRiseWait, s.rampageRiseWaitTime, true);
            var rampageBeforeNoticeWait = new WaitState<MaehwaStateId>(MaehwaStateId.RampageBeforeNoticeWait, s.rampageBeforeNoticeWaitTime, true);
            var rampageBlink = new WaitState<MaehwaStateId>(MaehwaStateId.RampageBlink, s.rampageBlinkWait, true);
            var rampageNotice = new RampageAttackState(MaehwaStateId.RampageNotice, s.rampageNoticeInterval, s.rampageBeforeAttackTime, s.rampageAttackTime, s.rampageAttackAfterWaitTime);

            var rampageStart = new EmptyState<MaehwaStateId>(MaehwaStateId.RampageStart);
            StateMachine.AddState(MaehwaStateId.RampageStart, rampageStart);
            StateMachine.AddState(MaehwaStateId.RampageRise, rampageRise);
            StateMachine.AddState(MaehwaStateId.RampageRiseWait, rampageRiseWait);
            StateMachine.AddState(MaehwaStateId.RampageBeforeNoticeWait, rampageBeforeNoticeWait);
            StateMachine.AddState(MaehwaStateId.RampageBlink, rampageBlink);
            StateMachine.AddState(MaehwaStateId.RampageNotice, rampageNotice);

            StateMachine.AddTransition(MaehwaStateId.RampageStart, MaehwaStateId.RampageRise, _ => true);
            StateMachine.AddTransition(MaehwaStateId.RampageRise, MaehwaStateId.RampageRiseWait, _ => rampageRise.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.RampageRiseWait, MaehwaStateId.RampageBeforeNoticeWait, _ => rampageRiseWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.RampageBeforeNoticeWait, MaehwaStateId.RampageNotice, _ => rampageBeforeNoticeWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.RampageNotice, MaehwaStateId.DownAirWait, _ => true);

            // === Down ===
            var downBlink = new EmptyState<MaehwaStateId>(MaehwaStateId.DownBlink);
            var downAirWait = new WaitState<MaehwaStateId>(MaehwaStateId.DownAirWait, s.downAirWaitTime, true);
            var downGetAccel = new MoveByVelocity<MaehwaStateId>(MaehwaStateId.DownGetAccel, Vector2.down, s.downAccel, s.downAccelTime, 0f);
            var downSmashWait = new WaitState<MaehwaStateId>(MaehwaStateId.DownSmashWait, s.downAfterSmashTime, false);
            var downSmashRampageWait = new WaitState<MaehwaStateId>(MaehwaStateId.DownSmashRampageWait, s.rampageStaggerTime, false);

            var downStart = new EmptyState<MaehwaStateId>(MaehwaStateId.DownStart);
            StateMachine.AddState(MaehwaStateId.DownStart, downStart);
            StateMachine.AddState(MaehwaStateId.DownBlink, downBlink);
            StateMachine.AddState(MaehwaStateId.DownAirWait, downAirWait);
            StateMachine.AddState(MaehwaStateId.DownGetAccel, downGetAccel);
            StateMachine.AddState(MaehwaStateId.DownSmashWait, downSmashWait);
            StateMachine.AddState(MaehwaStateId.DownSmashRampageWait, downSmashRampageWait);

            StateMachine.AddTransition(MaehwaStateId.DownStart, MaehwaStateId.DownBlink, _ => true);
            StateMachine.AddTransition(MaehwaStateId.DownBlink, MaehwaStateId.DownAirWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.DownAirWait, MaehwaStateId.DownGetAccel, _ => downAirWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.DownGetAccel, MaehwaStateId.DownSmashWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.DownSmashWait, MaehwaStateId.EndPhase, _ => true);
            StateMachine.AddTransition(MaehwaStateId.DownSmashRampageWait, MaehwaStateId.EndPhase, _ => true);

            // Select-Attack 에서 패턴 분기
            StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.ComboStart, _ => true);
            StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.BodyStart, _ => true);
            StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.HorizonStart, _ => true);
            StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.RampageStart, _ => true);
            StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.DownStart, _ => true);
        }
    }
}
