using UnityEngine;

[CreateAssetMenu(fileName = "MaeHwaStats", menuName = "Boss/MaeHwa Stats")]
public class MaeHwaStats : ScriptableObject
{
    [Header("시작")]
    public float startWaitTime = 5.5f;
    
    [Header("걷기")]
    public float walkSpeed = 3f;
    public float walkTime = 1f;
    public float walkCloselyDistance = 4f;

    [Header("가로베기")]
    public float horizonBeforeWaitTime = 1f;
    public float horizonAfterWaitTime = 1f;
    public float horizonStepSpeed = 20f;

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
    public float rampageRiseSpeed = 5f;
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

    [Header("기타")]
    public float betweenPhaseWaitTime = 1f;

}