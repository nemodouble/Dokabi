using System;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossEffect : MonoBehaviour
    {
        [Header("Common Boss Effect References")]
        [SerializeField] protected Component boss; // BossController<TStateId>
        [SerializeField] protected ParticleSystem hitPS;
        [SerializeField] protected ParticleSystem deadPS;
        [SerializeField] protected SpriteFlasher spriteFlasher;

        public void Initialize()
        {
            if (boss == null)
                boss = GetComponent(typeof(BossController<,>));
            if (spriteFlasher == null)
                spriteFlasher = GetComponent<SpriteFlasher>();

            if (boss == null)
                return;

            if (boss.TryGetComponent(out BossContext<object> ctx))
            {
                ctx.OnHit += OnHit;
                ctx.OnDead += OnDead;
            }
        }
        
        public void OnHit(int damage, Vector2 attackDir, float knockbackForce)
        {
            if (hitPS != null)
            {
                var angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
                var hitPSTransform = hitPS.transform;
                hitPSTransform.position = transform.position;
                hitPSTransform.eulerAngles = new Vector3(0, 0, angle);
                hitPS.Play();
            }

            if (spriteFlasher != null)
            {
                spriteFlasher.Flash();
            }
        }

        public void OnDead()
        {
            if (deadPS != null)
            {
                deadPS.Play();
            }
        }

        /// <summary>
        /// 파티클 재생용 공통 헬퍼 (상속에서 사용)
        /// </summary>
        /// <param name="ps"></param>
        protected void PlayParticle(ParticleSystem ps)
        {
            if (ps == null) return;
            ps.Play();
        }

        /// <summary>
        /// 파티클 정지용 공통 헬퍼 (상속에서 사용)
        /// </summary>
        /// <param name="ps"></param>
        protected void StopParticle(ParticleSystem ps)
        {
            if (ps == null) return;
            ps.Stop();
        }
    }
}
