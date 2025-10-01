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

        // Analysis source kept in perfect sync
        [SerializeField]
        private AudioSource analysisSource;
        public AudioSource AnalysisSource => analysisSource;

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

            if (createAnalysisSource)
            {
                var go = new GameObject("Music_AnalysisSource");
                go.transform.SetParent(transform, false);
                analysisSource = go.AddComponent<AudioSource>();
                analysisSource.playOnAwake = false;
                //analysisSource.loop = true;
                analysisSource.spatialBlend = 0f; // 2D
                analysisSource.dopplerLevel = 0f;
                if (analysisMixer)
                {
                    analysisSource.outputAudioMixerGroup = analysisMixer;
                }
                analysisSource.volume = 0f;
            }
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

            // Use analysisSource for master time (stable no matter where player stands)
            AudioSource first = analysisSource != null ? analysisSource : (speakers.Count > 0 ? speakers[0].source : null);
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
            if (analysisSource != null)
            {
                analysisSource.clip = track;
                //analysisSource.loop = true;
                analysisSource.PlayScheduled(DspStartTime);
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
            if (analysisSource)
            {
                analysisSource.Stop();
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