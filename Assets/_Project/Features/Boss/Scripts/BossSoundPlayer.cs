using System;
using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossSoundPlayer : MonoBehaviour
    {
        [SerializeField] public BossController boss;
        
        [ShowInInspector] private Dictionary<string, EventReference> soundEvents = new();
        public EventReference hitEvent;
        public EventReference dead;

        public void Initialize()
        {
            boss.Context.OnHit += OnHit;
            boss.Context.OnDead += OnDead;
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