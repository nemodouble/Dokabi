using Character.Player;
using UnityEngine;

namespace Mechanics.System
{
    public class DangerRange : ActiveTemporary
    {
        [SerializeField] private int damage = 1;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag.Equals("Player"))
            {
                var attackDir = transform.position.x > GameObject.Find("Player").transform.position.x ? -1 : 1;
                PlayerController player = col.GetComponent<PlayerController>();
                if(player.CanChangeActionState(PlayerController.ActionStatus.Stagger))
                {
                    player.ChangeActionState(PlayerController.ActionStatus.Stagger);
                }
                //gameObject.SetActive(false);
            }
        }
    }
}
