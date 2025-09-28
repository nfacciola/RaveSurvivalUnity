// using UnityEngine;

// namespace RaveSurvival
// {
//     public class Spawn : MonoBehaviour
//     {
//         public enum SpawnType
//         {
//             spawnPoint = 0,
//             spawnArea
//         }

//         public enum SpawnUser
//         {
//             player = 0,
//             enemy,
//             boss
//         }

//         public SpawnType spawnType = SpawnType.spawnPoint;
//         public SpawnUser spawnUser = SpawnUser.enemy;
//         public float radius = 0f;

//         public void Spawn(Gameobject[] entities, float delay, float rate)
//         {
//             if (spawnType == SpawnType.spawnPoint)
//             {

//             }


//         }
//         public IEnumerator SpawnEntities(Gameobject[] entities, float delay, float rate)
//         {
//             // wait delay
//             yield return new WaitForSeconds(delay);

//             foreach (Gameobject entity in entities)
//             {
//                 SpawnEntity();
//                 yield return new WaitForSeconds(rate);
//             }
//             StopCoroutine();
//         }

//         private void SpawnEntity(GameObject entity)
//         {

//         }
//     }
// }