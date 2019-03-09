using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Character : MonoBehaviour
{
    // Audio
    protected AudioSource audioSource;

    // Movement
    [SerializeField] protected float fSpeed = 12;
    [SerializeField] protected float fRotSpeed = 12;

    // Combat
    [SerializeField] private int iMaxHealth = 100;
    private int iCurrentHealth = 100;
    private int iTempHealth = 0;

    [SerializeField] private int iDamage = 10;

    [SerializeField] private float fMeleeCooldown = 1;
    private float fMeleeTimer = 0;

    [SerializeField] private float fMagicCooldown = 5;
    private float fMagicTimer = 0;

    [SerializeField] private GameObject baseSpell;

    // State control
    protected bool bIsFreeLooking = false;
    protected bool bIsMoving = false;
    protected bool bIsTurning = false;
    protected bool bIsAttacking = false;
    protected bool bGameActive = true;
    protected Direction travelDirection;
    protected Vector3 v3Destination;
    protected Vector3 v3DestRotation;

    protected virtual void Awake()
    {
        CS_GameManager.AddCharacter(this);
        audioSource = GetComponent<AudioSource>();
    }


    public void Look(Direction dir)
    {
        switch (dir)
        {
            case Direction.BACKWARDS:
                {
                    v3DestRotation = -transform.forward;
                    break;
                }
            case Direction.LEFT:
                {
                    v3DestRotation = -transform.right;
                    break;
                }
            case Direction.RIGHT:
                {
                    v3DestRotation = transform.right;
                    break;
                }
        }

    }

    public void Move(Direction dir)
    {
        switch (dir)
        {
            case Direction.FORWARD:
                {
                    CalculateMove(transform.forward);
                    break;
                }
            case Direction.LEFT:
                {
                    CalculateMove(-transform.right);
                    break;
                }
            case Direction.RIGHT:
                {
                    CalculateMove(transform.right);
                    break;
                }
            case Direction.BACKWARDS:
                {
                    CalculateMove(-transform.forward);
                    break;
                }

        }

    }

    public void SetDamage(int a_newDamage)
    {
        iDamage = a_newDamage;
    }

    public void SetHealth(int a_newHealth)
    {
        iCurrentHealth = a_newHealth;
    }

    public void FullHeal()
    {
        iCurrentHealth = iMaxHealth;
    }

    public virtual void TakeDamage(int a_amount)
    {
        iCurrentHealth = iCurrentHealth - a_amount;

        if(iCurrentHealth <= 0)
        {
            Die();
        }
    }

    public void MeleeAttack()
    {
        if(fMeleeTimer > 0)
        {
            return;
        }

        fMeleeTimer = fMeleeCooldown;
        RaycastHit outHit;
        if(Physics.Raycast(transform.position, transform.forward, out outHit, CS_GameManager.fGridCellSize))
        {
            CS_Character other = outHit.transform.gameObject.GetComponent<CS_Character>();
            if (other != null)
            {
                Debug.Log("Agent: " + this.gameObject.name + " deals " + iDamage + " damage to " + other.gameObject.name);
                other.TakeDamage(iDamage);
            }
        }
    }

    public bool MagicAttack()
    {
        if (fMagicTimer > 0)
        {
            return false;
        }
        fMagicTimer = fMagicCooldown;
        GameObject spell = Instantiate(baseSpell, transform.position, transform.rotation);
        spell.GetComponent<CS_Projectile>().SetCaster(this);
        spell.GetComponent<CS_Projectile>().SetDamage(iDamage);
        spell.GetComponent<CS_Projectile>().Launch();

        return true;
    }

    public int GetHealth()
    {
        return iCurrentHealth;
    }

    protected virtual void Die()
    {

    }

    public void SetDestination(Vector3 a_Dest)
    {
        if(a_Dest.x < 0 || a_Dest.z < 0)
        {
            Debug.Log("FAIL");
        }
        if(CS_GameManager.TakeSpace(a_Dest))
        {
            v3Destination = a_Dest;
            CS_GameManager.ClearSpace(transform.position);
            bIsMoving = true;
        }
    }

    public Vector3 GetDestination()
    {
        return v3Destination;
    }

    protected void CalculateMove(Vector3 a_axisToMove)
    {
        Vector3 dest = transform.position + (new Vector3(Mathf.RoundToInt(a_axisToMove.x), Mathf.RoundToInt(a_axisToMove.y), Mathf.RoundToInt(a_axisToMove.z)) * CS_GameManager.fGridCellSize);

        if (!CS_GameManager.IsClear(dest))
        {
            return;
        }

        SetDestination(dest);
    }

    public void OnGameStart()
    {
        bGameActive = true;
    }

    public void OnGameEnd()
    {
        bGameActive = false;
        bIsMoving = false;
        bIsTurning = false;
        bIsFreeLooking = false;
    }

    public bool IsMoving()
    {
        return bIsMoving;
    }

    public void UpdateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(v3Destination - transform.position), Time.deltaTime * fSpeed);
    }

    public virtual void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, v3Destination, Time.deltaTime * fSpeed);
        if (Vector3.Distance(transform.position, v3Destination) <= 0.05f)
        {
            transform.position = new Vector3(Mathf.RoundToInt(v3Destination.x), transform.position.y, Mathf.RoundToInt(v3Destination.z));
            bIsMoving = false;
        }
    }

    public virtual bool CanMelee()
    {
        if (fMeleeTimer > 0)
        {
            return false;
        }
        return true;
    }

    public bool CanMagic()
    {
        if (fMagicTimer > 0)
        {
            return false;
        }
        return true;
    }

    public void UpdateAttacking()
    {
        if (fMeleeTimer > 0)
        {
            fMeleeTimer -= Time.deltaTime;
        }

        if (fMagicTimer > 0)
        {
            fMagicTimer -= Time.deltaTime;
        }
    }
}
