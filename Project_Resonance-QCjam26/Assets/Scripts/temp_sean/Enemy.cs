using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Rendering.Universal.Internal;

public class Enemy : MonoBehaviour
{
    [SerializeField] int damage, health, speed;
    [SerializeField] float visionRange, attackRange;

    private NavMeshAgent navMeshAgent;
    private enum state{attack,chase,idle,dead};
    state currentState;
    bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        currentState = state.idle;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState){
            case state.idle:
            navMeshAgent.SetDestination(transform.position);
                if(Vector3.Distance(transform.position,PlayerMovement3D.Instance.transform.position)<visionRange){
                    currentState = state.chase;
                }
                break;
            case state.chase:
                navMeshAgent.SetDestination(PlayerMovement3D.Instance.transform.position);
                navMeshAgent.speed=speed;
                if(Vector3.Distance(transform.position,PlayerMovement3D.Instance.transform.position)>visionRange*1.5f){
                    currentState = state.idle;
                }else if(Vector3.Distance(transform.position,PlayerMovement3D.Instance.transform.position)<=attackRange){
                    currentState = state.attack;
                }
                break;
            case state.attack:
                Quaternion quaternion = Quaternion.LookRotation(PlayerMovement3D.Instance.transform.position-transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion,1);
                if(!isAttacking){
                    isAttacking = true;
                    StartCoroutine(Attack());
                }
                break;
            case state.dead:
                break;
        }
    }

    IEnumerator Attack(){
        navMeshAgent.SetDestination(transform.position);
        //Change this number to the length of the animation!
        yield return new WaitForSeconds(1);
        isAttacking = false;
        if(Vector3.Distance(transform.position,PlayerMovement3D.Instance.transform.position)<=attackRange+.1f){
            PlayerMovement3D.Instance.hit(damage);
        }else{
            currentState = state.chase;
        }
    }

    IEnumerator Die()
    {
        currentState = state.dead;
        navMeshAgent.SetDestination(transform.position);
        Destroy(this.GetComponent<Collider>());
        //Change this number to the length of the animation!
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            collision.gameObject.GetComponent<PlayerMovement3D>().hit(damage);
        }
    }

    public void hit(int damage)
    {
        health-=damage;
        if(health<=0){
            StartCoroutine(Die());
        }
    }
}
