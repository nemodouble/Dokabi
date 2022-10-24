using System.Collections;
using System.Linq;
using UnityEngine;

namespace Character.Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        private PlayerController m_PlayerController;
        private PlayerAnimationController m_AnimationController;
        
        private Transform m_Up;
        private Transform m_Down;
        private Transform m_Left;
        private Transform m_Right;
        
        private const float AtkCool = 0.3f;
        private float lastAtkTime;
        private const float MeleeComboLimitTime = 1f;
        private const float AtkDurationMax = 0.2f;

        private int meleeDamage;

        public enum AttackState
        {
            None,
            MeleeUp,
            MeleeDown,
            MeleeLeft,
            MeleeRight
        }

        public AttackState nowAttackState   
        {
            get;
            private set;
        }
        private Coroutine m_NowCoroutine;

        public GameObject tmpEffect;

        private void Start()
        {
            var parent = transform.parent;
            m_PlayerController = parent.GetComponent<PlayerController>();
            m_AnimationController = parent.Find("AnimationController").GetComponent<PlayerAnimationController>();
            m_Up = transform.Find("MeleeUp");
            m_Down = transform.Find("MeleeDown");
            m_Left = transform.Find("MeleeLeft");
            m_Right = transform.Find("MeleeRight");
        }
        
        // 공격 시도
        public void TryMeleeAttack(Vector2 xyDir)
        {
            if (nowAttackState != AttackState.None || Time.time - lastAtkTime < AtkCool) return;
            
            DoMeleeAttack(xyDir);
        }
        
        // 공격 실행
        private void DoMeleeAttack(Vector2 xyDir)
        {
            SetComboAttackAnimation();
            
            // 공격 방향 판단
            Vector2 attackPos;
            Vector2 attackBox;
            Vector2 attackDir;

            if (xyDir.y >= 0.5)
            {
                attackPos = m_Up.position;
                attackBox = m_Up.GetComponent<BoxCollider2D>().size;
                attackDir = Vector2.up;
                nowAttackState = AttackState.MeleeUp;
            }
            else if (xyDir.y <= -0.5 && m_PlayerController.PlayerPlatformStatus != PlayerController.PlatformStatus.Flat)
            {
                attackPos = m_Down.position;
                attackBox = m_Down.GetComponent<BoxCollider2D>().size;
                attackDir = Vector2.down;
                nowAttackState = AttackState.MeleeDown;
            }
            else if(xyDir.x > 0 || xyDir.x == 0 && m_PlayerController.LookingDir == 1)
            {
                attackPos = m_Right.position;
                attackBox = m_Right.GetComponent<BoxCollider2D>().size;
                attackDir = Vector2.right;
                nowAttackState = AttackState.MeleeRight;
            }
            else if(xyDir.x < 0 || xyDir.x == 0 && m_PlayerController.LookingDir == -1)
            {
                attackPos = m_Left.position;
                attackBox = m_Left.GetComponent<BoxCollider2D>().size;
                attackDir = Vector2.left;
                nowAttackState = AttackState.MeleeLeft;
            }
            else
            {
                Debug.LogError("잘못된 공격 방향");
                attackPos = m_Right.position;
                attackBox = m_Right.GetComponent<BoxCollider2D>().size;
                attackDir = Vector2.right;
                nowAttackState = AttackState.MeleeRight;
            }

            m_NowCoroutine = StartCoroutine(MeleeAttackCoroutine(attackPos, attackBox, attackDir));
        }
        
        // 콤보 공격 애니메이션 설정
        private void SetComboAttackAnimation()
        {
            if (Time.time - lastAtkTime < MeleeComboLimitTime)
            {
                m_AnimationController.onComboAttackTime = !m_AnimationController.onComboAttackTime;
            }
            else
            {
                m_AnimationController.onComboAttackTime = false;
            }
            lastAtkTime = Time.time;
        }

        public void AttackCancel()
        {
            StopCoroutine(m_NowCoroutine);
        }
        
        private IEnumerator MeleeAttackCoroutine(Vector2 atkPos, Vector2 atkBox, Vector2 atkDir)
        {
            // 이미 피격된 콜라이더 저장
            var attacked = new Collider2D[10];
            var atkHead = 0;
            
            // 공격 판정 시간 지정
            lastAtkTime = Time.time;
            var atkDurationNow = 0f;
            while(atkDurationNow < AtkDurationMax)
            {
                // 공격 범위 스캔
                var scanned = new Collider2D[10];
                var scanningLayer = LayerMask.GetMask("Enemy") + LayerMask.GetMask("Platform");
                var scannedCount = Physics2D.OverlapBoxNonAlloc( atkPos, atkBox, 0, scanned, scanningLayer);
                
                // 가로막는 플랫폼 확인
                var hit = Physics2D.Raycast(transform.position, atkDir, atkBox.x > atkBox.y ? atkBox.x : atkBox.y,
                    scanningLayer);
                if (hit.collider != null)
                {
                    // Instantiate(tmpEffect, hit.point, Quaternion.identity);
                }
                
                var enemyHit = false;
                for (var i = 0; i < scannedCount; i++)
                {
                    // 이미 피격 여부 확인
                    if(attacked.Contains(scanned[i])) continue;
                    attacked[atkHead++] = scanned[i];
                    
                    if (scanned[i].GetComponent<IHitAble>() != null)
                    {
                        StartCoroutine(scanned[i].GetComponent<IHitAble>().Hit(meleeDamage, atkDir));
                        enemyHit = true;
                    }
                }
                
                // 다운어택 바운스 
                if (enemyHit && atkDir == Vector2.down)
                {
                    //StartCoroutine(DownAttackBounce(downAttackBounceForce, 0.05f));
                }

                atkDurationNow += Time.deltaTime;
                yield return null;
            }

            
            nowAttackState = AttackState.None;
        }
    }
}