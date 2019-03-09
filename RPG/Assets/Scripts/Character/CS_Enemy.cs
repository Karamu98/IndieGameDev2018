using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy : CS_Character
{
    // Audio
    [SerializeField] AudioClip EnemyDie;

    // Static Vars
    public static List<CS_Enemy> agentList = new List<CS_Enemy>();


    // Member Vars
    private GameObject player;
    public Vector3 playerLastPos;
    private bool bCanSeePlayer = false;
    private bool bPursuingPlayer = false;
    private byte halffov = 55;

    

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetPursue(bool a_newstate)
    {
        bPursuingPlayer = a_newstate;
    }

    protected virtual void Start()
    {
        // On start add ourselves to the brain list
        agentList.Add(this);
        player = CS_GameManager.player.gameObject;
        audioSource.clip = EnemyDie;
    }

    protected override void Die()
    {
        base.Die();

        audioSource.Play();
        CS_GameManager.CharacterKilled(this);
        CS_GameManager.ClearSpace(transform.position);
        CS_GameManager.ClearSpace(v3Destination);
        Destroy(gameObject);
    }

    protected virtual void Update()
    {
        //DrawFOV();
        UpdateAttacking();
        //CanSeePlayer();
    }

    void DrawFOV()
    {
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halffov, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halffov, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Debug.DrawRay(transform.position, leftRayDirection * 4000, Color.red);
        Debug.DrawRay(transform.position, rightRayDirection * 4000, Color.red);
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    public override bool CanMelee()
    {
        if(!base.CanMelee())
        {
            return false;
        }

        RaycastHit outHit;
        if (Physics.Raycast(transform.position, transform.forward, out outHit, CS_GameManager.fGridCellSize))
        {
            CS_Character other = outHit.transform.gameObject.GetComponent<CS_Character>();
            if (other == null)
            {
                return false;
            }
            if (other.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    public bool CanSeePlayer()
    {
        Vector3 line = Vector3.Normalize(player.transform.position - transform.position);

        float degree;

        if(line == transform.forward)
        {
            degree = 0;
        }
        else
        {
            // Finding the angle to the player
            degree = Vector3.Dot(line, transform.forward);
            degree = Mathf.Acos(degree);
            degree = degree * Mathf.Rad2Deg;
        }

        


        // If the player is within enemy FOV
        if (degree < halffov)
        {
            RaycastHit outHit;
            Debug.DrawRay(transform.position, line * 500);
            if (Physics.Raycast(transform.position, line, out outHit, 500))
            {
                if (outHit.transform.gameObject == player)
                {
                    playerLastPos = player.transform.position;
                    bCanSeePlayer = true;
                    return true;
                }
            }
        }

        bCanSeePlayer = false;
        return false;
    }

    public bool isInPursuit()
    {
        if(playerLastPos == transform.position)
        {
            bPursuingPlayer = false;
            return false;
        }

        if(bPursuingPlayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}