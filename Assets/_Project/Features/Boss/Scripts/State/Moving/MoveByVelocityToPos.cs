using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class MoveByVelocityToPos<TStateId> : MoveByVelocity<TStateId>
    {
        public MoveByVelocityToPos(
            TStateId id,
            Vector2 dir,
            float velocity,
            float timeMax,
            Vector2 targetPos,
            float length = 0)
            : base(id, dir, velocity, timeMax, length)
        {
            this.targetPos = targetPos;
            haveTargetPos = true;
            Debug.DrawRay(targetPos, Vector2.up, Color.blue, 3f);
        }
    }
}