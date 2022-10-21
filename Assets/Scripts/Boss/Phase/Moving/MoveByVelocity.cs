using System.Collections;

namespace Boss.Phase
{
    public class MoveByVelocity : BossPhase
    {
        public MoveByVelocity(string phaseName) : base(phaseName)
        {
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            throw new System.NotImplementedException();
        }
    }
}