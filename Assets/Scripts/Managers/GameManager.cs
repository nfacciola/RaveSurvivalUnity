using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace RaveSurvival {
  public class GameManager: NetworkBehaviour {
    void Start() {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.InstanceID);

        foreach(Camera cam in cameras) {
          
        }
      }
  }
}