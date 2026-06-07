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
        private TMP_Text _text;

        private readonly List<float> _frameTimes = new ();

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();

            TimeUtils.Instance.ExecuteRepeating(() =>
            {
                float average = _frameTimes
#if ZLINQ
                    .AsValueEnumerable()
#endif
                    .Average();
                _text.text = $"FPS: {1f / average:0}";
                _frameTimes.Clear();
            }, 0.1f, () => true);
        }

        public override void OnUpdate()
        {
            _frameTimes.Add(Time.unscaledDeltaTime);
        }
    }
}
