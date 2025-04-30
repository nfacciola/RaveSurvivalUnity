using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections.Generic;

namespace RaveSurvival 
{
	public class GameManager: NetworkBehaviour 
	{
		public static GameManager instance = null;
		public List<Player> players;

        void Awake()
        {
            if(instance == null)
			{
				instance = this;
			}
			else
			{
				Destroy(this.gameObject);
			}
		}

		public void SetPlayerList()
		{
			players = new List<Player>(FindObjectsByType<Player>(FindObjectsSortMode.None));
		}
        public void SetLocalCamera()
		{
			foreach(Player player in players)
			{
				player.gameObject.transform.GetChild(1).GetComponent<Camera>().enabled = isLocalPlayer;
			}
		}
    }
}