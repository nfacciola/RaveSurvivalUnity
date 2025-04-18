using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public NavMeshAgent agent;
    public ArrayList playerTansforms = new ArrayList();
    public Transform playerPos;

  public void Start()
  {
    agent = GetComponent<NavMeshAgent>();
    var playerMoves = FindObjectsByType<PlayerMove>(FindObjectsSortMode.None);
    foreach(PlayerMove player in playerMoves) {
      var temp = player.GetComponentInParent<Transform>();
      if (temp != null) {
        playerTansforms.Add(temp);
      }
    }
  }

  public void Update()
  {
    float min = 99999999;
    foreach(Transform player in playerTansforms) {
      float dist = Vector3.Distance(player.transform.position, GetComponent<Transform>().transform.position);
      if(dist < min) {
        min = dist;
        playerPos = player;
      }
    }
    MoveToPlayer();
  }

  void MoveToPlayer() {
    if(playerPos != null) {
      //agent.CalculatePath(playerPos.transform.position, new NavMeshPath());
      transform.LookAt(playerPos);
      agent.SetDestination(playerPos.position);
    }
    
  }

  public void TakeDamage(float dmg) {
      health -= dmg;
      if(health <= 0f) {
        Die();
      }
    }

    void Die() {
      Destroy(gameObject);
    }
}
