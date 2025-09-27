using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace RaveSurvival
{
    public class SpawnManager : NetworkBehaviour
    {
        public static SpawnManager Instance = null;
        public GameObject enemyPrefab;
        public GameObject playerPrefab;
        public Transform spawnPointParent;
        public List<Transform> spawnPoints = new List<Transform>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        // void Start()
        // {
        //     for (int i = 0; i < spawnPointParent.childCount; i++)
        //     {
        //         spawnPoints.Add(spawnPointParent.GetChild(i).transform);
        //     }
        //     if (isServer)
        //     {
        //         SpawnEnemies();
        //     }
        // }

        public void SpawnPlayers(GameManager.GameType gameType)
        {
            if (gameType == GameManager.GameType.SinglePlayer)
            {
                Instantiate(playerPrefab);
            }
        }

        void SpawnEnemies()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                NetworkServer.Spawn(enemy);
            }
        }
    }
}
