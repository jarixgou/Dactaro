using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnnemiesAI : MonoBehaviour
{
    [Range(0f, 50f)] public float radiusCheck;
    [Range(0f, 50f)] public float radiusInRange;
    [Range(0f, 25f)] public float speed;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask mask;
    public GameObject target;
    public RaycastHit2D behindWall;
    private GameObject spwanPointOfEnnemies;

    private void Start()
    {
        spwanPointOfEnnemies = new GameObject("spwanPointOfEnnemies");
        spwanPointOfEnnemies.transform.position = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        behindWall = Physics2D.Raycast(transform.position, (target.transform.position - transform.position).normalized, mask);
        Debug.LogWarning(behindWall.collider);
        
        if (PlayerInZone() && !behindWall.collider.CompareTag("Wall"))
        {  
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, spwanPointOfEnnemies.transform.position, speed * Time.deltaTime);
        }
    }

    public bool PlayerInZone()
    {
        return Physics2D.OverlapCircle(transform.position, radiusCheck, playerLayer);
    }

    public bool InRangeAttack()
    {
        return Physics2D.OverlapCircle(transform.position, radiusInRange, playerLayer);
    }

    private void OnDrawGizmos()
    {
        if (PlayerInZone())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusCheck);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radiusCheck);
        }

        Gizmos.DrawWireSphere(transform.position, radiusInRange);
        
        Gizmos.DrawRay(transform.position, (Vector2)target.transform.position - (Vector2)transform.position);
        Gizmos.DrawWireCube(spwanPointOfEnnemies.transform.position, new Vector3(1,1));
    }
    
}
