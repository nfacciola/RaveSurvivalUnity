using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace RaveSurvival
{
	public class Enemy : NetworkBehaviour
	{
		public float health = 50f;
		public float range = 10f;
		private NavMeshAgent agent;
		private Transform target = null;
		private IEnumerator behaviorCo = null;
		public EnemyState enemyState = EnemyState.IDLE;
		public Gun gun;

		public enum EnemyState
		{
			IDLE,
			WANDER,
			CHASE,
			ATTACK,
			DEAD
		};

		public void Start()
		{
			enemyState = EnemyState.IDLE;
			agent = GetComponent<NavMeshAgent>();
		}

		public void PlayerSpotted(Transform player)
		{
			if (target != player && behaviorCo == null)
			{
				target = player;
				behaviorCo = AttackPlayer(target);
				StartCoroutine(behaviorCo);
			}
		}

		public IEnumerator AttackPlayer(Transform player)
		{
			float wait = 0.25f;
			while (true)
			{
				if (Vector3.Distance(player.position, transform.position) > range)
				{
					MoveToPlayer(player);
				}
				else
				{
					ShootPlayer(player);
				}
				yield return wait;
			}
		}
		public void NoPlayerFound()
		{
			if (behaviorCo != null)
			{
				IEnumerator delay = DelayedStop(behaviorCo, 2f);
				StartCoroutine(delay);
				behaviorCo = null;
				target = null;
			}
		}

		private void MoveToPlayer(Transform player)
		{
			enemyState = EnemyState.CHASE;
			transform.LookAt(player);
			agent.SetDestination(player.position);
		}

		private void ShootPlayer(Transform player)
		{
			enemyState = EnemyState.ATTACK;
			agent.ResetPath();
			transform.LookAt(player);
			if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
			{
				gun.OnlineShoot(true);
			}
			else
			{
				gun.SinglePlayerShoot(true);
			}
		}

		public void TakeDamage(float dmg, Transform bulletDirection)
		{
			transform.LookAt(bulletDirection);
			health -= dmg;
			if (health <= 0f)
			{
				Die();
			}
		}

		void Die()
		{
			Destroy(gameObject);
		}

		private IEnumerator DelayedStop(IEnumerator co, float seconds)
		{
			yield return new WaitForSeconds(seconds);
			StopCoroutine(co);
			enemyState = EnemyState.IDLE;
		}
	}
}

