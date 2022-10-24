using System;
using FMODUnity;
using UnityEngine;

namespace Boss.MaeHwa
{
    public class RampageRange : MonoBehaviour
    {
        [SerializeField] private float attackSize = 0.1f;
        [SerializeField] private float noticeTime = 0.2f;
        [SerializeField] private float attackTime = 0.2f;
        
        private float m_DestroyTime;
        public float destroyTimeNow = 0;

        private BoxCollider2D m_Collider2D;
        
        public EventReference horizonAttackEvent;
        public EventReference yell3;

        private void Start()
        {
            m_Collider2D = GetComponent<BoxCollider2D>();
        }

        private void OnEnable()
        {
            destroyTimeNow = 0;
            if (gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                m_DestroyTime = noticeTime;
                GetComponent<ParticleSystem>().Play();
                gameObject.layer = LayerMask.NameToLayer("ParticleBlocker");
            }
            else
                Destroy(gameObject);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void SetDanger()
        {
            m_DestroyTime = attackTime;
            m_Collider2D.isTrigger = true;
            m_Collider2D.size = new Vector2(m_Collider2D.size.x, attackSize);
            gameObject.layer = LayerMask.NameToLayer("Danger");
            RuntimeManager.PlayOneShot(horizonAttackEvent);
            RuntimeManager.PlayOneShot(yell3);
        }
        void Update()
        {
            if (destroyTimeNow < m_DestroyTime)
            {
                destroyTimeNow += Time.deltaTime;
            }
            else
            {
                if (gameObject.layer == LayerMask.NameToLayer("ParticleBlocker"))
                {
                    destroyTimeNow = 0;
                    gameObject.layer = LayerMask.NameToLayer("Default");
                }
                else if (gameObject.layer == LayerMask.NameToLayer("Danger"))
                    Destroy(gameObject);
            }
        }

        public void SetDestroyTime(float destroyTime)
        {
            this.m_DestroyTime = destroyTime;
        }
    }
}
