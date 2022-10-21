using System.Collections;
using System.Collections.Generic;

namespace Enemy.DeadLogic
{
    public class EnemyDeadLogic
    {
        private readonly Queue<EnemyDeadState.EnemyDeadState> deadStates;

        public EnemyDeadLogic(Queue<EnemyDeadState.EnemyDeadState> queue)
        {
            deadStates = queue;
        }

        public IEnumerator Dead(EnemyController enemyController)
        {
            foreach (var deadState in deadStates)
            {
                yield return deadState.Dead(enemyController);
            }
        }
    }
}