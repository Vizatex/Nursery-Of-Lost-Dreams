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

        [Header("Movement Settings")]
        public float maxSpeed = 7;
        public float jumpTakeOffSpeed = 12;
        [Tooltip("Multiplier applied to horizontal speed while airborne.")]
        public float airControlFactor = 0.5f;

        [Header("Jump Physics Settings")]
        public bool enableCustomGravity = false;
        public float fallGravityMultiplier = 1.0f;
        public float lowJumpGravityMultiplier = 1.0f;

        [Header("Dash Settings")]
        public float dashSpeed = 15f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 5f;

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

        public Color currentColor;
        public CharacterSkin[] availableSkins;
        private int currentSkinIndex = 0;

        private bool isDashing;
        private float dashTime;
        private float dashCooldownTimer;

        private int jumpCount = 0;
        public int maxJumpCount = 2;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            attackCollider.SetActive(false);

            if (availableSkins != null && availableSkins.Length > 0)
            {
                currentSkinIndex = 0;
                UpdateSkin();
            }
            else
            {
                currentColor = spriteRenderer.color;
            }

            isDashing = false;
            dashTime = 0f;
            dashCooldownTimer = 0f;
        }

        protected override void Update()
        {
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

                if ((jumpState == JumpState.Grounded || jumpCount < maxJumpCount) && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                    jumpCount++;
                }
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

                dashCooldownTimer -= Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0f)
                {
                    isDashing = true;
                    dashTime = dashDuration;
                    dashCooldownTimer = dashCooldown;
                }

                if (isDashing)
                {
                    dashTime -= Time.deltaTime;
                    if (dashTime <= 0f)
                    {
                        isDashing = false;
                    }
                }
            }
            else
            {
                move.x = 0;
            }

            UpdateJumpState();
            base.Update();
        }

        void UpdateSkin()
        {
            CharacterSkin skin = availableSkins[currentSkinIndex];
            spriteRenderer.sprite = skin.sprite;
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
                        jumpCount = 0;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && (IsGrounded || jumpCount <= maxJumpCount))
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

            if (enableCustomGravity)
            {
                if (velocity.y < 0)
                {
                    velocity.y += Physics2D.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
                }
                else if (velocity.y > 0 && !Input.GetButton("Jump"))
                {
                    velocity.y += Physics2D.gravity.y * (lowJumpGravityMultiplier - 1) * Time.deltaTime;
                }
            }

            float effectiveSpeed = maxSpeed;
            if (!IsGrounded && !isDashing)
            {
                effectiveSpeed *= airControlFactor;
            }

            if (isDashing)
            {
                targetVelocity = new Vector2(move.x * dashSpeed, velocity.y);
            }
            else
            {
                targetVelocity = move * effectiveSpeed;
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        }

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