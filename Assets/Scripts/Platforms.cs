using Platformer.Gameplay;
using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    // Color asignado a la plataforma.
    public Color platformColor;

    // Arreglo de colores disponibles para asignar.
    // Desde el Inspector podrás asignar los colores que desees.
    public Color[] availableColors;

    private Rigidbody2D rb;
    private bool isStable = true;
    private Vector3 initialPosition;

    // Arreglo estático que contiene todas las instancias de Platforms en la escena.
    public static Platforms[] allPlatforms;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // La plataforma se mantiene estable (sin ser afectada por la física) hasta que se active la caída.
            rb.isKinematic = true;
        }
        // Guarda la posición inicial para poder reiniciarla.
        initialPosition = transform.position;
    }

    void Start()
    {
        // Obtiene todas las plataformas de la escena.
        allPlatforms = FindObjectsOfType<Platforms>();

        // Si hay colores disponibles, asigna aleatoriamente uno al inicio.
        if (availableColors != null && availableColors.Length > 0)
        {
            int randomIndex = Random.Range(0, availableColors.Length);
            platformColor = availableColors[randomIndex];
            UpdateSpriteColor();
        }
    }

    // Actualiza el color del SpriteRenderer, sin modificar el sprite.
    void UpdateSpriteColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = platformColor;
        }
    }

    // Método que debe ser llamado al detectar que el jugador está sobre la plataforma.
    // Si el color del jugador coincide con el color de la plataforma, la plataforma se mantiene estable;
    // de lo contrario, la plataforma se "desactiva" (cae).
    public void CheckPlayerColor(PlayerController player)
    {
        if (ColorUtility.ToHtmlStringRGB(player.currentColor) == ColorUtility.ToHtmlStringRGB(platformColor))
        {
            StayStable();
        }
        else
        {
            DisableCollider();
        }
    }

    private void StayStable()
    {

    }


    // Reinicia la plataforma a su estado inicial y asigna un nuevo color (tomado aleatoriamente de availableColors).
    // Solo se actualiza el color; el sprite se mantiene.
    public void ResetPlatform(Color newColor)
    {
        transform.position = initialPosition;
        platformColor = newColor;

        // Restablecer color visual
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = newColor;
        }

        // Reactivar el Collider2D
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    private void DisableCollider()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
            StartCoroutine(EnableColliderAfterDelay(2f)); // Reactiva después de 2 segundos
        }
       
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
       
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            CheckPlayerColor(player);
        }
    }



}

