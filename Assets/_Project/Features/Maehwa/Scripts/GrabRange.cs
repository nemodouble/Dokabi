using Character.Player;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts
{
    public class GrabRange : MonoBehaviour
    {
        public Transform grabPosition;
        
        private void OnTriggerStay2D(Collider2D col)
        {
            if (!col.gameObject.CompareTag("Player")) return;
            var pc = col.gameObject.GetComponent<PlayerController>();
            pc.TryChangeStunState(1f, true);
            pc.SetPosition(grabPosition.position);
        }
    }
}
