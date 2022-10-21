using UnityEngine;

namespace Player
{
    public class PlayerMovementLogic
    {
        // 생성시 넘겨받는 상수
        private readonly float walkSpeed;
        private readonly float jumpSpeed;
        private readonly float slopeSpeedMax;
        private readonly float dragSlopeSpeed;
        private readonly float jumpCoolMax;
        private readonly float maxAngle;
        private readonly float slopeAccel;
        private readonly float wallDragSpeed;
        private readonly float wallJumpCoolMax;
        private readonly float coyoteTime;
        private readonly float wallJumpSpeed;

        //변수
        private float jumpCoolCur = 0.6f;
        private float slopeSpeedCur;
        private float wallJumpCoolCur;
        private float wallJumpDir;
        private bool isRising;
        private bool beforeGetKeyZ;
        // private bool canDoubleJump;
        public bool wallJumping;

        // private bool unlockDoubleJump;
        private bool unlockWallJump;
        
        //Debug
        
        
        public PlayerMovementLogic(float walkSpeed, float jumpSpeed, float jumpCoolMax, float maxAngle,float slopeSpeedMax,float slopeAccel, 
            float wallDragSpeed, float wallJumpCoolMax, float dragSlopeSpeed, float coyoteTime, float wallJumpSpeed)
        {
            this.walkSpeed = walkSpeed;
            this.jumpSpeed = jumpSpeed;
            this.jumpCoolMax = jumpCoolMax;
            this.maxAngle = maxAngle;
            this.slopeSpeedMax = slopeSpeedMax;
            this.dragSlopeSpeed = dragSlopeSpeed;
            this.slopeAccel = slopeAccel;
            this.wallDragSpeed = wallDragSpeed;
            this.wallJumpCoolMax = wallJumpCoolMax;
            this.coyoteTime = coyoteTime;
            this.wallJumpSpeed = wallJumpSpeed;
        }

        public void SetUnlockedAbility(bool unlockWallJump)
        {
            this.unlockWallJump = unlockWallJump;
        }
        public Vector2 GetMoveDir(float xDir, bool getKeyZ, bool isOnPlatform, Vector2 frontSlopeNormal,
            Vector2 backSlopeNormal, Rigidbody2D rigid, float slopeDifference, float lookingDir,
            bool isEndOfMaxSlope, bool isHeading, bool isOnMaxSlope, bool isGrabWall, float lastGroundedTime,
            float lastGrabWallTime)
        {
            var playerVelocity = rigid.velocity;
            var isCoyoteTime = lastGroundedTime <= coyoteTime;
            var getKeyZDown = !beforeGetKeyZ && getKeyZ;
            var getKeyZUp = beforeGetKeyZ && !getKeyZ;
            beforeGetKeyZ = getKeyZ;
            var frontSlopeAngle = Vector2.Angle(frontSlopeNormal, Vector2.up);
            var backSlopeAngle = Vector2.Angle(backSlopeNormal, Vector2.up);
            var dir = new Vector2(0, 0);
            var isOnJumpAble = isOnPlatform && !isOnMaxSlope;
            // 점프 컷
            if (playerVelocity.y > 0 && (getKeyZUp || isHeading))
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0);
                jumpCoolCur = jumpCoolMax;
                isRising = false;
            }
            
            // 벽 점프 방향 지정
            if(isGrabWall)
            {
                wallJumpDir = -lookingDir;
            }
            
            // 점프 초기화
            if(!getKeyZ && (isOnJumpAble || isCoyoteTime))
            {
                jumpCoolCur = 0;
                // canDoubleJump = true;
            }
            // // 더블 점프 사용
            // else if(unlockDoubleJump && canDoubleJump && !isOnJumpAble && getKeyZDown)
            // {
            //     jumpCoolCur = 0;
            //     isRising = true;
            //     if (!isCoyoteTime)
            //         canDoubleJump = false;
            // }
            
