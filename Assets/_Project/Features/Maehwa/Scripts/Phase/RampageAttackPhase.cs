using System.Collections.Generic;
using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using Boss.MaeHwa;
using UnityEngine;
using Util;

namespace _Project.Features.Maehwa.Scripts.Phase
{
    public class RampageAttackState : BossState<MaehwaStateId, MaehwaContext>
    {
        private readonly float noticeWaitTime;
        private readonly float attackBeforeWaitTime;
        private readonly float attackTime;
        private readonly float attackAfterWaitTime;

        // 서브 상태 정의
        private enum SubState
        {
            None,
            PreparePositions,
            Warning,        // 범위 하나씩 생성
            PreAttackWait,  // 공격 전 정지
            Attack,         // SetDanger 적용
            PostAttackWait, // 공격 후 정지
            Finished
        }

        private SubState _subState = SubState.None;
        private float _stateTimer;

        private readonly List<Vector2> _posList = new();
        private readonly List<Vector3> _rotList = new();
        private readonly List<MaeHwaRampageRange> _rangeList = new();

        private BossController<MaehwaStateId, BossContext<MaehwaStateId>> _maeHwaController;
        private Transform _playerTransform;

        private int _spawnedWarningCount;

        public RampageAttackState(MaehwaStateId id, float noticeWaitTime, float attackBeforeWaitTime, float attackTime, float attackAfterWaitTime) : base(id)
        {
            this.noticeWaitTime = noticeWaitTime;
            this.attackBeforeWaitTime = attackBeforeWaitTime;
            this.attackTime = attackTime;
            this.attackAfterWaitTime = attackAfterWaitTime;
        }

        public override void OnEnter(MaehwaContext ctx)
        {
            _maeHwaController = ctx.Controller;
            if (_maeHwaController == null)
            {
                Debug.LogError("RampageAttackState: MaeHwaController 캐스팅 실패");
                _subState = SubState.Finished;
                return;
            }

            var playerObj = ctx.PlayerTransform;
            if (playerObj == null)
            {
                Debug.LogError("RampageAttackState: Player 오브젝트를 찾지 못했습니다.");
                _subState = SubState.Finished;
                return;
            }
            _playerTransform = playerObj.transform;

            _posList.Clear();
            _rotList.Clear();
            _rangeList.Clear();
            _spawnedWarningCount = 0;
            _stateTimer = 0f;

            _subState = SubState.PreparePositions;
        }

        public override void OnExit(MaehwaContext ctx)
        {
            // 여기서는 별도 정리 작업이 필요 없지만, 나중에 이펙트/코루틴 정리 등이 필요하면 추가.
        }

        public override void Tick(MaehwaContext ctx, float deltaTime)
        {
            if (_subState == SubState.Finished)
                return;

            _stateTimer += deltaTime;

            switch (_subState)
            {
                case SubState.PreparePositions:
                    TickPreparePositions(ctx);
                    break;
                case SubState.Warning:
                    TickWarning(ctx);
                    break;
                case SubState.PreAttackWait:
                    if (_stateTimer >= attackBeforeWaitTime)
                    {
                        EnterAttack();
                    }
                    break;
                case SubState.Attack:
                    // Attack 단계는 즉시 다음 상태로 넘어가므로 여기선 할 일 없음
                    break;
                case SubState.PostAttackWait:
                    if (_stateTimer >= attackAfterWaitTime)
                    {
                        _subState = SubState.Finished;
                        // 필요하다면 여기서 다음 상태로 전환하는 이벤트를 날릴 수 있음.
                    }
                    break;
            }
        }

        public override void FixedTick(MaehwaContext ctx, float deltaTime)
        {
            // 물리 기반 행동이 없으므로 비워둠
        }

        public override void HandleEvent(MaehwaContext ctx, object evt)
        {
            // 현재 패턴에서는 별도 이벤트 처리 없음
        }

