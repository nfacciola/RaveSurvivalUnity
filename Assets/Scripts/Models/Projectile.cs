using UnityEngine;
using Mirror;
using RaveSurvival;

public class Projectile : NetworkBehaviour
{
	private float damage = 5.0f;
	private Rigidbody rb;
	[SerializeField]
	private float lifetime = 3f;

	void Awake()
	{
		if (GameManager.Instance == null)
		{
			Debug.LogError("Error... trying to instantiate project when game manager is null!");
			return;
		}

		rb = GetComponent<Rigidbody>();
	}

	public void FireBullet(float velocity)
	{
		if (rb != null)
		{
			rb.AddForce(transform.forward * velocity);
		}

		if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
		{
			//Handle destrcution server side
			if (isServer)
			{
				Invoke(nameof(DestroySelf), lifetime);
			}
		}
		else if (GameManager.Instance.gameType == GameManager.GameType.SinglePlayer)
		{
			Destroy(this.gameObject, lifetime);
		}
		
	}

	void DestroySelf()
	{
		NetworkServer.Destroy(this.gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (!isServer && GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
			return;

		if (other.gameObject.tag == "Player")
		{
			Player player = other.gameObject.GetComponent<Player>();
			if (player != null)
			{
				player.TakeDamage(damage, gameObject);
			}
		}

		if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
			NetworkServer.Destroy(this.gameObject);
		else
			Destroy(this.gameObject);
	}

	public void SetDamage(float dmg)
	{
		damage = dmg;
	}
}
