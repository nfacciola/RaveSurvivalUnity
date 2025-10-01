using UnityEngine;
using UnityEngine.Audio;
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
        public AudioMixerGroup musicMixerGroup;  
        public AudioAnalyzer analyzer;

        [SerializeField]
        private AudioSource analysisSource;
        private readonly List<Speaker> speakers = new();
        private bool isPlaying = false;
        //private int masterSamples = 0;

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

            // Build analysis source
            analysisSource = gameObject.AddComponent<AudioSource>();
            analysisSource.outputAudioMixerGroup = musicMixerGroup;
            //analysisSource.loop = true;
            analysisSource.playOnAwake = false;
            // 2D sound
            analysisSource.spatialBlend = 0f; 
            analysisSource.volume = 1f;
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

            analysisSource.clip = track;
            analysisSource.PlayScheduled(DspStartTime);

            isPlaying = true;
        }

        public AudioSource GetAnalysisSource() => analysisSource;

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
        }

        public void SetTrack(AudioClip audioClip)
        {
            track = audioClip;
        }
    }
}