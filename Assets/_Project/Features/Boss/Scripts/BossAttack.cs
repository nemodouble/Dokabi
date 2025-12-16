using _Project.Features.Battle.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Boss.Scripts
{
    public class BossAttack<TStateId> : MonoBehaviour
    {
        [SerializeField] private BossContext<TStateId> context; 

        public void Initialize()
        {
            context = GetComponent<BossContext<TStateId>>();
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
