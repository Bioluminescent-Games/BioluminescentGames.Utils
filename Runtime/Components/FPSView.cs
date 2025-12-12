using System.Collections.Generic;
using System.Linq;
using BioluminescentGames.Utils;
using TMPro;
using UnityEngine;

namespace BioluminescentGames.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class FPSView : MonoBehaviour
    {
        private TMP_Text _text;
        
        private readonly List<float> _frameTimes = new ();
        
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            
            TimeUtils.Instance.ExecuteRepeating(() =>
            {
                float average = _frameTimes.Aggregate(0.0f, (current, f) => current += f) / _frameTimes.Count;
                _text.text = $"FPS: {1f / average:0}";
                _frameTimes.Clear();
            }, 0.1f, () => true);
        }

        private void Update()
        {
            _frameTimes.Add(Time.unscaledDeltaTime);
        }
    }
}
