using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement;
using System.Collections;

namespace RaveSurvival
{
    public class SpawnManager : NetworkBehaviour
    {
        public static SpawnManager Instance = null;
        public GameObject enemyPrefab;
        public GameObject playerPrefab;
        public Transform spawnPointParent;
        public List<Spawn> playerSpawnPoints = new();
        public List<Spawn> enemySpawnPoints = new();
        public List<Spawn> bossSpawnPoints = new();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

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
                Transform child = spawnPointParent.GetChild(i);
                Spawn spawn = child.gameObject.GetComponent<Spawn>();
                if (spawn != null)
                {
                    if (spawn.GetSpawnUser() == Spawn.SpawnUser.player)
                    {
                        playerSpawnPoints.Add(spawn);
                    }
                    else if (spawn.GetSpawnUser() == Spawn.SpawnUser.enemy)
                    {
                        enemySpawnPoints.Add(spawn);
                    }
                    else if (spawn.GetSpawnUser() == Spawn.SpawnUser.boss)
                    {
                        bossSpawnPoints.Add(spawn);
                    }
                    else
                    {
                        Debug.LogError("invalid spawn user type");
                    }
                }
            }
            Debug.Log($"{playerSpawnPoints.Count} player spawn(s), {enemySpawnPoints.Count} enemy spawn(s), {bossSpawnPoints.Count} boss spawn(s)");
        }

        public void SpawnPlayers(GameManager.GameType gameType)
        {
            if (gameType == GameManager.GameType.SinglePlayer)
            {
                int rand = UnityEngine.Random.Range(0, playerSpawnPoints.Count - 1);
                GameObject[] temp = { playerPrefab };
                playerSpawnPoints[rand].SpawnCharacter(temp);
            }
        }

        public IEnumerator SpawnEnemies()
        {
            yield return new WaitForSeconds(5.0f);
            foreach (Spawn spawnPoint in enemySpawnPoints)
            {
                yield return new WaitForSeconds(2.0f);
                GameObject[] temp = { enemyPrefab };
                spawnPoint.SpawnCharacter(temp, 2.0f);
            }
        }
    }
}
