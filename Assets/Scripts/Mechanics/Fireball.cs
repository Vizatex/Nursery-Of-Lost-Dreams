using UnityEngine;

namespace Platformer.Mechanics
{
    public class Fireball : MonoBehaviour
    {
        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // Matar al jugador al contacto
                player.health.Die();
            }
            Destroy(gameObject);
        }
        
    }
}