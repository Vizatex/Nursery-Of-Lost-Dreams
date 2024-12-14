using System.Collections;
using UnityEngine;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class BossController : MonoBehaviour
    {
        public int health = 15;
        public float flySpeed = 5f;
        public float fireballSpeed = 10f;
        public GameObject fireballPrefab;
        public Transform fireballSpawnPoint;
        public float minAttackInterval = 1f;
        public float maxAttackInterval = 3f;
        public float restDuration = 5f;
        public float maxHeight = 10f; // Altura máxima de vuelo
        public float minHeight = 2f; // Altura mínima de vuelo
        public float lateralSpeed = 3f; // Velocidad lateral
        public Transform player; // Referencia al jugador
        public float screenWidth = 16f; // Ancho de la pantalla
        public float minChangeDirectionInterval = 1f; // Intervalo mínimo entre cambios de dirección
        public float maxChangeDirectionInterval = 3f; // Intervalo máximo entre cambios de dirección

        private bool isResting = false;
        private AnimationController animationController;
        private Collider2D collider2d;
        private Vector3 targetPosition;

        void Awake()
        {
            animationController = GetComponent<AnimationController>();
            collider2d = GetComponent<Collider2D>();
        }

        void Start()
        {
            StartCoroutine(BehaviorRoutine());
            StartCoroutine(ChangeDirectionRoutine());
        }

        void Update()
        {
            if (!isResting)
            {
                Move();
                LookAtPlayer();
            }
        }

        void Move()
        {
            // Movimiento suave del boss
            transform.position = Vector3.Lerp(transform.position, targetPosition, flySpeed * Time.deltaTime);

            // Limitar la altura del boss
            if (transform.position.y > maxHeight)
            {
                transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
            }
            else if (transform.position.y < minHeight)
            {
                transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
            }

            // Limitar el movimiento lateral del boss dentro de la pantalla
            if (transform.position.x > screenWidth / 2)
            {
                transform.position = new Vector3(screenWidth / 2, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < -screenWidth / 2)
            {
                transform.position = new Vector3(-screenWidth / 2, transform.position.y, transform.position.z);
            }
        }

        void LookAtPlayer()
        {
            if (player != null)
            {
                Vector3 direction = player.position - transform.position;
                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(-2, 2, 2); // Mirar a la derecha
                }
                else
                {
                    transform.localScale = new Vector3(2, 2, 2); // Mirar a la izquierda
                }
            }
        }

        IEnumerator ChangeDirectionRoutine()
        {
            while (health > 0)
            {
                // Cambiar la dirección de movimiento cada cierto tiempo
                float directionX = Random.Range(-screenWidth / 2, screenWidth / 2);
                float directionY = Random.Range(minHeight, maxHeight);
                targetPosition = new Vector3(directionX, directionY, 0);

                // Esperar un tiempo aleatorio antes de cambiar de dirección
                float changeDirectionInterval = Random.Range(minChangeDirectionInterval, maxChangeDirectionInterval);
                yield return new WaitForSeconds(changeDirectionInterval);
            }
        }

        IEnumerator BehaviorRoutine()
        {
            while (health > 0)
            {
                if (!isResting)
                {
                    FireballAttack();
                    float attackInterval = Random.Range(minAttackInterval, maxAttackInterval);
                    yield return new WaitForSeconds(attackInterval);
                }
                else
                {
                    yield return new WaitForSeconds(restDuration);
                    isResting = false;
                }
            }
        }

        void FireballAttack()
        {
            Debug.Log("Disparando bola de fuego");
            var fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
            Vector2 fireballDirection = (player.position - fireballSpawnPoint.position).normalized;
            fireball.GetComponent<Rigidbody2D>().velocity = fireballDirection * fireballSpeed;
            fireball.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(fireballDirection.y, fireballDirection.x) * Mathf.Rad2Deg));
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Colisión detectada");
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("Jugador detectado, matando al jugador");
                player.health.Die();
            }
        }

        IEnumerator RestRoutine()
        {
            isResting = true;
            // Lógica para descender al suelo
            while (transform.position.y > 0)
            {
                transform.position += Vector3.down * flySpeed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(restDuration);
            isResting = false;
        }
    }
}