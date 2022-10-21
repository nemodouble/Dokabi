using System.Collections;
using System.Collections.Generic;
using Enemy.AttackLogic;
using Enemy.AttackLogic.AttackState;
using Enemy.DeadLogic;
using Enemy.DeadLogic.EnemyDeadState;
using Enemy.IdleLogic;
using Enemy.StateCheckLogic;
using Enemy.StateCheckLogic.DetectLogic;
using UnityEngine;

namespace Enemy
{
    public class TmpFlyEnemyController : EnemyController
    {
        [Header("IDLE")]
        [SerializeField] protected Vector2 idleMoveableBox;
        [SerializeField] protected float idleFlySpeed;
        [SerializeField] protected float dirChangeTime;
        [SerializeField] protected bool isReturnPos;
        
        [Header("detect")]
        [SerializeField] protected float undetectTime;
        [SerializeField] protected float lookingDistance;
        
        [Header("trace")]
        [SerializeField] protected float amplitude;
        [SerializeField] protected float waveLength;
        [SerializeField] protected Vector2 traceTargetPos;
        [SerializeField] protected float traceSpeed;
        
        protected override void SetLogic()
        {
            idleLogic = new FlyIdleLogic(idleFlySpeed, transform.position, idleMoveableBox, dirChangeTime, isReturnPos);
            traceLogic = new FlyTraceLogic(traceSpeed, amplitude, waveLength, traceTargetPos);
            detectLogic = new CircleDetectLogic(lookingDistance);
            attackLogic = new EnemyAttackLogic(new List<EnemyAttackState>());
            stateSelectLogic = new EnemyStateSelectLogic(detectLogic, attackLogic, undetectTime);
            var deadStates = new Queue<EnemyDeadState>();
            deadStates.Enqueue(new BecomeDummyState());
            deadLogic = new EnemyDeadLogic(deadStates);
        }

        public override IEnumerator Attacked(int attackDamage, Vector2 knockbackDir, float attackForceScale)
        {
            spriteRenderer.color = Color.gray;
            yield return StartCoroutine(base.Attacked(attackDamage, knockbackDir, attackForceScale));
            yield return new WaitForSeconds(0.2f);
            if(!isDead)
                spriteRenderer.color = Color.white;
        }
        
    }
}
