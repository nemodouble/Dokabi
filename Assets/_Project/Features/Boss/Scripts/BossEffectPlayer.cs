using System;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossEffectPlayer : MonoBehaviour
    {
        public BossController boss;
        
        public ParticleSystem hitPS;
        public ParticleSystem deadPS;
        public SpriteFlasher spriteFlasher;

        public void Initialize()
        {
            if(boss == null)
                boss = GetComponent<BossController>();
            boss.Context.OnHit += OnHit;
            boss.Context.OnDead += OnDead;
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

            spriteFlasher.Flash();
        }

        public void OnDead()
        {
            if (deadPS != null)
            {
                deadPS.Play();
            }
        }
    }
}