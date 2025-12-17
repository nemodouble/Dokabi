using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MaeHwaStats", menuName = "Boss/MaeHwa Stats")]
public class MaeHwaStats : ScriptableObject
{
    [Header("시작")]
    public float startWaitTime = 5.5f;
    
    [Header("걷기")]
    public float walkSpeed = 6f;
    public float walkTime = 1f;
    public float walkCloselyDistance = 4f;

    [Header("스텝")]
    [Tooltip("스텝 시 기준이 되는 상대 방향 (기본 3f 우측)")]
    public float stepOffsetX = 3f;
    [Tooltip("스텝 시 최대 이동 속도")]
    public float stepMaxSpeed = 20f;
    [Tooltip("스텝 시 감속 가속도(절대값이 클수록 급감속)")]
    public float stepDecelAccel = 10f;
    [Tooltip("감속을 시작하는 길이 비율 (1이면 전체 거리, 0.5면 절반 지점부터 감속)")]
    public float stepDecelStartRatio = 0.3f;
    [Tooltip("플레이어를 기준으로 스텝을 수행할 최대 거리")]
    public float stepMaintainDistance = 10f;

    [Header("가로베기")]
    public float horizonBeforeWaitTime = 1f;
    public float horizonAfterWaitTime = 1f;
    public float horizonStepSpeed = 20f;
    public float horizonTeleportWaitTime = 0.2f;
    [Tooltip("가로 돌진 시 최대 이동 시간")]
    public float horizonMaxRunTime = 3f;

    [Header("바디태클")]
    public float bodyDashSpeed = 20f;
    public float bodyDashTime = 0.3f;
    public float bodyAfterDashWaitTime = 1f;
    public float bodyAfterAttackWaitTime = 1f;
    public float bodyDashStopBeforeObstacleDistance = 3f;

    [Header("콤보")]
    public float comboFirstBeforeWaitTime = 0.4f;
    public float comboAfterFirstWaitTime = 0.1f;
    public float comboBeforeSecondWaitTime = 0.4f;
    public float comboAfterSecondWaitTime = 0.1f;
    public float comboBeforeThirdWaitTime = 0.9f;
    public float comboAfterThirdWaitTime = 1f;
    public float comboNormalSpeed = 10f;
    public float comboNormalLength = 0.2f;
    public float comboStingSpeed = 20f;
    public float comboStingTime = 0.2f;
    public float comboSkipSecondDistance = 5f;
    public float comboAttackWithDashDistance = 2.5f;

    [Header("난무")]
    public float rampageRiseSpeed = 20f;
    public float rampageRiseTime = 0.3f;
    public float rampageRiseWaitTime = 0.3f;
    public float rampageBeforeNoticeWaitTime = 1f;
    public float rampageBlinkWait = 2f;
    public float rampageNoticeInterval = 0.1f;
    public float rampageBeforeAttackTime = 0.5f;
    public float rampageAttackTime = 0.2f;
    public float rampageAttackAfterWaitTime = 0.3f;
    public float rampageStaggerTime = 3f;


    [Header("다운스매싱")]
    public float downAirWaitTime = 0.5f;
    public float downAccel = 30f;
    public float downAccelTime = 2f;
    public float downAfterSmashTime = 1f;
    public float downTeleportYPos = 7f;

    [Header("기타")]
    public float betweenPhaseWaitTime = 1f;

}