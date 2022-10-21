using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementController : MonoBehaviour
{
    // 컴포넌트 선언
    public Rigidbody2D rigid2D;
    public Transform platformCheck;
    CapsuleCollider2D colider2D;
    Animator animator;

    // 걷기 관련 변수
    [SerializeField] float maxWalkSpeed = 5.0f;   // [인스펙터 창에서도 속도값 조절 가능하게 설정] X축의 움직임 최대 속도
    public int xDir = 0; // x축 이동방향

    // 점프 관련 변수
    float jumpSpeed = 5.0f;
    float maxJumpCool = 0.5f;
    float curJumpCool = 0.6f;
    bool canMove = true; // 움직일 수 있는지 여부

    // 착지 관련 변수
    float extraSize = 0.1f;
    bool landing = false;

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>(); 
        colider2D = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement();
        jump();
        land();


        // IDLE 애니메이션
        if (rigid2D.velocity.x == 0 && rigid2D.velocity.y == 0) 
            animator.SetTrigger("Idle Trigger");
        else
            animator.ResetTrigger("Idle Trigger");

        // 낙하 애니메이션
        if(rigid2D.velocity.y < 0 && !isOnPlatForm())
            animator.SetTrigger("Fall Trigger");
        else
            animator.ResetTrigger("Fall Trigger");

        // 좌우 쳐다보게
        if (xDir != 0)
            if (canMove)
                transform.localScale = new Vector3(xDir, 1, 1);

    }
    void movement()
    {
        // 좌우 이동 방향을 xDir에 저장 
        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow)) // 좌위 화살표 동시에 누르면 이동 안하게
            xDir = 0;
        else if (Input.GetKey(KeyCode.LeftArrow)) // 왼쪽 화살표를 눌렀을 때 방향을 좌측으로 변경
            xDir = -1;
        else if (Input.GetKey(KeyCode.RightArrow)) // 오른쪽 화살표를 눌렀을 때 방향을 우측으로 변경
            xDir = 1;
        else
            xDir = 0;
        Debug.Log(Input.GetAxisRaw("Horizontal"));
        // xDir방향으로 x축 이동
        Vector2 moveDir;
        RaycastHit2D rayHit = raycastUnderPlayer();
        if (rayHit.normal.x > 0)
            platformCheck.position = new Vector3(transform.position.x - 0.15f, platformCheck.position.y);
        else if(rayHit.normal.x < 0)
            platformCheck.position = new Vector3(transform.position.x + 0.15f, platformCheck.position.y);

        if (!isOnPlatForm())
        {
            Debug.Log("공중");
            if (curJumpCool >= maxJumpCool && rigid2D.velocity.y > 0)
            {
                moveDir = new Vector2(maxWalkSpeed * xDir, 0);
            }
            else
            {
                moveDir = new Vector2(maxWalkSpeed * xDir, rigid2D.velocity.y);
            }
        }
        else
        {
            Debug.Log("땅");
            if (rayHit.normal.x == 0)
            {
                moveDir = new Vector2(maxWalkSpeed * xDir, 0);
            }
            else if (rayHit.normal.x > 0 && xDir == 1 || rayHit.normal.x < 0 && xDir == -1)
            {
                Debug.DrawRay(rigid2D.position, rayHit.normal.normalized, Color.blue);
                moveDir = Quaternion.AngleAxis(-90, Vector3.forward) * rayHit.normal.normalized * maxWalkSpeed;
            }
            else
            {
                Debug.DrawRay(rigid2D.position, rayHit.normal.normalized, Color.blue);
                moveDir = Quaternion.AngleAxis(90, Vector3.forward) * rayHit.normal.normalized * maxWalkSpeed;
            }
        }
        Debug.DrawRay(rigid2D.position, moveDir, Color.black);
        rigid2D.velocity = moveDir;// x축에서 key방향으로 속력을 지정

        // 경사 미끄러짐 방지
        if (xDir == 0)
            rigid2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else
            rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 걷기 애니메이션
        if (xDir != 0)
            animator.SetTrigger("Run Trigger");
        else
            animator.ResetTrigger("Run Trigger");
    }
    void jump()
    {
        // 점프
        if (Input.GetKey(KeyCode.Z))
        {
            if (curJumpCool < maxJumpCool)
            {
                curJumpCool += Time.deltaTime;
                rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpSpeed * (1 - curJumpCool / maxJumpCool) + 1.0f);

                // 점프 애니메이션
                animator.SetTrigger("Jump Trigger");
            }
            else
                animator.ResetTrigger("Jump Trigger");
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            if (curJumpCool < maxJumpCool)
            {
                rigid2D.velocity = new Vector2(rigid2D.velocity.x, 0);
                curJumpCool = maxJumpCool;
                animator.ResetTrigger("Jump Trigger");
            }
        }

    }
    void land()
    {
        if (isOnPlatForm())
        {
            // 착지 애니메이션 
            animator.ResetTrigger("Land Trigger");
            // isOnPlatForm이 true가 되는 처음에만 Land Trigger 활성화
            if (landing == false)
            {
                landing = true;
                animator.SetTrigger("Land Trigger");
            }
            // Z를 누른채로 착지시 바로 점프되는 현상 방지
            if (!Input.GetKey(KeyCode.Z))
                curJumpCool = 0;
        }
        else
        {
            landing = false;
        }

    }
    RaycastHit2D raycastUnderPlayer()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(platformCheck.position, Vector2.down, 3, LayerMask.GetMask("Platform"));
        Debug.DrawRay(platformCheck.position, Vector2.down, Color.green);
        return rayHit;
    }
    bool isOnPlatForm()
    {
        RaycastHit2D rayHit = raycastUnderPlayer();
        return rayHit.collider != null && rayHit.distance <= extraSize;
    }
}
