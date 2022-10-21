using System.Collections.Generic;
using Boss.Phase;
using Boss.Phase.Moving;
using UnityEngine;
using UnityEngine.UIElements;

namespace Boss.MaeHwa
{
    public class MaeHwa : BossController
    {
        
        private const int RightDir = 1;
        private int lookingDir = RightDir;
        
        private readonly BossPhase startingWait = new WaitPhase("startingWait", 3f, false);
        private BossPhase endPhase = new EmptyPhase("EndPhase");
        
        private readonly BossPhase rightStep = new BackStep("Step", Vector2.right * 3f, 20f, 10, 0.3f);
        private readonly BossPhase leftStep = new BackStep("Step", Vector2.left * 3f, 20f, 10, 0.3f);
        
        // horizon attack
        private readonly BossPhase horizonAttackStart = new EmptyPhase("Horizon-Start");

        [SerializeField] private float horizonBeforeWaitTime = 1f;
        [SerializeField] private float horizonAfterWaitTime = 1f;
        [SerializeField] private GameObject horizonAttackRange;
        private Vector3 leftEdgePos;
        private Vector3 rightEdgePos;
        
        private BossPhase selectNextPhase = new EmptyPhase("selectNextPhase");

        protected override void Start()
        {
            var parent = gameObject.transform.parent;
            leftEdgePos = parent.transform.Find("LeftEdge").transform.position;
            rightEdgePos = parent.transform.Find("RightEdge").transform.position;
            base.Start();
        }
        protected override List<BossPhase> GetAblePhaseList()
        {
            var ablePhaseList = new List<BossPhase>();
            if (PrevPhaseStack == "")
            {
                ablePhaseList.Add(startingWait);
            }
            else switch (PrevPhaseStack)
            {
                case "startingWait":
                case "EndPhase":
                    ablePhaseList.Add(horizonAttackStart);
                    break;
                
                // 가로베기
                case "Horizon-Start":
                    var playerPos = Player.transform.position;
                    var bossPos = transform.position;
                    var toPlayerLength = ((Vector2)(playerPos - bossPos)).magnitude;
                        ablePhaseList.Add(playerPos.x > bossPos.x
                            ? new BackStep("Horizon-Step", leftEdgePos - transform.position, 20f, 10, 0.1f)
                            : new BackStep("Horizon-Step", rightEdgePos - transform.position, 20f, 10, 0.1f));
                    break;
                case "Horizon-Step":
                    ablePhaseList.Add(new WaitPhase("Horizon-BeforeWait", horizonBeforeWaitTime,true));
                    break;
                case "Horizon-BeforeWait":
                    ablePhaseList.Add(new AttackFixedRange("Horizon-Attack",horizonAttackRange));
                    break;
                case "Horizon-Attack":
                    ablePhaseList.Add(new WaitPhase("Horizon-AfterWait", horizonAfterWaitTime,true));
                    break;
                case "Horizon-AfterWait":
                    //ablePhaseList.Add(new BackStep("EndPhase", Vector2.left * 3f, 20f, 10, 0.3f));
                    ablePhaseList.Add(endPhase);
                    break;
                        
                
                case "Right BackStep":
                    ablePhaseList.Add(startingWait);
                    break;
                case "Left BackStep":
                    ablePhaseList.Add(startingWait);
                    break;
                default:
                    Debug.Log("상태 지정 안됨");
                    ablePhaseList.Add(startingWait);
                    break;
            }

            return ablePhaseList;
        }

        protected override void SetDeadState()
        {
            DeadPhase = new DeadInstant("dead");
        }
    }
}