using UnityEngine;

namespace RaveSurvival
{
    [RequireComponent(typeof(Light))]
    public class LightReactive : MonoBehaviour
    {
        public AudioAnalyzer analyzer;
        [Range(0,7)] public int band = 3;
        public float minIntensity = 0.2f;
        public float maxIntensity = 8f;
        public Gradient colorByLevel;

        private Light _light;

        void Awake()
        {
            _light = GetComponent<Light>();
        }

        void Start()
        {
            if (MusicConductor.Instance != null)
            {
                analyzer = MusicConductor.Instance.analyzer;
            }
        }

        void Update()
        {
            if (!analyzer)
            {
                return;
            }
            float v = analyzer.GetBand(band);
            print("band is " + v.ToString());
            _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, v);
            if (colorByLevel != null)
            {
                _light.color = colorByLevel.Evaluate(v);
            }
        }
    }
}