        private void TickPreparePositions(MaehwaContext ctx)
        {
            if (_playerTransform == null)
            {
                _subState = SubState.Finished;
                return;
            }

            var playerPos = (Vector2)_playerTransform.position;

            float firstAngleDeg;
            RaycastHit2D raycastHit2D;
            // 장애물 없는 첫 번째 방향 찾기 (3f 거리)
            int safety = 0;
            do
            {
                InfiniteLoopDetector.Run();
                firstAngleDeg = Random.Range(0f, 360f);
                var dir = new Vector2(Mathf.Cos(firstAngleDeg * Mathf.Deg2Rad), Mathf.Sin(firstAngleDeg * Mathf.Deg2Rad));
                raycastHit2D = Physics2D.Raycast(playerPos, dir, 3f, LayerMask.GetMask("Platform"));
                safety++;
                if (safety > 50)
                {
                    break;
                }
            } while (raycastHit2D.collider != null);

            float secondAngleDeg;
            safety = 0;
            // 장애물 없는 두 번째 방향 찾기 (5f 거리)
            do
            {
                InfiniteLoopDetector.Run();
                secondAngleDeg = Random.Range(0f, 360f);
                var dir = new Vector2(Mathf.Cos(secondAngleDeg * Mathf.Deg2Rad), Mathf.Sin(secondAngleDeg * Mathf.Deg2Rad));
                raycastHit2D = Physics2D.Raycast(playerPos, dir, 5f, LayerMask.GetMask("Platform"));
                safety++;
                if (safety > 50)
                {
                    break;
                }
            } while (raycastHit2D.collider != null);

            _posList.Clear();
            _posList.Add(playerPos);
            _posList.Add(new Vector2(playerPos.x + 3f * Mathf.Cos(firstAngleDeg * Mathf.Deg2Rad), playerPos.y + 3f * Mathf.Sin(firstAngleDeg * Mathf.Deg2Rad)));
            _posList.Add(new Vector2(playerPos.x + 5f * Mathf.Cos(secondAngleDeg * Mathf.Deg2Rad), playerPos.y + 5f * Mathf.Sin(secondAngleDeg * Mathf.Deg2Rad)));

            _rotList.Clear();
            _rotList.Add(new Vector3(0, 0, Random.Range(-10f, 10f)));
            _rotList.Add(new Vector3(0, 0, Random.Range(20f, 40f)));
            _rotList.Add(new Vector3(0, 0, Random.Range(-40f, -20f)));

            _rangeList.Clear();
            _spawnedWarningCount = 0;
            _stateTimer = 0f;
            _subState = SubState.Warning;
        }

        private void TickWarning(MaehwaContext ctx)
        {
            // noticeWaitTime마다 하나씩 범위 생성 (최대 3개)
            if (_spawnedWarningCount >= 3)
            {
                // 모두 생성했으면 공격 전 대기 상태로 전환
                _stateTimer = 0f;
                _subState = SubState.PreAttackWait;
                return;
            }

            if (_stateTimer < noticeWaitTime)
                return;

            _stateTimer = 0f;

            if (_posList.Count == 0 || _rotList.Count == 0)
            {
                // 데이터 이상 시 바로 종료
                _subState = SubState.Finished;
                return;
            }

            var posIndex = Random.Range(0, _posList.Count);
            var pos = _posList[posIndex];
            _posList.RemoveAt(posIndex);

            var rotIndex = Random.Range(0, _rotList.Count);
            var rot = _rotList[rotIndex];
            _rotList.RemoveAt(rotIndex);

            var attackController = ctx.Attack as MaehwaAttack;
            if (attackController != null)
            {
                var range = attackController.InstantiateRampageRange(pos, rot);
                range.SetActive(true);
                _rangeList.Add(range);
            }
            else {
                Debug.LogError("RampageAttackState: MaeHwaAttackController 캐스팅 실패");
            }

            _spawnedWarningCount++;
        }

        private void EnterAttack()
        {
            foreach (var range in _rangeList)
            {
                if (range == null) continue;
                var comp = range.GetComponent<MaeHwaRampageRange>();
                if (comp != null)
                {
                    comp.SetDestroyTime(attackTime);
                }
                range.SetDanger();
                // TO-DO : range 애니메이션 설정
            }

            _stateTimer = 0f;
            _subState = SubState.PostAttackWait;
        }
    }
}