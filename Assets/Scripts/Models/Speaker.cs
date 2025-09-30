using UnityEngine;

namespace RaveSurvival
{
    [RequireComponent(typeof(AudioSource))]
    public class Speaker : MonoBehaviour
    {
        public AudioSource source;

        [Header("Occlusion")]
        public bool enableOcclusion = true;
        public LayerMask occluders = ~0; // Not 0 (so everything by default)
        public float occludedVolume = 0.55f;
        public int occlusionLowpassCutoff = 800;  // Hz
        public int clearLowpassCutoff = 22000;    // Hz
        public float occlusionCheckInterval = 0.1f;

        private AudioLowPassFilter lowPassFilter;
        private float timer;

        void Awake()
        {
            source = GetComponent<AudioSource>();
            source.spatialBlend = 1f;
            source.dopplerLevel = 0f;
            lowPassFilter = GetComponent<AudioLowPassFilter>();
        }

        void Update()
        {
            if (!enableOcclusion || lowPassFilter == null)
            {
                return;
            }

            timer += Time.deltaTime;
            if (timer < occlusionCheckInterval)
            {
                return;
            }
            timer = 0f;

            AudioListener listener = FindAnyObjectByType<AudioListener>();
            if (listener == null)
            {
                return;
            }

            Vector3 listenerPos = listener.transform.position;
            Vector3 dir = transform.position - listenerPos;
            float dist = dir.magnitude;

            bool isBlocked = Physics.Raycast(listenerPos, dir.normalized, dist, occluders, QueryTriggerInteraction.Ignore);

            lowPassFilter.cutoffFrequency = isBlocked ? occlusionLowpassCutoff : clearLowpassCutoff;
            source.volume = isBlocked ? occludedVolume : 1f;

        }
    }
}