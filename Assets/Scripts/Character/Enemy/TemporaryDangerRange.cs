using Character.Player;
using UnityEngine;

namespace Mechanics.System
{
    public class TemporaryDangerRange : ActiveTemporary
    {
        [SerializeField] private int damage = 1;

        private void FixedUpdate()
        {
            var atkBox = GetComponent<BoxCollider2D>().size;
            var hit = Physics2D.OverlapBox(transform.position, atkBox, 0, LayerMask.GetMask("Player"));
            if (hit != null)
            {
                var playerController = hit.gameObject.GetComponent<PlayerController>();
                var attackDir = playerController.GetAttackedDir(transform.position);
                playerController.Hit(1, attackDir);
            }
        }
    }
}
