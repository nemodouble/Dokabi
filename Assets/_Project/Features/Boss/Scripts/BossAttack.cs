using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossAttack : MonoBehaviour
    {
        private Component boss; // BossController<TStateId> 대응

        public void Initialize()
        {
            if (boss == null)
                boss = GetComponent(typeof(BossController<,>));

            if (boss != null && boss.TryGetComponent(out BossContext<object> ctx))
            {
                ctx.OnDead += OnDead;
            }
        }

        private void OnDead()
        {
        }

        internal void CallInstantiate(GameObject instantiateGameObject, Vector3 relativePos)
        {
            var summonPos = transform.position + (Vector3)relativePos;
            Instantiate(instantiateGameObject, summonPos, Quaternion.identity);
        }
    }
}
