using Character.Player;
using UnityEngine;

namespace Mechanics.System
{
    public class TemporaryDangerRange : ActiveTemporary
    {
        [SerializeField] private int damage = 1;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                var playerController = col.gameObject.GetComponent<PlayerController>();
                var attackDir = playerController.GetAttackedDir(transform.position);
                playerController.Hit(1, attackDir);
            }
        }
    }
}
