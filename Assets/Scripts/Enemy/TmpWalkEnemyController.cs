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
    public class TmpWalkEnemyController : EnemyController
    {
        [Header("Idle")]
        [SerializeField] private float movingSpeed;
        [SerializeField] private float maxMovingTime;
        [SerializeField] private float maxStopTime;
        [SerializeField] private float movingLength;
        [SerializeField] private bool isRandomDirChange;
        [SerializeField] private float randomRange;
        
        protected override void SetLogic()
        {
            idleLogic = new WalkIdleLogic(movingSpeed, maxMovingTime, maxStopTime, movingLength, randomRange,isRandomDirChange);
            detectLogic = new CircleDetectLogic(0);
            attackLogic = new EnemyAttackLogic(new List<EnemyAttackState>());
            stateSelectLogic = new EnemyStateSelectLogic(detectLogic, attackLogic, 0);
            var deadStates = new Queue<EnemyDeadState>();
            deadStates.Enqueue(new BecomeDummyState());
            deadLogic = new EnemyDeadLogic(deadStates);
        }

        
    }
}