using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        public float maxSpeed = 7;
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public GameObject attackCollider;

        public Bounds Bounds => collider2d.bounds;

        // Color actual del jugador (se actualizará al cambiar el skin)
        public Color currentColor;

        // NUEVAS VARIABLES PARA LOS SKINS 
        // Array de skins (cada uno con sprite y color)
        public CharacterSkin[] availableSkins;
        // Índice para llevar la cuenta del skin actual
        private int currentSkinIndex = 0;
        // 

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            attackCollider.SetActive(false);

            // Si se definieron skins, inicializa con el primero
            if (availableSkins != null && availableSkins.Length > 0)
            {
                currentSkinIndex = 0;
                UpdateSkin();
            }
            else
            {
                // Si no hay skins definidos, se usa el sprite y color actuales
                currentColor = spriteRenderer.color;
            }
        }

        protected override void Update()
        {
            // Al presionar "Fire2", se cambia al siguiente skin (sprite y color)
            if (Input.GetButtonDown("Fire2"))
            {
                if (availableSkins != null && availableSkins.Length > 0)
                {
                    currentSkinIndex = (currentSkinIndex + 1) % availableSkins.Length;
                    UpdateSkin();
                }
            }

            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    animator.SetTrigger("attack");
                    attackCollider.SetActive(true);
                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    attackCollider.SetActive(false);
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        // Actualiza el sprite y color del personaje según el skin seleccionado.
        void UpdateSkin()
        {
            CharacterSkin skin = availableSkins[currentSkinIndex];
            // Actualiza el sprite del SpriteRenderer (el Animator usará este sprite en sus animaciones)
            spriteRenderer.sprite = skin.sprite;
            // Actualiza el color
            ChangeColor(skin.color);
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        // Método que cambia el color del jugador (actualiza la variable y el SpriteRenderer)
        public void ChangeColor(Color newColor)
        {
            currentColor = newColor;
            spriteRenderer.color = newColor;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}