            // 급경사 시작 속도 설정
            if(!isOnMaxSlope)
            {
                if (playerVelocity.y < 0)
                    slopeSpeedCur = - playerVelocity.y / 3.0f;
                else
                    slopeSpeedCur = 0;
            }
            
            // 입력 무시 영역
            if (wallJumping) // 벽점프 진행중
            {
                wallJumpCoolCur += Time.deltaTime;
                jumpCoolCur += Time.deltaTime;
                isRising = true;
                if(jumpCoolCur <= jumpCoolMax)
                {
                    dir = new Vector2(wallJumpSpeed * walkSpeed * wallJumpDir,
                        jumpSpeed * (1 - jumpCoolCur / jumpCoolMax + 1.0f));
                }
                else
                {
                    dir = new Vector2(wallJumpSpeed * walkSpeed * wallJumpDir, playerVelocity.y);
                }
                if (wallJumpCoolCur > wallJumpCoolMax)
                {
                    wallJumping = false;
                }
            }
            else
            {
                // 벽점프 입력
                if (unlockWallJump && getKeyZDown && (isGrabWall || lastGrabWallTime < coyoteTime))
                {
                    wallJumping = true;
                    wallJumpCoolCur = 0;
                    // canDoubleJump = true;
                    jumpCoolCur = 0;
                }
                // 플랫폼 위 이동
                else if (isOnPlatform)
                {
                    // 평지걸을때, 평지에서 급경사 떨어질때
                    if (frontSlopeAngle == 0 && backSlopeAngle == 0 || slopeDifference * lookingDir * -1 > maxAngle)
                    {
                        dir = new Vector2(xDir * walkSpeed, 0);
                    }
                    // 급경사 미끄러짐 끝날때
                    else if (isEndOfMaxSlope)
                    {
                        dir = Vector2.Perpendicular(backSlopeNormal).normalized * slopeSpeedMax;
                    }
                    // 급경사 미끄러지다 벗어날때
                    else if (xDir * frontSlopeNormal.x > 0 && backSlopeAngle > maxAngle)
                    {
                        dir = new Vector2(xDir * walkSpeed, playerVelocity.y);
                    }
                    // 급경사 미끄러짐
                    else if (isOnMaxSlope)
                    {
                        float slopeSpeed;
                        if (xDir * frontSlopeNormal.x < 0)
                            slopeSpeed = dragSlopeSpeed;
                        else
                            slopeSpeed = slopeSpeedMax;
                        dir = Vector2.Perpendicular(frontSlopeNormal).normalized * slopeSpeedCur;
                        if (slopeSpeedCur < slopeSpeed)
                            slopeSpeedCur += (slopeSpeed / slopeSpeedMax) * slopeAccel * Time.deltaTime;
                        else if (slopeSpeedCur >= slopeSpeed)
                            slopeSpeedCur = slopeSpeed;
                    }
                    // 경사 갈때
                    else if(frontSlopeNormal == Vector2.zero)
                    {
                        dir = Vector2.Perpendicular(backSlopeNormal).normalized * xDir * walkSpeed * -1;
                    }
                    else
                    {
                        dir = Vector2.Perpendicular(frontSlopeNormal).normalized * xDir * walkSpeed * -1;
                    }
                }
                // 공중 이동
                else
                {
                    dir.x = xDir * walkSpeed;
                    if (unlockWallJump && isGrabWall)
                    {
                        dir.y = playerVelocity.y * wallDragSpeed;
                    }
                    else
                    {
                        dir.y = playerVelocity.y;
                    }
                }
                
                // 점프시 dir.y 덮어쓰기
                if (jumpCoolCur < jumpCoolMax && ((isOnJumpAble || isCoyoteTime) && getKeyZDown || !isOnJumpAble && getKeyZ && isRising))
                {
                    jumpCoolCur += Time.deltaTime;
                    dir.y = jumpSpeed * (1 - jumpCoolCur / jumpCoolMax + 1.0f);
                    isRising = jumpCoolCur < jumpCoolMax;
                }
            }
            return dir;
        }
    }
}