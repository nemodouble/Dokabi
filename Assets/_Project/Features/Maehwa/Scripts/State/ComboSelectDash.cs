using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;

namespace _Project.Features.Maehwa.Scripts.State
{
    public class ComboSelectDash : WaitState<MaehwaStateId>
    {
        private bool _isThird;
        public DashDir SelectedDashDir { get; private set; }
        public enum DashDir
        {
            None,
            Left,
            Right
        }
        
        public ComboSelectDash(MaehwaStateId id, float waitingSecond, bool notMoving = false, bool isThird = false) : base(id, waitingSecond, notMoving)
        {
            _isThird = isThird;
        }

        public override void OnEnter(BossContext<MaehwaStateId> ctx)
        {
            base.OnEnter(ctx);
            var c = ctx as MaehwaContext;
            if (!_isThird)
                SelectedDashDir = c? c.SelectDashDir() : DashDir.None;
            else
                SelectedDashDir = ctx.PlayerTransform.position.x < ctx.transform.position.x ? DashDir.Left : DashDir.Right;

            if (c != null)
                c.SelectedDashDir = SelectedDashDir;
        }
    }
}