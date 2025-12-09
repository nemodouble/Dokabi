using _Project.Features.Boss.Scripts;
using FMODUnity;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaehwaSoundPlayer : BossSoundPlayer
    {
        // 사운드
        public EventReference horizonAttackEvent;
        public EventReference bodyAttackEvent;
        public EventReference comboFirstAttackEvent;
        public EventReference comboSecondAttackEvent;
        public EventReference comboStingAttackEvent;
        public EventReference downSmashEvent;
        public EventReference rampageWindEvent;
        public EventReference rampageRiseEvent;
        public EventReference dashEvent;
        public EventReference walkEvent;
        public EventReference yell1;
        public EventReference yell2;
        public EventReference yell3;
        public EventReference jump;
        public EventReference land;
        public EventReference deadVoice;
        public EventReference outro;
        public EventReference teleportEvent;
    }
}