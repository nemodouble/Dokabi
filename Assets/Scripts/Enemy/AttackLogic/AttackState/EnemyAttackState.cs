using System.Collections;
using Enemy.StateCheckLogic.DetectLogic;

namespace Enemy.AttackLogic.AttackState
{
    public abstract class EnemyAttackState
    {
        protected readonly EnemyDetectLogic attackDetectLogic;

        public EnemyAttackState(EnemyDetectLogic attackDetectLogic)
        {
            this.attackDetectLogic = attackDetectLogic;
        }
        public abstract IEnumerator Attack(EnemyController enemyController);

        public virtual bool CanAttack(EnemyController enemyController)
        {
            return attackDetectLogic.CanDetect(enemyController);
        }
    }
}