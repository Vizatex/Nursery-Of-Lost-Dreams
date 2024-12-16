using UnityEngine;
using Platformer.Gameplay;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class BossDeathZone : MonoBehaviour
    {
        public float deathYLevel = -10f; // Nivel Y por debajo del cual el jugador morirá

        void Update()
        {
            var player = FindObjectOfType<PlayerController>();
            if (player == null || player.transform.position.y >= deathYLevel)
            {
                return;
            }
            // Programar el evento de muerte del jugador
            Simulation.Schedule<PlayerDeath>();
        }
    }
}