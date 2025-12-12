using _Project.Features.Boss.Scripts;
using Boss.MaeHwa;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaehwaContext : BossContext<MaehwaStateId>
    {
        [SerializeField] public MaeHwaStats stats;

        public enum LookingDir
        {
            RightDir = 1,
            LeftDir = -1
        }
        private LookingDir lookingDir = LookingDir.LeftDir;
        
        public void SetToLookPlayer()
        {
            lookingDir = transform.position.x > PlayerTransform.position.x ? LookingDir.LeftDir : LookingDir.RightDir;
        }

        public void SetLookingDir(LookingDir dir)
        {
            lookingDir = dir;
        }

        public bool CanSelectMoveRight()
        {
            return IsInDistance(0f, 4f) ^ (PlayerTransform.position.x > transform.position.x);
        }
        
        public bool IsInDistance(float minDistance, float maxDistance)
        {
            var distance = Mathf.Abs(transform.position.x - PlayerTransform.position.x);
            return distance >= minDistance && distance <= maxDistance;
        }
    }
}