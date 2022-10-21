using System;
using System.Collections;
using System.Linq;
using Boss;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Player
{
    public class PlayerController : MonoBehaviour , IHitAble
    {
        private Rigidbody2D rigid2D;
        private SpriteRenderer spriteRenderer;
        private CapsuleCollider2D capsuleCollider2D;
        private BoxCollider2D boxCollider2D;
        private CircleCollider2D circleCollider2D;
        private Animator animator;
        [SerializeField]
        private GameObject sideAttackObj;
        [SerializeField]
        private GameObject upAttackObj;
        [SerializeField]
        private GameObject downAttackObj;
        private GameObject frontSlopeChecker;
        private GameObject backSlopeChecker;
        private Animator dashImpactAnimator;
        private ParticleSystem dustParticleSys;
        private ParticleSystem flyParticleSys;

        private PlayerMovementLogic playerMovementLogic;
        private PlayerAnimationLogic playerAnimationLogic;

        // 애니메이션 트리거
        private readonly string[] triggerList = { "Idle Trigger", "Fall Trigger", "Run Trigger", "Jump Trigger", "Land Trigger", "AtkSide1 Trigger",
            "AtkSide2 Trigger", "AtkUp Trigger", "AtkDown Trigger", "Flying Trigger" , "Dash Trigger", "Hurt Trigger" };
        
        // 타격 가능 태그
        private static readonly string[] HitAbleTagList = {"Enemy", "Boss"};
        
        // 액션
        private const int NotInAction = 0;
        private const int BeforeDelay = 1;
        private const int InAction = 2;
        private const int AfterDelay = 3;

        // 물리 판단, 판정
        private bool isOnPlatform;
        private Vector2 frontSlopeNormal;
        private Vector2 backSlopeNormal;
        private bool isOnMaxSlope;
        private float slopDifference;
        private bool isEndOfMaxSlope;
        private bool isNearWall;

        [Header("이동 조건")]
        [SerializeField] private bool unlockFly;
        [SerializeField] private bool unlockDash;
        [SerializeField] private bool unlockWallJump;
        [SerializeField] private bool unlockBall;
        [SerializeField] private bool unlockDownSmash;
        [SerializeField] private bool unlockTeleport;
        
        [Header("이동 공통")]
        [SerializeField] private float accelerate = 30f;
        private Vector2 moveDir;
        private bool canMove = true;

        [Header("걷기")]
        [SerializeField] private float walkSpeed = 11.5f;

        [Header("점프")]
        [SerializeField] private float jumpLengthMax = 0.02f;
        [SerializeField] private float jumpSpeed = 100f;
        [SerializeField] private float fallGravityAccel = 2f;
        private bool getKeyZ;
        private bool isHeading;

        [Header("벽 점프")]
        [SerializeField] private float wallDragSpeed = 0.5f;
        [SerializeField] private float wallJumpLengthMax = 0.1f;
        [SerializeField] private float wallJumpSpeed = 1f;
        private bool isGrabWall;
        private bool isOnWall;
        private RaycastHit2D wallHit;

        [Header("코요테 타임")]
        [SerializeField] private float coyoteTime = 0.1f;
        private float lastGroundedTime;
        private float lastGrabWallTime;

        [Header("경사면")]
        [SerializeField] private float angleMax = 50f;
        [SerializeField] private float slopeSpeed = 19f;
        [SerializeField] private float slopeAccel = 30f;
        [SerializeField] private float draggedSlopeSpeed = 10f;
    
        [Header("대쉬")]
        [SerializeField] private float dashSpeed = 40f;
        [SerializeField] private float dashLengthMax = 0.15f;
        [SerializeField] private float dashCoolMax = 0.3f;
        private float dashLengthCur;
        private float dashCoolCur;
        private int dashStatus = NotInAction;
        private float dashDir = 1;
        private bool canAirDash = true; 
        private bool getDashKey;
        private GameObject dashImpact;

        [Header("아래찍기")]
        [SerializeField] private float downSmashBeforeDelay = 0.2f;
        [SerializeField] private float downSmashSpeed = 30f;
        [SerializeField] private float downSmashAfterDelay = 0.3f;
        private float downSmashDelayCur;
        private int downSmashStatus;
        private bool getDownSmashKey;

        [Header("굴러가기")]
        [SerializeField] private float ballMass = 10;
        [SerializeField] private float ballLinearDrag = 0.1f;
        [SerializeField] private float ballAngularDrag;
        [SerializeField] private float ballAccel = 20f;
        [SerializeField] private float ballSpeedMax = 20f;
        [SerializeField] private float ballCoolMax = 0.1f;
        private PhysicsMaterial2D originalMaterial;
        private float originalMass;
        private float originalLinearDrag;
        private float originalAngularDrag;
        private int ballStatus;
        private float ballCoolCur;
        private bool getBallKey;

        [Header("비행")]
        [SerializeField] private float flyingTimeMax = 0.3f;
        [SerializeField] private float flyingSpeed = 15f;
        [SerializeField] private float flyingStartAccel = 1000f;
        private float flyingGravity = 0;
        private float flyingTimeCur;
        private int flyingStatus;
        private bool canFlyingAccel = true;
        private bool getFlyingKey;
        private bool canFlying = true;

        //[Header("텔포")]
        private Vector3 teleportPos;
        private bool getTeleportSaveKey;
        private bool getTeleportLoadKey;
    
        [Header("공격")]
        [SerializeField] private Vector2 sideAttackSize = new Vector2(2.2f, 1.6f);
        [SerializeField] private Vector2 upAttackSize = new Vector2(1.6f, 2.2f);
        [SerializeField] private Vector2 downAttackSize = new Vector2(1.6f, 2.2f);
        [SerializeField] private Vector2 sideAttackPos;
        [SerializeField] private Vector2 upAttackPos;
        [SerializeField] private Vector2 downAttackPos;
        [SerializeField] private float atkCoolMax = 0.3f;
        [SerializeField] private float attackDetectTimeMax = 0.1f;
        [SerializeField] private float downAttackBounceForce = 40000;
        private float atkCoolCur = -1f;
        private bool getAttackKey;
        private bool doAttack;
        private bool isAttacking;
        private bool isSideAttack1 = true;
        private float attackDetectTimeCur;

        [Header("피격")]
        [SerializeField] private float invincibleTimeMax = 1.5f;
        [SerializeField] private float staggerTimeMax = 0.15f;
        [SerializeField] private float knockBackSpeed = 20;
        private float invincibleTimeCur;
        private float staggerTimeCur;
        private bool isStagger;
        
        [Header("카메라")]
        [SerializeField] private Vector3 originalCMPositon = new Vector2(1, 0);
        private Transform cmPointTransform;

        [Space(10)]
        [SerializeField] private LayerMask platform;
        [SerializeField] private Vector2 platformCheckBoxSize = new Vector2(0.3f, 0.03f);
        [SerializeField] private Vector2 wallCheckBoxSize = new Vector2(0.03f, 0.3f);

        //물리
        private float originGravity;
        private float nowGravity;
        
        [Space(10)]
        public float timescale = 1f;

        private float xDir;
        private float yDir;
        private float lookingDir = 1;
        private bool isInvincible;
        private int playerActionStatus;

        //카메라에서 참조, 필요여부 불확실
        public float velocityX;

        // 임시
        private int attackedCount;
        [SerializeField] private GameObject TMP_effect;
        private bool TMP_toggle;
        private bool TMP_toggle2;
        private GameObject tmpenemy;
        private static readonly int DashLengthHash = Animator.StringToHash("Dash Length");
        private static readonly int DashImpactProperty = Animator.StringToHash("Dash Trigger");
        private static readonly int BallBool = Animator.StringToHash("Ball Bool");
        private static readonly int Exit = Animator.StringToHash("Exit");

        private void Start()
        {
            rigid2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            circleCollider2D = GetComponent<CircleCollider2D>();
            animator = GetComponent<Animator>();
            platform = LayerMask.GetMask("Platform");
            frontSlopeChecker = transform.Find("FrontSlopeChecker").gameObject;
            backSlopeChecker = transform.Find("BackSlopeChecker").gameObject;
            dashImpactAnimator = transform.Find("DashImpact").GetComponent<Animator>();
            dashImpactAnimator.SetFloat(DashLengthHash, 1/dashLengthMax);
            dustParticleSys = transform.Find("WalkDustParticle").GetComponent<ParticleSystem>();
            flyParticleSys = transform.Find("FlyParticle").GetComponent<ParticleSystem>();
            dashImpact = transform.Find("DashImpact").gameObject;
            cmPointTransform = transform.Find("CM point");
            
            tmpenemy = GameObject.Find("tmp_enemy2");

            originalMaterial = rigid2D.sharedMaterial;
            originalMass = rigid2D.mass;
            originalLinearDrag = rigid2D.drag;
            originalAngularDrag = rigid2D.angularDrag;
            originGravity = rigid2D.gravityScale;

            playerMovementLogic = new PlayerMovementLogic(walkSpeed, jumpSpeed, jumpLengthMax, angleMax, slopeSpeed, slopeAccel, wallDragSpeed, wallJumpLengthMax, draggedSlopeSpeed, coyoteTime, wallJumpSpeed);
            playerAnimationLogic = new PlayerAnimationLogic(sideAttackObj.GetComponent<Animator>(), upAttackObj.GetComponent<Animator>(), downAttackObj.GetComponent<Animator>());
        }

        private void Update()
        {
            xDir = Input.GetAxisRaw("Horizontal");
            if (xDir != 0 && dashStatus == NotInAction && !isAttacking)
                lookingDir = xDir;
            yDir = Input.GetAxisRaw("Vertical");
            getFlyingKey = !getKeyZ && Input.GetAxisRaw("Jump") == 1F && !isOnPlatform;
            getKeyZ = Input.GetAxisRaw("Jump") == 1f;
            isHeading = false;
            if (getKeyZ)
                isHeading = IsHeading();
            getAttackKey = Input.GetKeyDown(KeyCode.X);
            if (getAttackKey && atkCoolCur <= 0 && (playerActionStatus == NotInAction || flyingStatus == InAction || ballStatus == InAction))
            {
                doAttack = true;
                if (atkCoolCur > -1)
                {
                    isSideAttack1 = !isSideAttack1;
                }
                StopAllAction();
                dashCoolCur = dashCoolMax;
            }
            getDashKey = Input.GetKeyDown(KeyCode.C);
            getDownSmashKey = Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.DownArrow);
            getBallKey = Input.GetKeyUp(KeyCode.A) && !Input.GetKey(KeyCode.DownArrow);
            
            

            #region 액션키입력

            // 비액션 도중
            #region 쿨타임 관리

            if (dashCoolCur < dashCoolMax)
            {
                dashCoolCur += Time.deltaTime;
            }
            if (ballCoolCur < ballCoolMax)
            {
                ballCoolCur += Time.deltaTime;
            }

            #endregion

            #region 단발형 액션

            if(unlockTeleport && Input.GetKeyDown(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                    getTeleportSaveKey = true;
                else
                    getTeleportLoadKey = true;
            }

            #endregion

            #region 지속형 액션

            if (playerActionStatus == NotInAction)
            {
                //지속 액션
                #region 비행

                if(unlockFly && getFlyingKey && canFlying && flyingTimeCur <= flyingTimeMax && lastGroundedTime > coyoteTime)
                {
                    StopAttack();
                    rigid2D.gravityScale = 0;
                    canMove = false;
                    canFlying = false;
                    flyingStatus = InAction;
                    playerActionStatus = InAction;
                    flyParticleSys.Play();
                }

                #endregion
                #region 대쉬

                else if (unlockDash && getDashKey && dashCoolCur >= dashCoolMax && (isOnPlatform && !isOnMaxSlope || canAirDash))
                {
                    StartDash();
                }

                #endregion
                
                // 토글 액션
                #region 공 활성화

                else if (unlockBall && getBallKey && ballCoolCur>= ballCoolMax)
                {
                    ballCoolCur = 0;
                    ChangeBallStatus();
                }

                #endregion
                
                // 딜레이 액션
                #region 다운스매쉬

                else if (unlockDownSmash && getDownSmashKey && !isOnPlatform)
                {
                    downSmashStatus = BeforeDelay;
                    playerActionStatus = InAction;
                    canMove = false;
                }

                #endregion
                
            }
            #endregion
            
            // 액션 도중
            else
            {
                #region 공 비활성화

                if (getBallKey && ballStatus == InAction && ballCoolCur >= ballCoolMax)
                {
                    ballCoolCur = 0;
                    ChangeBallStatus();
                }

                #endregion

                #region 액션 중 대쉬

                if (unlockDash && getDashKey && dashCoolCur >= dashCoolMax && (isOnPlatform && !isOnMaxSlope || canAirDash))
                {
                    if (flyingStatus == InAction)
                    {
                        StopFlying();
                        StartDash();
                    }
                    else if (ballStatus == InAction)
                    {
                        ChangeBallStatus();
                        StartDash();
                    }
                }

                #endregion
                
                #region 비행 컷

                if (!getKeyZ && flyingStatus == InAction)
                {
                    StopFlying();
                    rigid2D.velocity = Vector2.zero;
                }

                #endregion
            }

            #endregion

            #region animator
            string setTrigger;
            if(isStagger)
            {
                setTrigger = "Hurt Trigger";
            }
            else if (playerActionStatus == NotInAction)
            {
                setTrigger = playerAnimationLogic.GetTrigger(getKeyZ, doAttack, rigid2D.velocity, yDir, isOnPlatform,
                    isOnMaxSlope, isSideAttack1);
            }
            else if(dashStatus == InAction)
            {
                setTrigger = "Dash Trigger";
            }
            else if (ballStatus == InAction)
            {
                setTrigger = "Nothing";
            }
            else
            {
                setTrigger = "Flying Trigger";
            }
            foreach (var trigger in triggerList)
            {
                if (trigger == setTrigger)
                    animator.SetTrigger(trigger);
                else
                    animator.ResetTrigger(trigger);
            }
            transform.localScale = new Vector3(lookingDir, 1, 1);
            #endregion

            
            #region 평타 공격
            // attack
            if (atkCoolCur > -1f)
            {
                atkCoolCur -= Time.deltaTime;
                if (atkCoolCur <= -1f)
                {
                    isSideAttack1 = true;
                }
            }
            if (doAttack)
            {
                // 공격 방향별 범위, 위치 설정
                Vector2 attackDir;
                Vector2 attackBox;
                Vector2 attackPos;
                if (yDir == 0)
                {
                    attackDir = Vector2.right * lookingDir;
                    attackBox = sideAttackSize;
                    attackPos = lookingDir * sideAttackPos;
                }
                else
                {
                    attackDir = Vector2.up * yDir;
                    attackBox = upAttackSize;
                    attackPos = yDir == 1 ? upAttackPos : downAttackPos;
                }
                
                // 공격 실행
                StartCoroutine(Attack(attackPos, attackBox, attackDir));

                atkCoolCur = atkCoolMax;
                doAttack = false;
            }
            #endregion


            //디버깅
            try
            {
                var changeCheck = TMP_toggle;
                TMP_toggle = GameObject.Find("TestingToggle").GetComponent<Toggle>().isOn;
                if (changeCheck != TMP_toggle)
                    tmpenemy.SetActive(TMP_toggle);
                // TMP_toggle2 = GameObject.Find("TestingToggle2").GetComponent<Toggle>().isOn;
            }
            catch (NullReferenceException)
            {
            }
            Time.timeScale = timescale;
        }
        private void FixedUpdate()
        {
            #region 변수 할당
            velocityX = rigid2D.velocity.x;
            frontSlopeNormal = GetFrontPlatformNormal();
            backSlopeNormal = GetBackPlatformNormal();
            isOnPlatform = IsOnPlatform();
            isOnMaxSlope = IsOnMaxSlope();
            slopDifference = GetSlopeDifference();
            isEndOfMaxSlope = IsEndOfMaxSlope();
            isGrabWall = IsGrabWall();
            isNearWall = GetWallHit(lookingDir, new Vector2(wallCheckBoxSize.x*1.5f, wallCheckBoxSize.y * 1.5f)).collider != null ||
                         GetWallHit(lookingDir * -1, new Vector2(wallCheckBoxSize.x*1.5f, wallCheckBoxSize.y * 1.5f)).collider != null;
            #endregion
            
            // 액션

            #region InAction

            #region 텔레포트
            // 텔레포트
            
            if (getTeleportSaveKey)
            {
                var position = transform.position;
                teleportPos = position;
                //tmp
                GameObject.Find("dark1").transform.position = position;
                getTeleportSaveKey = false;
            }
            else if (getTeleportLoadKey)
            {
                rigid2D.transform.position = teleportPos;
                getTeleportLoadKey = false;
            }
            
            #endregion
            #region 비행
            
            if(flyingStatus == InAction)
            {
                if(flyingTimeCur < flyingTimeMax)
                {
                    moveDir = new Vector2(xDir, yDir).normalized * flyingSpeed;
                    if (canFlyingAccel)
                    {
                        if (moveDir != Vector2.zero)
                        {
                            rigid2D.AddForce(moveDir.normalized * flyingStartAccel);
                            canFlyingAccel = false;
                        }
                    }
                    flyingTimeCur += Time.deltaTime;
                }
                else
                {
                    StopFlying();
                }
            }
            
            #endregion
            #region 대쉬
            else if (dashStatus == InAction)
            {
                if (dashLengthCur < dashLengthMax)
                {
                    moveDir = new Vector2(dashSpeed * dashDir, 0);
                    dashLengthCur += Time.deltaTime;
                }
                else
                {
                    canMove = true;
                    dashCoolCur = 0;
                    dashLengthCur = 0;
                    dashStatus = NotInAction;
                    playerActionStatus = NotInAction;
                }
            }
            #endregion
            #region 다운스매쉬
            else if (downSmashStatus != NotInAction)
            {
                switch (downSmashStatus)
                {
                    case BeforeDelay:
                    {
                        downSmashDelayCur += Time.deltaTime;
                        nowGravity = 0;
                        moveDir = Vector2.zero;
                        rigid2D.velocity = Vector2.zero;
                        if (downSmashDelayCur > downSmashBeforeDelay)
                        {
                            nowGravity = originGravity;
                            downSmashDelayCur = 0;
                            downSmashStatus = InAction;
                        }
                        break;
                    }
                    case InAction:
                    {
                        moveDir = new Vector2(0, -downSmashSpeed);
                        if (isOnPlatform && !isOnMaxSlope)
                        {
                            downSmashStatus = AfterDelay;
                        }
                        break;
                    }
                    case AfterDelay:
                    {
                        downSmashDelayCur += Time.deltaTime;
                        moveDir = new Vector2(0, 0);
                        if (downSmashDelayCur > downSmashAfterDelay)
                        {
                            downSmashDelayCur = 0;
                            downSmashStatus = NotInAction;
                            playerActionStatus = NotInAction;
                            canMove = true;
                        }
                        break;
                    }
                }
            }
#endregion

            #endregion
            
            #region NotInAction
            else if (playerActionStatus == NotInAction)
            {
                if(isOnPlatform && !isOnMaxSlope)
                {
                    flyingTimeCur = 0;
                    canFlying = true;
                    canAirDash = true;
                }
                
            }
            #endregion

            #region 코요테타임
            if (isOnPlatform && !isOnMaxSlope)
                lastGroundedTime = 0;
            else if(lastGroundedTime <= coyoteTime)
                lastGroundedTime += Time.deltaTime;
            #endregion
            #region moveDir적용
            // moveLogic 에서 moveDir가져옴
            if (canMove)
            {
                moveDir = playerMovementLogic.GetMoveDir(xDir, getKeyZ, isOnPlatform, frontSlopeNormal, backSlopeNormal, rigid2D, slopDifference,
                    lookingDir, isEndOfMaxSlope, isHeading, isOnMaxSlope, isGrabWall, lastGroundedTime, lastGrabWallTime);
            }
            // (가)속도 결정
            if (ballStatus == InAction)
            {
                Vector2 force;
                if(rigid2D.velocity.magnitude > ballSpeedMax)
                {
                    var nowVelocity = rigid2D.velocity;
                    var targetVelocity = nowVelocity.normalized * ballSpeedMax;
                    force = targetVelocity - nowVelocity;
                    force *= 1000;
                }
                else
                {
                    force = ballAccel * Vector2.right * xDir;
                }
                rigid2D.AddForce(force);
                Debug.DrawRay(transform.position, force);
            }
            else
            {
                var velocity = rigid2D.velocity;
                Debug.DrawRay(transform.position, moveDir, Color.blue);
                var force = new Vector2(moveDir.x - velocity.x, moveDir.y - velocity.y) * accelerate;
                rigid2D.AddForce(force);
            }
            #endregion

            #region 중력 설정
            if(playerActionStatus == NotInAction)
            {
                if (rigid2D.velocity.y < 0)
                {
                    nowGravity = originGravity * fallGravityAccel;
                }
                else
                {
                    nowGravity = originGravity;
                }
            }
            else if(flyingStatus == InAction)
            {
                nowGravity = flyingGravity;
            }
            else if (ballStatus == InAction)
            {
                nowGravity = originGravity;
            }

            rigid2D.gravityScale = nowGravity;
            #endregion

            #region Effect처리

            if(isOnPlatform && playerActionStatus == NotInAction)
            {
                dustParticleSys.Play();
            }
            else
            {
                dustParticleSys.Stop();
            }

            #endregion
            #region constraints
            // 경사 미끄러짐 방지, x축 입력 없을때 이동 안함
            // 나중에 피격등으로 움직이게 될시 수정해줘야함
            if (ballStatus == InAction)
                rigid2D.constraints = RigidbodyConstraints2D.None;
            else if (
                xDir == 0 &&
                !isOnMaxSlope &&
                !isEndOfMaxSlope &&
                dashStatus == NotInAction &&
                !playerMovementLogic.wallJumping &&
                !isNearWall &&
                staggerTimeCur == 0 && staggerTimeCur<staggerTimeMax)
                rigid2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            else
                rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            #endregion

            //디버그
            playerMovementLogic.SetUnlockedAbility(unlockWallJump);
        }
        protected void OnTriggerStay2D(Collider2D col)
        {
            if(col.gameObject.layer == LayerMask.NameToLayer("PlayerTouch"))
            {
                var knockBackDir = new Vector2(col.transform.position.x > transform.position.x ? -1 : 1, 0);
                StartAttacked(1, knockBackDir);
            }
        }
        private IEnumerator Attack(Vector2 attackPos, Vector2 attackBox, Vector2 attackDir)
        { 
            var attackedColliders = new Collider2D[3];
            var index = 0;
            isAttacking = true;
            
            // 피격 판정 
            yield return new WaitForSeconds(0.05f);
            while(attackDetectTimeCur < attackDetectTimeMax)
            {
                if (!isAttacking)
                {
                    attackDetectTimeCur = 0;
                    yield break;
                }
                attackDetectTimeCur += Time.deltaTime;
                if (attackDetectTimeCur >= attackDetectTimeMax) break;
                
                var overlapColliders = new Collider2D[3];
                var attackedColliderCount = Physics2D.OverlapBoxNonAlloc( (Vector2)transform.position + attackPos, attackBox, 0,
                    overlapColliders, ~LayerMask.GetMask("Player"));
                
                var enemyHit = false;
                for (var i = 0; i < attackedColliderCount; i++)
                {
                    if (attackedColliders.Contains(overlapColliders[i])) continue;
                    attackedColliders[index++] = overlapColliders[i];
                    
                    if (!HitAbleTagList.Any(hitAbleTag => overlapColliders[i].CompareTag(hitAbleTag))) continue;
                    StartCoroutine(attackedColliders[i].gameObject
                        .GetComponent<IHitAble>()
                        .Attacked( 10, attackDir));
                    enemyHit = true;
                    
                    // 피격 이펙트
                    var hit = Physics2D.BoxCast( (Vector2)transform.position + attackPos, attackBox, 0, attackDir, 0.02f,
                        ~LayerMask.GetMask("Player"));
                    if (hit.collider != null)
                    {
                        Instantiate(TMP_effect, hit.point, Quaternion.identity);
                    }
                } 
                
                // 다운어택 바운스 
                if (enemyHit && attackDir == Vector2.down)
                {
                    StartCoroutine(DownAttackBounce(downAttackBounceForce, 0.05f));
                }

                yield return null;
            }

            attackDetectTimeCur = 0;
            isAttacking = false;
        }
        private IEnumerator DownAttackBounce(float bounceForce, float bounceTimeMax)
        {
            var bounceTimeCur = 0f;
            rigid2D.velocity = Vector2.zero;
            while (bounceTimeCur <= bounceTimeMax)
            {
                bounceTimeCur += Time.deltaTime;
                rigid2D.AddForce(Vector2.up * bounceForce * Time.deltaTime);
                yield return null;
            }
        }
        private void StopAllAction()
        {
            playerActionStatus = NotInAction;
            canMove = true;

            dashStatus = NotInAction;
            dashCoolCur = 0;
            dashLengthCur = 0;

            if (flyingStatus == InAction)
                StopFlying();

            downSmashStatus = NotInAction;
            downSmashDelayCur = 0;

            if (ballStatus == InAction) ChangeBallStatus();
        }
        private void StopFlying()
        {
            canMove = true;
            canFlyingAccel = true;
            rigid2D.gravityScale = originGravity;
            flyingTimeCur = flyingTimeMax;
            flyingStatus = NotInAction;
            playerActionStatus = NotInAction;
            flyParticleSys.Stop();
        }
        private void StartDash()
        {
            if (!isOnPlatform || isOnMaxSlope)
            {
                canAirDash = false;
            }
            canMove = false;
            
            StopAttack();
            
            if (isGrabWall)
            {
                lookingDir *= -1;
            }

            dashDir = lookingDir;
            dashStatus = InAction;
            playerActionStatus = InAction;
            dashImpactAnimator.SetTrigger(DashImpactProperty);
        }
        private void StopAttack()
        {
            isAttacking = false;
            lookingDir = xDir == 0 ? lookingDir : xDir;
            
            sideAttackObj.GetComponent<Animator>().SetTrigger(Exit);
            upAttackObj.GetComponent<Animator>().SetTrigger(Exit);
            downAttackObj.GetComponent<Animator>().SetTrigger(Exit);
        }
        private void ChangeBallStatus()
        {
            switch (ballStatus)
            {
                case NotInAction:
                    animator.SetBool(BallBool,true);
                    canMove = false;
                    playerActionStatus = InAction;
                    ballStatus = InAction;
                    circleCollider2D.enabled = true;
                    boxCollider2D.enabled = false;
                    capsuleCollider2D.enabled = false;
                    rigid2D.sharedMaterial = null;
                    rigid2D.mass = ballMass;
                    rigid2D.drag = ballLinearDrag;
                    rigid2D.angularDrag = ballAngularDrag;
                    cmPointTransform.localPosition = Vector3.zero;
                    break;
                case InAction:
                    animator.SetBool(BallBool,false);
                    canMove = true;
                    playerActionStatus = NotInAction;
                    ballStatus = NotInAction;
                    circleCollider2D.enabled = false;
                    boxCollider2D.enabled = true;
                    capsuleCollider2D.enabled = true;
                    rigid2D.sharedMaterial = originalMaterial;
                    rigid2D.mass = originalMass;
                    rigid2D.drag = originalLinearDrag;
                    rigid2D.angularDrag = originalAngularDrag;
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                    cmPointTransform.localPosition = originalCMPositon;
                    break;
            }
            
        }
        public void StartAttacked(int attackDamage, Vector2 attackDir, float attackForceScale = 1)
        {
            StartCoroutine(Attacked(attackDamage, attackDir, attackForceScale));
        }
        public IEnumerator Attacked(int attackDamage, Vector2 attackDir, float attackForceScale = 1)
        {
            if (attackDir == Vector2.zero)
                attackDir = new Vector2(-lookingDir,0);
            var knockBackDir = attackDir.x;
            if (isInvincible)
                yield return null;
            else
            {
                //tmp
                try
                {
                    attackedCount++;
                    GameObject.Find("HitCount").GetComponent<Text>().text = attackedCount.ToString();
                    GameObject.Find("ResetButton").GetComponent<Button>().interactable = false;
                    GameObject.Find("TestingToggle").GetComponent<Toggle>().interactable = false;
                }
                catch (NullReferenceException)
                {
                }

                StopAllAction();
                isInvincible = true;
                spriteRenderer.color = Color.gray;
                while(invincibleTimeCur < invincibleTimeMax)
                {
                    isStagger = false;
                    if (staggerTimeCur < staggerTimeMax)
                    {
                        isStagger = true;
                        canMove = false;
                        moveDir = new Vector2(2 * knockBackDir, 1).normalized * knockBackSpeed;
                        rigid2D.AddForce(moveDir);
                        staggerTimeCur += Time.deltaTime;
                        if (staggerTimeCur >= staggerTimeMax)
                            canMove = true;
                    }
                    invincibleTimeCur += Time.deltaTime;
                    yield return null;
                }
                spriteRenderer.color = Color.white;
                staggerTimeCur = 0;
                invincibleTimeCur = 0;
                isInvincible = false;
                
                //tmp
                try
                {
                    GameObject.Find("ResetButton").GetComponent<Button>().interactable = true;
                    GameObject.Find("TestingToggle").GetComponent<Toggle>().interactable = true;
                }
                catch (NullReferenceException)
                {
                }
            }
        }
        private bool IsOnPlatform()
        {
            var raycastHit2D = Physics2D.BoxCast(transform.position, platformCheckBoxSize, 0f, Vector2.down,
                boxCollider2D.size.y / 2 + 0.07f - boxCollider2D.offset.y, platform);
            
            return raycastHit2D.collider != null;
        }
        private Vector2 GetFrontPlatformNormal()
        {
            var raycastHit2D = Physics2D.Raycast(frontSlopeChecker.transform.position, Vector2.down, 2.0f, platform);
            return raycastHit2D.normal;
        }
        private Vector2 GetBackPlatformNormal()
        {
            var raycastHit2D = Physics2D.Raycast(backSlopeChecker.transform.position, Vector2.down, 2.0f, platform);
            return raycastHit2D.normal;
        }
        private bool IsHeading()
        {
            var headingCheck = MyDebug.BoxCast(transform.position, platformCheckBoxSize, 0f, Vector2.up, 0.05f, platform);
            return headingCheck.collider != null;
        }
        private bool IsOnMaxSlope()
        {
            return Vector2.Angle(frontSlopeNormal, Vector2.up) > angleMax || Vector2.Angle(backSlopeNormal, Vector2.up) > angleMax;
        }
        private float GetSlopeDifference()
        {
            return Vector2.Angle(frontSlopeNormal, Vector2.up) - Vector2.Angle(backSlopeNormal, Vector2.up);
        }
        private bool IsEndOfMaxSlope()
        {
            return frontSlopeNormal.x != backSlopeNormal.x 
                   && Vector2.Angle(backSlopeNormal, Vector2.up) > angleMax;
        }
        private bool IsGrabWall()
        {
            wallHit = GetWallHit(lookingDir, wallCheckBoxSize);
            isOnWall = wallHit.collider != null && xDir == lookingDir;
            if (!isGrabWall && isOnWall && !isOnPlatform)
            {
                isGrabWall = true;
                lastGrabWallTime = 0;
            }
            if (isOnPlatform || wallHit.collider == null)
            {
                isGrabWall = false;
                if (lastGrabWallTime <= coyoteTime)
                    lastGrabWallTime += Time.deltaTime;
            }
            return isGrabWall;
        }
        private RaycastHit2D GetWallHit(float wallDir, Vector2 checkBoxSize)
        {
            wallHit = Physics2D.BoxCast(transform.position, checkBoxSize, 0f, Vector2.right * wallDir, boxCollider2D.size.x / 2, platform);
            return wallHit;
        } 
        public static Vector2 GetPosToPlayerDir(Vector2 startPos)
        {
            var posToPlayerDir = new Vector2();
            var player = GameObject.Find("Player");
            if (player.transform.position.x < startPos.x) 
                posToPlayerDir.x = -1;
            else
                posToPlayerDir.x = 1;
            
            if (player.transform.position.y < startPos.y)
                posToPlayerDir.y = -1;
            else
                posToPlayerDir.y = 1;
            return posToPlayerDir;
        }
        private void OnDrawGizmos()
        {        
            //공격 범위
            // Gizmos.color = Color.red;
            // var position = transform.position;
            // Gizmos.DrawWireCube(position + lookingDir * (Vector3)sideAttackPos, sideAttackSize);
            // Gizmos.color = Color.blue;
            // Gizmos.DrawWireCube(position + (Vector3)upAttackPos, upAttackSize);
            // Gizmos.color = Color.green;
            // Gizmos.DrawWireCube(position + (Vector3)downAttackPos, downAttackSize);
        }
        

    }
}