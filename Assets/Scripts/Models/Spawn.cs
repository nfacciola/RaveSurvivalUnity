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

    public void SpawnCharacters(GameObject[] entities, float delay = 0.0f, float spwnRate = 0.0f)
    {
        IEnumerator spawn = SpawnEntities(entities, delay, spwnRate);
        StartCoroutine(spawn);
    }

    public SpawnUser GetSpawnUser()
    {
        return spawnUser;
    }

    private IEnumerator SpawnEntities(GameObject[] entities, float delay, float spwnRate)
    {
        // wait delay
        yield return new WaitForSeconds(delay);

        foreach (GameObject entity in entities)
        {
            SpawnEntity(entity);
            yield return new WaitForSeconds(spwnRate);
        }
    }

    private void SpawnEntity(GameObject entity)
    {
        GameObject character = Instantiate(entity, transform.position, transform.rotation);
        Debug.Log("Created entity: " + character.name);
        if (GameManager.Instance.gameType == GameManager.GameType.OnlineMultiplayer)
        {
            NetworkServer.Spawn(character);
        }
    }
}