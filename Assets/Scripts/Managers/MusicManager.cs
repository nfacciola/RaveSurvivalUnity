using System.Collections.Generic;
using UnityEngine;

namespace RaveSurvival
{
    public class MusicManager : MonoBehaviour
    {

        public static MusicManager Instance = null;
        public List<AudioClip> tracks;
        public MusicConductor musicConductor;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void SetSong(AudioClip clip)
        {
            if (MusicConductor.Instance != null)
            {
                MusicConductor.Instance.SetTrack(clip);
            }
        }

        public void FindSpeakers()
        {
            if (MusicConductor.Instance != null)
            {
                Speaker[] speakers = FindObjectsByType<Speaker>(FindObjectsSortMode.None);
                foreach (Speaker s in speakers)
                {
                    MusicConductor.Instance.Register(s);
                }
            }
        }
    }
}