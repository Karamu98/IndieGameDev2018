using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;


public enum Direction
{
    FORWARD,
    BACKWARDS,
    LEFT,
    RIGHT
}

public class CS_Player : CS_Character
{
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject mapCamera;
    [SerializeField] CS_SpriteAnimator beenHitAnim;
    [SerializeField] RawImage[] MiniImages;
    [SerializeField] GameObject[] UIElements;
    [SerializeField] GameObject deathUI;

    // Audio
    [SerializeField] AudioClip heal;
    [SerializeField] AudioClip Hurt;
    [SerializeField] AudioClip SpellLaunch;
    [SerializeField] AudioClip Swipe;

    // Use this for initialization
    void Start()
    {
        CS_GameManager.SetPlayer(this);
        // Start the player at the correct height
        float y = (-CS_GameManager.fGridCellSize * 0.5f);

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(bGameActive)
        {
            /// Attacking
            HandleAttacking();
            /// Movement
            HandleMovement();

            /// Turning
            HandleTurning();

            /// Minimap toggle
            Minimap();
        }
	}

    private void HandleAttacking()
    {
        UpdateAttacking();

        if (CrossPlatformInputManager.GetButtonDown("Melee"))
        {
            if(CanMelee())
            {
                MeleeAttack();
                audioSource.clip = Swipe;
                audioSource.Play();
            }
        }

        if(CrossPlatformInputManager.GetButtonDown("Magic"))
        {
            if (MagicAttack())
            {
                audioSource.clip = SpellLaunch;
                audioSource.Play();
            }
        }
    }

    public override void TakeDamage(int a_amount)
    {
        beenHitAnim.Animate();
        audioSource.clip = Hurt;
        audioSource.Play();
        base.TakeDamage(a_amount);
    }

    protected override void Die()
    {
        base.Die();

        // Remove all UI
        for (int i = 0; i < 2; i++)
        {
            Destroy(MiniImages[i]);
        }

        for (int i = 0; i < UIElements.Length; i++)
        {
            Destroy(UIElements[i]);
        }

        // Display dead UI
        deathUI.SetActive(true);

        CS_GameManager.PlayerDied();
    }

    private void HandleMovement()
    {
        // Testing is restricted if the player is already moving
        if (!bIsMoving)
        {
            if (CrossPlatformInputManager.GetButton("Forward"))
            {
                // Move forward
                gameObject.tag = "Untagged";
                Move(Direction.FORWARD);
            }

            if (CrossPlatformInputManager.GetButton("Left"))
            {
                gameObject.tag = "Untagged";
                Move(Direction.LEFT);
            }

            if (CrossPlatformInputManager.GetButton("Right"))
            {
                gameObject.tag = "Untagged";
                Move(Direction.RIGHT);
            }

            if (CrossPlatformInputManager.GetButton("Backwards"))
            {
                gameObject.tag = "Untagged";
                Move(Direction.BACKWARDS);
            }
        }
        else
        {
            UpdatePosition();
        }
    }

    public override void UpdatePosition()
    {
        base.UpdatePosition();
        if(!bIsMoving)
        {
            gameObject.tag = "Player";
        }
    }

    private void HandleTurning()
    {
        // Testing is restricted if the player is already turning
        if (!bIsTurning)
        {
            if (CrossPlatformInputManager.GetButton("LeftTurn"))
            {
                // Turn Left
                Look(Direction.LEFT);
                bIsTurning = true;
            }
            else if (CrossPlatformInputManager.GetButton("RightTurn"))
            {
                // Turn Right
                Look(Direction.RIGHT);
                bIsTurning = true;
            }
        }
        else
        {
            UpdateCamera();
        }
    }

    private void UpdateCamera()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(v3DestRotation), fRotSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(v3DestRotation)) <= 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(v3DestRotation);
            bIsTurning = false;
        }
    }

    private void UpdateMinimap()
    {
        // Raycast to each tile within view
        // Toggle to render on Minimap too if not
    }

    private void Minimap()
    {
        if (CrossPlatformInputManager.GetButton("Minimap"))
        {
            mainCamera.SetActive(false);
            mapCamera.SetActive(true);

            for (int i = 0; i < 2; i++)
            {
                MiniImages[i].color = new Color(MiniImages[i].color.r, MiniImages[i].color.g, MiniImages[i].color.b, 0);
            }

            for (int i = 0; i < UIElements.Length; i++)
            {
                UIElements[i].SetActive(false);
            }


        }
        if (CrossPlatformInputManager.GetButtonUp("Minimap"))
        {
            mainCamera.SetActive(true);
            mapCamera.SetActive(false);

            for (int i = 0; i < 2; i++)
            {
                MiniImages[i].color = new Color(MiniImages[i].color.r, MiniImages[i].color.g, MiniImages[i].color.b, 1);
            }

            for (int i = 0; i < UIElements.Length; i++)
            {
                UIElements[i].SetActive(true);
            }
        }
    }
}
