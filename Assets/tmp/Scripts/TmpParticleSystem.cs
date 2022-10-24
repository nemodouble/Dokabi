using System;
using UnityEngine;

namespace tmp.Scripts
{
    public class TmpParticleSystem : MonoBehaviour
    {
        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles;
        private void Start()
        {
            ps = GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        }

        
    }
}
