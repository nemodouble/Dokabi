using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boss.Phase;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boss
{
    public abstract class BossController : MonoBehaviour, IHitAble
    {
        internal Rigidbody2D rigid2D;
        internal Animator animator;
        private BoxCollider2D boxCollider;
        
        protected BossPhase DeadPhase;
        private bool isDead;
        private Dictionary<string, int> phaseSelectCount = new Dictionary<string, int>();
        private int phaseCountSum = 10;
        protected string PrevPhaseStack = "";
        
        protected GameObject Player;


        [SerializeField] private string nowPhaseName;
        
        [SerializeField]
        internal int health;
        protected virtual void Start()
        {
            rigid2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            Player = GameObject.Find("Player");
            SetDeadState();
            
            StartCoroutine(Action());
        }

        protected IEnumerator Action()
        {
            while (!isDead)
            {
                var ableStateList = GetAblePhaseList();
                var nowPhase = SelectPhase(ableStateList);
                nowPhaseName = nowPhase.PhaseName;
                PrevPhaseStack = nowPhase.PhaseName;
                yield return nowPhase.DoPhase(this);
            }
        }

        public IEnumerator Attacked(int damage, Vector2 attackDir, float attackForceScale)
        {
            health -= damage;
            if (health <= 0)
            {
                isDead = true;
                yield return DeadPhase.DoPhase(this);
            }
        }

        internal RaycastHit2D IsHeading(Vector2 moveDir)
        {
            var length = Mathf.Sqrt(moveDir.x * moveDir.x + moveDir.y * moveDir.y);
            return IsHeading(moveDir, length);
        }
        internal RaycastHit2D IsHeading(Vector2 moveDir, float distance)
        {
            LayerMask platformLayer = LayerMask.GetMask("Platform");
            return Physics2D.BoxCast(transform.position, boxCollider.size, 0f, moveDir, distance, platformLayer);
        }
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            // if (collision.collider.CompareTag("Player"))
            // {
            //     var knockBackDir = new Vector2((int)GetBossToPlayerDir().x, 0);
            //     StartCoroutine(collision.collider.GetComponent<PlayerController>().Attacked(1, knockBackDir));
            // }
        }

        protected Vector2 GetBossToPlayerDir()
        {
            return PlayerController.GetPosToPlayerDir(transform.position);
        }

        internal void CallInstantiate(GameObject instantiateGameObject, Vector3 relativePos)
        {
            var summonPos = transform.position + (Vector3)relativePos;
            Instantiate(instantiateGameObject, summonPos, Quaternion.identity);
        }

        private BossPhase SelectPhase(IReadOnlyList<BossPhase> phases)
        {
            var phaseSelectRange = new int[phases.Count];
            for(var i =0; i< phases.Count; i++)
            {
                if (!phaseSelectCount.TryGetValue(phases[i].PhaseName, out phaseSelectRange[i]))
                    phaseSelectCount.Add(phases[i].PhaseName, 0);
                phaseSelectRange[i] = (phaseCountSum - 2 * phaseSelectRange[i]) / phases[i].Rarity;
            }
            var randVal = Random.Range(0, phaseSelectRange.Sum());
            var sum = 0;
            for (var i = 0; i < phaseSelectRange.Length; i++)
            {
                sum += phaseSelectRange[i];
                if (randVal < sum)
                {
                    phaseCountSum += 1;
                    phaseSelectCount[phases[i].PhaseName] += 1;
                    return phases[i];
                }
            }
            Debug.Log("패턴 선택 오류");
            return phases[0];
        }
        protected abstract List<BossPhase> GetAblePhaseList();
        protected abstract void SetDeadState();

    }
}
