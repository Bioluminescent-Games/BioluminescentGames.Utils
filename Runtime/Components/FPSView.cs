using System.Collections.Generic;
using BioluminescentGames.Utils.Utilities;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using TMPro;
using UnityEngine;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

namespace BioluminescentGames.Utils.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class FPSView : BioluminescentBehaviour
    {
        [SerializeField] private float measureRateSeconds = 0.1f;
        
        private TMP_Text _text;

        private readonly List<float> _frameTimes = new ();

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            ExecuteRepeatingRealtime(() =>
            {
                float average = _frameTimes.Count <= 0 
                    ? Time.unscaledDeltaTime
                    : _frameTimes
#if ZLINQ
                        .AsValueEnumerable()
#endif
                        .Average();
                _text.text = $"FPS: {1f / average:0}";
                _frameTimes.Clear();
            }, measureRateSeconds, delaySeconds: measureRateSeconds);
        }

        public override void OnUpdate()
        {
            _frameTimes.Add(Time.unscaledDeltaTime);
        }
    }
}
