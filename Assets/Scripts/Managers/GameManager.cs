using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections.Generic;

namespace RaveSurvival 
{
	public class GameManager: NetworkBehaviour 
	{
		public List<Player> players;

        void Start()
        {
            players = new List<Player>(FindObjectsByType<Player>(FindObjectsSortMode.None));
			SetLocalCamera();
        }

		void SetLocalCamera()
		{
			foreach(Player player in players)
			{
				player.gameObject.transform.GetChild(1).GetComponent<Camera>().enabled = isLocalPlayer;
			}
		}
    }
}