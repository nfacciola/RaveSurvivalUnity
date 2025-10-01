using UnityEngine;

namespace RaveSurvival
{
    public class AudioAnalyzer : MonoBehaviour
    {
        [Range(256, 4096)] public int fftSize = 1024;
        public FFTWindow window = FFTWindow.BlackmanHarris;

        [Header("Shaping")]
        [Range(1f, 100f)] public float gain = 20f;
        [Range(1f, 60f)] public float attackHz = 25f;
        [Range(1f, 60f)] public float releaseHz = 8f;

        public float[] bands = new float[8];

        AudioSource source;
        float[] spec;
        float[] smooth;

        void Awake()
        {
            spec   = new float[fftSize];
            smooth = new float[bands.Length];
        }

        void Start()
        {
            MusicConductor mc = MusicConductor.Instance;
            if (mc)
            {
                source = mc.AnalysisSource;
            }
        }

        void Update()
        {
            // Fill spectrum
            if (source && source.isPlaying)
            {
                source.GetSpectrumData(spec, 0, window);
            }
            else
            {
                AudioListener.GetSpectrumData(spec, 0, window);
            }

            int bin = 0;
            for (int b = 0; b < bands.Length; b++)
            {
                int binsThis = Mathf.Clamp((int)Mathf.Pow(2, b) * (fftSize / 512), 1, spec.Length - bin);
                float maxv = 0f;
                for (int i = 0; i < binsThis; i++, bin++)
                    if (spec[bin] > maxv) maxv = spec[bin];

                // shape → 0..1
                float v = Mathf.Clamp01(maxv * gain);

                // attack/release smoothing
                float cur = smooth[b];
                float rate = (v > cur ? attackHz : releaseHz) * Time.deltaTime;
                smooth[b] = Mathf.Lerp(cur, v, Mathf.Clamp01(rate));

                bands[b] = smooth[b];
            }
        }

        public float GetBand(int i) => bands[Mathf.Clamp(i, 0, bands.Length - 1)];
    }
}
