using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace RaveSurvival
{
    public class SpawnManager : NetworkBehaviour
    {
        public GameObject enemyPrefab;
        public Transform spawnPointParent;
        public List<Transform> spawnPoints = new List<Transform>();

        void Start()
        {
            for (int i = 0; i < spawnPointParent.childCount; i++)
            {
                spawnPoints.Add(spawnPointParent.GetChild(i).transform);
            }
            if (isServer)
            {
                SpawnEnemies();
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
