using System;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationLogic
    {
        private readonly Animator sideEffectAnimator;
        private readonly Animator topEffectAnimator;
        private readonly Animator downEffectAnimator;

        private bool falling;
        private static readonly int AtkSide1 = Animator.StringToHash("AtkSide1");
        private static readonly int AtkTop = Animator.StringToHash("AtkTop");
        private static readonly int AtkDown = Animator.StringToHash("AtkDown");
        private static readonly int AtkSide2 = Animator.StringToHash("AtkSide2");

        public PlayerAnimationLogic(Animator side, Animator top, Animator down)
        {
            sideEffectAnimator = side;
            topEffectAnimator = top;
            downEffectAnimator = down;
        }

        public string GetTrigger(bool getKeyZ, bool isAttacking, Vector2 velocity, float yDir, bool onPlatform, bool isOverMaxAngleSlop, bool isSideAttack1)
        {
            if (isAttacking)
            {
                switch (yDir)
                {
                    case 0:
                        if(isSideAttack1)
                        {
                            sideEffectAnimator.SetTrigger(AtkSide1);
                            return "AtkSide1 Trigger";
                        }
                        else
                        {
                            sideEffectAnimator.SetTrigger(AtkSide2);
                            return "AtkSide2 Trigger";
                        }
                    case 1:
                        topEffectAnimator.SetTrigger(AtkTop);
                        return "AtkUp Trigger";
                    default:
                        downEffectAnimator.SetTrigger(AtkDown);
                        return "AtkDown Trigger";
                }
            }
            if (getKeyZ && velocity.y > 0 && !onPlatform)
            {
                return "Jump Trigger";
            }

            if (velocity.y < 0 && !onPlatform || isOverMaxAngleSlop)
            {
                falling = true;
                return "Fall Trigger";
            }

            if (falling && onPlatform)
            {
                falling = false;
                return "Land Trigger";
            }

            if (Math.Abs(velocity.x) >= 0.05)
            {
                return "Run Trigger";
            }

            return "Idle Trigger";
        }
    }
}
