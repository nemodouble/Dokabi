using System.Collections;

namespace Enemy.DeadLogic.EnemyDeadState
{
    public abstract class EnemyDeadState
    {
        public abstract IEnumerator Dead(EnemyController enemyController);
    }
}