using Platformer.Gameplay;
using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    // Color asignado a la plataforma
    public Color platformColor;

    private Rigidbody2D rb;
    private bool isStable = true;
    private Vector3 initialPosition;

    public Color[] availableColors;
    public Platforms[] platforms;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; // Se mantiene estable hasta que se active la caída
        }
        // Guarda la posición inicial de la plataforma
        initialPosition = transform.position;

        platforms = FindObjectsOfType<Platforms>();


        Color[] availableColors = new Color[2];
       


    }

    // Verifica si el color del jugador coincide con el de la plataforma
    public void CheckPlayerColor(PlayerController player)
    {
        if (player.currentColor == platformColor)
        {
            StayStable();
        }
        else
        {
            Fall();
        }
    }

    private void StayStable()
    {
        Debug.Log("Plataforma estable. ¡Buen salto!");
        // Aquí podrías agregar efectos visuales o de sonido
    }

    private void Fall()
    {
        if (isStable)
        {
            isStable = false;
            if (rb != null)
            {
                rb.isKinematic = false; // Permite que la física haga que la plataforma caiga
            }
            Debug.Log("La plataforma se cae.");
        }
    }

    // Reinicia la plataforma a su estado inicial y asigna un nuevo color
    public void ResetPlatform(Color newColor)
    {
        // Restablece posición y estado
        transform.position = initialPosition;
        isStable = true;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }
        // Asigna el nuevo color (proporcionado por el LevelManager)
        platformColor = newColor;

        // Actualiza visualmente el color, si la plataforma cuenta con SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = newColor;
        }

     
    }

    void playerDeath(PlayerController player)
    {
        foreach (Platforms platform in platforms)
        {
            Color randomColor = availableColors[Random.Range(0, availableColors.Length)];
            platform.ResetPlatform(randomColor);
        }
    }
}
