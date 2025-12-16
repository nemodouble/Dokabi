using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using _Project.Features.Boss.Scripts.State.Dead;
using _Project.Features.Boss.Scripts.State.Moving;
using _Project.Features.Maehwa.Scripts.State;
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
            var atk = Context.Attack as MaehwaAttack;
            
            if (atk == null)
            {
                Debug.LogError("MaehwaStateMachine: MaehwaAttack 컴포넌트를 찾지 못했습니다.");
                return;
            }

            // 시작 대기 상태
            var startWait = new WaitState<MaehwaStateId>(MaehwaStateId.StartWait, s.startWaitTime, false);
            StateMachine.AddState(MaehwaStateId.StartWait, startWait);
            StateMachine.SetInitialState(MaehwaStateId.StartWait);

            // 사망 상태
            var deadState = new DeadNormal<MaehwaStateId>(MaehwaStateId.Dead);
            StateMachine.AddState(MaehwaStateId.Dead, deadState);
            StateMachine.AddAnyTransition(MaehwaStateId.Dead, ctx => ctx.IsDead());

            // Select-Pattern
            var selectPattern = new EmptyState<MaehwaStateId>(MaehwaStateId.SelectPattern);
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
            StateMachine.AddTransition(MaehwaStateId.SelectPattern, MaehwaStateId.SelectStep, ctx => ctx.IsInDistance(0f, 2f), 0, 2f);
            StateMachine.AddTransition(MaehwaStateId.SelectPattern, MaehwaStateId.SelectAttack, _ => true);

            // Select-Walk -> WalkLeft / WalkRight
            StateMachine.AddTransition(MaehwaStateId.SelectWalk, MaehwaStateId.WalkRight, ctx => ctx.SelectMoveDir());
            StateMachine.AddTransition(MaehwaStateId.SelectWalk, MaehwaStateId.WalkLeft, ctx => !ctx.SelectMoveDir());

            // Walk 종료 후 Select-Attack
            StateMachine.AddTransition(MaehwaStateId.WalkLeft, MaehwaStateId.SelectAttack, _ => walkLeft.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.WalkRight, MaehwaStateId.SelectAttack, _ => walkRight.IsFinished);

            // Select-Step -> Front / Back
            StateMachine.AddTransition(MaehwaStateId.SelectStep, MaehwaStateId.BackStep, ctx => ctx.IsInDistance(0f, 3f));
            StateMachine.AddTransition(MaehwaStateId.SelectStep, MaehwaStateId.FrontStep, ctx => !ctx.IsInDistance(0f, 3f));

            // Step 종료 후 Select-Attack
            StateMachine.AddTransition(MaehwaStateId.FrontStep, MaehwaStateId.SelectAttack, _ => frontStep.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.BackStep, MaehwaStateId.SelectAttack, _ => backStep.IsFinished);

            // 공격 종료 공통 
            var endPhase = new WaitState<MaehwaStateId>(MaehwaStateId.EndAttack, s.betweenPhaseWaitTime, true);
            StateMachine.AddState(MaehwaStateId.EndAttack, endPhase);
            StateMachine.AddTransition(MaehwaStateId.EndAttack, MaehwaStateId.SelectPattern, _ => endPhase.IsFinished);
            
            // === Horizon / EndPhase ===
            var horizonBeforeWait = new WaitState<MaehwaStateId>(MaehwaStateId.HorizonBeforeWait, s.horizonBeforeWaitTime, true);
            var horizonAttack = new AttackFixedRange<MaehwaStateId>(MaehwaStateId.HorizonAttack, atk.HorizonAttackRange);
            var horizonAfterWait = new WaitState<MaehwaStateId>(MaehwaStateId.HorizonAfterWait, s.horizonAfterWaitTime, true);

            StateMachine.AddState(MaehwaStateId.HorizonBeforeWait, horizonBeforeWait);
            StateMachine.AddState(MaehwaStateId.HorizonAttack, horizonAttack);
            StateMachine.AddState(MaehwaStateId.HorizonAfterWait, horizonAfterWait);

            var horizonStart = new EmptyState<MaehwaStateId>(MaehwaStateId.HorizonStart);
            StateMachine.AddState(MaehwaStateId.HorizonStart, horizonStart);

            // 기존 MoveByVelocityToPos 대신 HorizonStep 사용
            var horizonStep = new HorizonRun(
                MaehwaStateId.HorizonRun,
                s.horizonStepSpeed,
                3f,
                s.horizonTeleportWaitTime);                     // 최대 이동 시간 (기존과 동일)
            StateMachine.AddState(MaehwaStateId.HorizonRun, horizonStep);

            StateMachine.AddTransition(MaehwaStateId.HorizonStart, MaehwaStateId.HorizonRun, _ => true);
            StateMachine.AddTransition(MaehwaStateId.HorizonRun, MaehwaStateId.HorizonBeforeWait, _ => horizonStep.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.HorizonBeforeWait, MaehwaStateId.HorizonAttack, _ => horizonBeforeWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.HorizonAttack, MaehwaStateId.HorizonAfterWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.HorizonAfterWait, MaehwaStateId.EndAttack, _ => horizonAfterWait.IsFinished);

            // === Body ===
            var bodyStart = new EmptyState<MaehwaStateId>(MaehwaStateId.BodyStart);
            var bodyDash = new BodyDash(MaehwaStateId.BodyDash, s.bodyDashSpeed, s.bodyDashTime);
            var bodyAfterDashWait = new WaitState<MaehwaStateId>(MaehwaStateId.BodyAfterDashWait, s.bodyAfterDashWaitTime, false);
            var bodyAttack = new AttackFixedRange<MaehwaStateId>(MaehwaStateId.BodyAttack, atk.BodyStrongAttack);
            var bodyAfterAttackWait = new WaitState<MaehwaStateId>(MaehwaStateId.BodyAfterAttackWait, s.bodyAfterAttackWaitTime, false);

            StateMachine.AddState(MaehwaStateId.BodyStart, bodyStart);
            StateMachine.AddState(MaehwaStateId.BodyDash, bodyDash);
            StateMachine.AddState(MaehwaStateId.BodyAfterDashWait, bodyAfterDashWait);
            StateMachine.AddState(MaehwaStateId.BodyAttack, bodyAttack);
            StateMachine.AddState(MaehwaStateId.BodyAfterAttackWait, bodyAfterAttackWait);

            StateMachine.AddTransition(MaehwaStateId.BodyStart, MaehwaStateId.BodyDash, _ => true);
            StateMachine.AddTransition(MaehwaStateId.BodyDash, MaehwaStateId.BodyAfterDashWait, _ => bodyDash.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.BodyAfterDashWait, MaehwaStateId.BodyAttack, _ => bodyAfterDashWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.BodyAttack, MaehwaStateId.BodyAfterAttackWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.BodyAfterAttackWait, MaehwaStateId.EndAttack, _ => bodyAfterAttackWait.IsFinished);

            // === Combo ===
            var comboStart = new EmptyState<MaehwaStateId>(MaehwaStateId.ComboStart);
            
            var comboFirstBeforeWait = new ComboSelectDash(MaehwaStateId.ComboFirstBeforeWait, s.comboFirstBeforeWaitTime);
            // 통합 콤보 공격 상태: First
            var comboFirstAttack = new ComboAttackState(
                MaehwaStateId.ComboFirstAttackColliderActive,
                Vector2.zero,                // 대시 없음 기본값, 실제 대시는 이후 상태에서 처리하거나 필요시 확장
                s.comboNormalSpeed,
                s.comboNormalLength,
                s.comboNormalLength,
                ComboAttackState.BossAttackRangeType.ComboNormal);
            var comboFirstAfterWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboFirstAfterWait, s.comboAfterFirstWaitTime, false);

            var comboSecondBeforeWait = new ComboSelectDash(MaehwaStateId.ComboSecondBeforeWait, s.comboBeforeSecondWaitTime, false);
            // 통합 콤보 공격 상태: Second
            var comboSecondAttack = new ComboAttackState(
                MaehwaStateId.ComboSecondAttackColliderActive,
                Vector2.zero,
                s.comboNormalSpeed,
                s.comboNormalLength,
                s.comboNormalLength,
                ComboAttackState.BossAttackRangeType.ComboNormal);
            var comboSecondAfterWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboSecondAfterWait, s.comboAfterSecondWaitTime, false);

            var comboThirdBeforeWait = new ComboSelectDash(MaehwaStateId.ComboThirdBeforeWait, s.comboBeforeThirdWaitTime, false, true);
            // 통합 콤보 공격 상태: Third (Sting)
            var comboThirdAttack = new ComboAttackState(
                MaehwaStateId.ComboThirdAttackColliderActive,
                Vector2.zero,
                s.comboStingSpeed,
                s.comboStingTime,
                s.comboStingTime,
                ComboAttackState.BossAttackRangeType.ComboSting);
            var comboThirdAfterWait = new WaitState<MaehwaStateId>(MaehwaStateId.ComboAfterWait, s.comboAfterThirdWaitTime, false);

            StateMachine.AddState(MaehwaStateId.ComboStart, comboStart);
            
            StateMachine.AddState(MaehwaStateId.ComboFirstBeforeWait, comboFirstBeforeWait);
            StateMachine.AddState(MaehwaStateId.ComboFirstAttackColliderActive, comboFirstAttack);
            StateMachine.AddState(MaehwaStateId.ComboFirstAfterWait, comboFirstAfterWait);
            StateMachine.AddState(MaehwaStateId.ComboSecondBeforeWait, comboSecondBeforeWait);
            StateMachine.AddState(MaehwaStateId.ComboSecondAttackColliderActive, comboSecondAttack);
            StateMachine.AddState(MaehwaStateId.ComboSecondAfterWait, comboSecondAfterWait);
            StateMachine.AddState(MaehwaStateId.ComboThirdBeforeWait, comboThirdBeforeWait);
            StateMachine.AddState(MaehwaStateId.ComboThirdAttackColliderActive, comboThirdAttack);
            StateMachine.AddState(MaehwaStateId.ComboAfterWait, comboThirdAfterWait);

            StateMachine.AddTransition(MaehwaStateId.ComboStart, MaehwaStateId.ComboFirstBeforeWait, _ => true);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstBeforeWait, MaehwaStateId.ComboFirstAttackColliderActive, _ => comboFirstBeforeWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstAttackColliderActive, MaehwaStateId.ComboFirstAfterWait, _ => comboFirstAttack.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstAfterWait, MaehwaStateId.ComboSecondBeforeWait, ctx => ctx.IsInDistance(0, s.comboSkipSecondDistance) && comboFirstAfterWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboFirstAfterWait, MaehwaStateId.ComboThirdBeforeWait, ctx => !ctx.IsInDistance(0, s.comboSkipSecondDistance) && comboFirstAfterWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboSecondBeforeWait, MaehwaStateId.ComboSecondAttackColliderActive, _ => comboSecondBeforeWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboSecondAttackColliderActive, MaehwaStateId.ComboSecondAfterWait, _ => comboSecondAttack.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboSecondAfterWait, MaehwaStateId.ComboThirdBeforeWait, _ => comboSecondAfterWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboThirdBeforeWait, MaehwaStateId.ComboThirdAttackColliderActive, _ => comboThirdBeforeWait.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboThirdAttackColliderActive, MaehwaStateId.ComboAfterWait, _ => comboThirdAttack.IsFinished);
            StateMachine.AddTransition(MaehwaStateId.ComboAfterWait, MaehwaStateId.EndAttack, _ => comboThirdAfterWait.IsFinished);

            // === Rampage ===
            var rampageRise = new MoveLikeJump<MaehwaStateId>(MaehwaStateId.RampageRise, s.rampageRiseSpeed, s.rampageRiseTime);
            var rampageRiseWait = new WaitState<MaehwaStateId>(MaehwaStateId.RampageRiseWait, s.rampageRiseWaitTime, true);
            var rampageBeforeNoticeWait = new WaitState<MaehwaStateId>(MaehwaStateId.RampageBeforeNoticeWait, s.rampageBeforeNoticeWaitTime, true);
            var rampageBlink = new Teleport<MaehwaStateId>(MaehwaStateId.DownBlink, true, new Vector2(0, 7f), Context.PlayerTransform);
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
            var downBlink = new Teleport<MaehwaStateId>(MaehwaStateId.DownBlink, true, new Vector2(0, 7f), Context.PlayerTransform);
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
            StateMachine.AddTransition(MaehwaStateId.DownGetAccel, MaehwaStateId.DownSmashWait, ctx => ctx.IsOnPlatform());
            StateMachine.AddTransition(MaehwaStateId.DownSmashWait, MaehwaStateId.EndAttack, _ => true);
            StateMachine.AddTransition(MaehwaStateId.DownSmashRampageWait, MaehwaStateId.EndAttack, _ => true);

            // Select-Attack 에서 패턴 분기
            // StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.ComboStart, _ => true);
            // StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.BodyStart, _ => true);
            StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.HorizonStart, _ => true);
            // StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.RampageStart, _ => true);
            // StateMachine.AddTransition(MaehwaStateId.SelectAttack, MaehwaStateId.DownStart, _ => true);
        }
    }
}
