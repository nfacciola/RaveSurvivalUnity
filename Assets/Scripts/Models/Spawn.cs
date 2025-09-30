using UnityEngine;
using RaveSurvival;
using System.Collections;
using Mirror;

public class Spawn : MonoBehaviour
{
    public enum SpawnType
    {
        spawnPoint = 0,
        spawnArea
    }

    public enum SpawnUser
    {
        player = 0,
        enemy,
        boss
    }

    public SpawnType spawnType = SpawnType.spawnPoint;
    public SpawnUser spawnUser = SpawnUser.enemy;
    public float radius = 0f;

    public void SpawnCharacter(GameObject[] entities, float delay = 0.0f)
    {
        IEnumerator spawn = SpawnEntity(entities, delay);
        StartCoroutine(spawn);
    }

    public SpawnUser GetSpawnUser()
    {
        return spawnUser;
    }

    IEnumerator SpawnEntity(GameObject[] entities, float delay)
    {
        foreach (GameObject entity in entities)
        {
            yield return new WaitForSeconds(delay);
            Temp(entity);
        }
    }

    private void Temp(GameObject entity)
    {
        GameObject character = Instantiate(entity, transform.position, transform.rotation);
        if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
        {
            NetworkServer.Spawn(character);
        }
    }
}