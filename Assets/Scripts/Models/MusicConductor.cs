using UnityEngine;
using System.Collections.Generic;

namespace RaveSurvival
{
    public class MusicConductor : MonoBehaviour
    {
        public static MusicConductor Instance;

        [Header("Track")]
        public AudioClip track;
        public bool playOnStart = true;

        [SerializeField]
        private List<Speaker> speakers = new();

        private bool isPlaying = false;
        private int masterSamples = 0;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            if (track != null && playOnStart)
            {
                StartTrack();
            }
        }

        public void Update()
        {
            if (!isPlaying || track == null)
            {
                return;
            }

            //Track master samples from first speaker to sync late joiners
            AudioSource first = speakers.Count > 0 ? speakers[0].source : null;
            if (first != null && first.isPlaying)
            {
                masterSamples = first.timeSamples;
            }
        }

        public void Register(Speaker s)
        {
            if (!speakers.Contains(s))
            {
                speakers.Add(s);
            }
            if (track != null && playOnStart && !isPlaying)
            {
                StartTrack();
            }
            else if (isPlaying && track != null)
            {
                s.source.clip = track;
                s.source.timeSamples = masterSamples;
                s.source.Play();
            }
        }

        public void Unregister(Speaker s) => speakers.Remove(s);

        public void StartTrack()
        {
            if (isPlaying || track == null || speakers.Count == 0)
            {
                return;
            }
            double startDsp = AudioSettings.dspTime + 0.05;
            foreach (Speaker s in speakers)
            {
                AudioSource src = s.source;
                src.clip = track;
                src.loop = true;
                src.PlayScheduled(startDsp);
            }
            isPlaying = true;
        }

        public void SetTrack(AudioClip audioClip)
        {
            track = audioClip;
        }
    }
}