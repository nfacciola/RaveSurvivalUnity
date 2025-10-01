using UnityEngine;
using System.Collections.Generic;
using System;

namespace RaveSurvival
{
    public class MusicConductor : MonoBehaviour
    {
        public static MusicConductor Instance;

        [Header("Track")]
        public AudioClip track;
        public bool playOnStart = true;

        [Header("Analysis")]
        public UnityEngine.Audio.AudioMixerGroup analysisMixer;
        public bool createAnalysisSource = true;
        public AudioAnalyzer analyzer;

        private readonly List<Speaker> speakers = new();
        private bool isPlaying = false;
        private int masterSamples = 0;

        // Expose timing to others
        public double DspStartTime { get; private set; }
        public event Action<double> OnSongStarted;
        public event Action OnSongStopped;


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
                //s.source.loop = true;
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
            DspStartTime = AudioSettings.dspTime + 0.05;
            foreach (Speaker s in speakers)
            {
                AudioSource src = s.source;
                src.clip = track;
                //src.loop = true;
                src.PlayScheduled(DspStartTime);
            }

            isPlaying = true;
            OnSongStarted?.Invoke(DspStartTime);
        }

        public void StopTrack()
        {
            if (!isPlaying)
            {
                return;
            }
            foreach (Speaker s in speakers)
            {
                s.source.Stop();
            }

            isPlaying = false;
            OnSongStopped?.Invoke();
        }

        public void SetTrack(AudioClip audioClip)
        {
            track = audioClip;
        }
    }
}