using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaveSurvival 
{
  public class MenuManager : MonoBehaviour
  {

    public static MenuManager Instance;

    void Awake()
    {
      if (Instance == null)
      {
        Instance = this; // Assign this instance as the singleton
      }
      else if (Instance != this)
      {
        Destroy(gameObject); // Destroy duplicate instances
        return;
      }
    }

    public void OnSinglePlayerClicked()
    {
      SceneManager.LoadScene("DubstepDungeonNF");
    }

    public void OnExit()
    {
      Application.Quit();
    }
  }
}
