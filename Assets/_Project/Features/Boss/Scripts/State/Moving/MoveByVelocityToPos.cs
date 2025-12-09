using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class MoveByVelocityToPos : MoveByVelocity
    {
        public MoveByVelocityToPos(
            string id,
            Vector2 dir,
            float velocity,
            float timeMax,
            Vector2 targetPos,
            float length = 0,
            string enterAnimTrigger = null)
            : base(id, dir, velocity, timeMax, length, enterAnimTrigger)
        {
            this.targetPos = targetPos;
            haveTargetPos = true;
            Debug.DrawRay((Vector3)targetPos, Vector2.up, Color.blue, 3f);
        }
    }
}