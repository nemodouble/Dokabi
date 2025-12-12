using Character.Player;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts
{
    public class KnockBackRange : MonoBehaviour
    {
        private void OnCollisionStay2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                PlayerController player = col.gameObject.GetComponent<PlayerController>();
                if (player.CanChangeActionState(PlayerController.ActionStatus.Stun))
                {
                    col.gameObject.GetComponent<PlayerController>().StartStunState(1f, true);
                }
            }
        }
    }
}
