using System;
using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossSound : MonoBehaviour
    {
        [SerializeField] public Component boss; // BossController<TStateId>
        
        [ShowInInspector] private Dictionary<string, EventReference> soundEvents = new();
        public EventReference hitEvent;
        public EventReference dead;

        public void Initialize()
        {
            if (boss == null)
                boss = GetComponent(typeof(BossController<,>));

            if (boss != null && boss.TryGetComponent(out BossContext<object> ctx))
            {
                ctx.OnHit += OnHit;
                ctx.OnDead += OnDead;
            }
        }

        public void PlaySound(string soundEventId)
        {
            var soundEvent = soundEvents[soundEventId];
            if (!soundEvent.IsNull)
                RuntimeManager.PlayOneShot(soundEvent);
        }
        private void OnHit(int damage, Vector2 dir, float force)
        {
            if (!hitEvent.IsNull)
            {
                RuntimeManager.PlayOneShot(hitEvent);
            }
        }

        private void OnDead()
        {
            if (!dead.IsNull)
            {
                RuntimeManager.PlayOneShot(dead);
            }
        }
    }
}
