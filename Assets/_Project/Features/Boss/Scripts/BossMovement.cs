using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossMovement : MonoBehaviour
    {
        private Rigidbody2D rigid2D;
        private BoxCollider2D boxCollider;

        public void Initialize()
        {
            if (rigid2D == null)
                rigid2D = GetComponent<Rigidbody2D>();
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider2D>();
        }

        internal RaycastHit2D IsHeading(Vector2 moveDir)
        {
            var length = Mathf.Sqrt(moveDir.x * moveDir.x + moveDir.y * moveDir.y);
            return IsHeading(moveDir, length);
        }
        internal RaycastHit2D IsHeading(Vector2 moveDir, float distance)
        {
            LayerMask platformLayer = LayerMask.GetMask("Platform");
            return Physics2D.BoxCast(transform.position, boxCollider.size, 0f, moveDir, distance, platformLayer);
        }

        public void SetVelocity(Vector2 moveDir)
        {
            rigid2D.velocity = moveDir;
        }
        
        public void SetVelocityX(float vx)
        {
            rigid2D.velocity = new Vector2(vx, rigid2D.velocity.y);
        }
        
        public void SetVelocityY(float vy)
        {
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, vy);
        }
    }
}