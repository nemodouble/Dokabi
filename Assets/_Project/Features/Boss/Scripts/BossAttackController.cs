using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossAttackController : MonoBehaviour
    {
        private BossController boss;
        private GameObject dangerRange;

        public void Initialize()
        {
            boss.Context.OnDead += OnDead;
        }

        private void OnDead()
        {
            dangerRange.SetActive(false);
        }

        internal void CallInstantiate(GameObject instantiateGameObject, Vector3 relativePos)
        {
            var summonPos = transform.position + (Vector3)relativePos;
            Instantiate(instantiateGameObject, summonPos, Quaternion.identity);
        }
    }
}