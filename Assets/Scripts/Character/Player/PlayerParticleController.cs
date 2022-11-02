using System;
using System.Collections;
using UnityEngine;

namespace Character.Player
{
    public class PlayerParticleController : MonoBehaviour
    {
        private ParticleSystem walkPS;
        private ParticleSystem flyPS;
        private ParticleSystem dashPS;
        private ParticleSystem jumpPS;

        private void Start()
        {
            walkPS = transform.Find("WalkPS").GetComponent<ParticleSystem>();
            flyPS = transform.Find("FlyPS").GetComponent<ParticleSystem>();
            dashPS = transform.Find("DashPS").GetComponent<ParticleSystem>();
            jumpPS = transform.Find("JumpPS").GetComponent<ParticleSystem>();
        }
        
        public void PlayWalkPS()
        {
            walkPS.Play();
        }
        
        public void StopWalkPS()
        {
            walkPS.Stop();
        }
        
        public void PlayFlyPS()
        {
            flyPS.Play();
        }
        
        public void StopFlyPS()
        {
            flyPS.Stop();
        }
        
        public void PlayDashPS()
        {
            dashPS.Play();
        }
        
        public void StopDashPS()
        {
            dashPS.Stop();
        }

        public void PlayJumpPS()
        {
            jumpPS.Play();
        }
    }
}