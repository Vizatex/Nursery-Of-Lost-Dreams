using Platformer.Mechanics;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(1);
        }
    }
}