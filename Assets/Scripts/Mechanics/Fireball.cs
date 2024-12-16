using UnityEngine;
using Platformer.Gameplay;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class Fireball : MonoBehaviour
    {
        void Update()
        {
            // Verificar si la bola de fuego está fuera de la pantalla
            if (!GetComponent<Renderer>().isVisible)
            {
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            var player = collider.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // Programar el evento de muerte del jugador
                var ev = Simulation.Schedule<PlayerDeath>();
            }
        }
    }
}