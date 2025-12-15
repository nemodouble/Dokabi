using _Project.Core.Scripts;
using Character.Player;
using Mechanics.System;
using UnityEngine;

namespace _Project.Features.Battle.Scripts
{
    public class FixedDangerRange : ActiveTemporary
    {
        [SerializeField] private int damage = 1;

        private void OnTriggerEnter2D(Collider2D col)
        {
            var playerController = col.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                var attackDir = playerController.GetAttackedDir(transform.position);
                playerController.Hit(1, attackDir);
            }
        }
    } 
}
