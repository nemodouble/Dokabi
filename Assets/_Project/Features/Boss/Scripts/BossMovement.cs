using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossMovement : MonoBehaviour
    {
        private Rigidbody2D rigid2D;
        private BoxCollider2D boxCollider;
        
        private float originalGravityScale;

        public void Initialize()
        {
            if (rigid2D == null)
                rigid2D = GetComponent<Rigidbody2D>();
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider2D>();
            originalGravityScale = rigid2D.gravityScale;
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
            SetVelocity(new Vector2(vx, rigid2D.velocity.y));
        }
        
        public void SetVelocityY(float vy)
        {
            SetVelocity(new Vector2(rigid2D.velocity.x, vy));
        }

        // 현재 보스가 플랫폼 위에 서 있는지 판별
        public bool IsOnPlatform(float checkDistance = 0.1f)
        {
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider2D>();

            LayerMask platformLayer = LayerMask.GetMask("Platform");

            // 콜라이더의 중심에서 약간 아래쪽으로 이동한 위치를 기준으로 박스캐스트
            Vector2 origin = (Vector2)transform.position + Vector2.down * (boxCollider.size.y * 0.5f - 0.01f);
            Vector2 size = new Vector2(boxCollider.size.x * 0.9f, 0.05f); // 살짝 줄여서 가장자리에서의 오탐을 줄임

            var hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, checkDistance, platformLayer);
            return hit.collider != null;
        }

        public void SetGravityEnabled(bool b)
        {
            if (b)
            {
                rigid2D.gravityScale = originalGravityScale;
            }
            else
            {
                originalGravityScale = rigid2D.gravityScale;
                rigid2D.gravityScale = 0f;
            }
        }
    }
}