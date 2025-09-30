using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaveSurvival
{
    public class GameBootstraper : MonoBehaviour
    {
        [SerializeField]
        GameObject gameplayManagersPrefab;
        private void Awake()
        {
            if (GameManager.Instance == null && SpawnManager.Instance == null)
            {
                Instantiate(gameplayManagersPrefab);
            }
        }
    }
}