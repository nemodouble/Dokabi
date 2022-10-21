using System.Collections;
using Enemy.StateCheckLogic.DetectLogic;
using Player;
using UnityEngine;

namespace Enemy.AttackLogic.AttackState
{
    public class RangeAttackState : EnemyAttackState
    {
        private readonly Vector2 attackPos;
        private readonly Vector2 attackSize;

        public RangeAttackState(EnemyDetectLogic attackDetectLogic, Vector2 attackPos, Vector2 attackSize) : base(attackDetectLogic)
        {
            this.attackPos = attackPos;
            this.attackSize = attackSize;
        }
        public override IEnumerator Attack(EnemyController enemyController)
        {
            var attackDir =  PlayerController.GetPosToPlayerDir(enemyController.transform.position);
            yield return AttackType.RangeAttack(attackPos, attackSize, attackDir);
        }
    }
}