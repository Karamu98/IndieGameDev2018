using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Projectile : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] private AudioClip collision;
    [SerializeField] private float fSpeed = 0.5f;
    [SerializeField] private int iDamage;
    [SerializeField] GameObject sphere;
    private bool bLaunch = false;
    private bool bCollision;
    private float maxInt;
    private bool bDying;

    CS_Character caster;

    public void SetCaster(CS_Character owner)
    {
        caster = owner;
    }

    public void Launch()
    {
        source = GetComponent<AudioSource>();
        source.clip = collision;
        bLaunch = true;
        GetComponent<SphereCollider>().enabled = true;
    }

    public void SetDamage(int a_damage)
    {
        iDamage = a_damage;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject == caster.gameObject)
        {
            return;
        }

        source.Play();
        bCollision = true;
        Destroy(GetComponent<CS_Flicker>());
        Destroy(GetComponent<SphereCollider>());
        Destroy(sphere);
        CS_Character oth = other.gameObject.GetComponent<CS_Character>();
        if (oth != null)
        {
            oth.TakeDamage(Mathf.RoundToInt(iDamage * 0.5f));
        }

        

        maxInt = GetComponent<Light>().intensity + 1;
    }

    private void Update()
    {
        if(!bLaunch)
        {
            return;
        }

        if(bCollision)
        {
            if(bDying)
            {
                if(GetComponent<Light>().intensity <= 0.2f)
                {
                    Destroy(gameObject);
                }

                GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, 0, Time.deltaTime * 10);
            }
            else
            {
                if(GetComponent<Light>().intensity >= maxInt - 0.2)
                {
                    bDying = true;
                }

                GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, maxInt, Time.deltaTime * 7);
            }
        }
        else
        {
            transform.position = transform.position + (transform.forward * fSpeed);
        }
    }



}
