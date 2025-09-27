using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

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

        public void FindSpawnPoints()
        {
            GameObject sp = GameObject.FindGameObjectWithTag("SpawnPointParent");
            if (sp == null)
            {
                throw new InvalidOperationException("Spawn system requires a GameObject with tag 'SpawnPointParent' but none was found.");
            }
            spawnPointParent = sp.transform;
            for (int i = 0; i < spawnPointParent.childCount; i++)
            {
                spawnPoints.Add(spawnPointParent.GetChild(i).transform);
            }
        }

        public void SpawnPlayers(GameManager.GameType gameType)
        {
            if (gameType == GameManager.GameType.SinglePlayer)
            {
                Instantiate(playerPrefab);
            }
        }

        public void SpawnEnemies()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                if(GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
                    NetworkServer.Spawn(enemy);
            }
        }
    }
}
