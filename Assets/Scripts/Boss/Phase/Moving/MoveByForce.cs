using System.Collections;

namespace Boss.Phase.Moving
{
    public class MoveByForce :BossPhase
    {
        public MoveByForce(string phaseName) : base(phaseName)
        {
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            throw new System.NotImplementedException();
        }
    }
}