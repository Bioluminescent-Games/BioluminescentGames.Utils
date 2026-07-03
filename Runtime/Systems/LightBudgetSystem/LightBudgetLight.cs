#if URP
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BioluminescentGames.Utils.Runtime
{
    [RequireComponent(typeof(UniversalAdditionalLightData), typeof(Light))]
    public class LightBudgetLight : MonoBehaviour
    {
        internal UniversalAdditionalLightData AdditionalLightData { get; private set; }
        internal Light Light { get; private set; }

        private void Awake()
        {
            AdditionalLightData = GetComponent<UniversalAdditionalLightData>();
            Light = GetComponent<Light>();
        }

        private void OnEnable()
        {
            LightBudgetManager.Instance.RegisterLight(this);
        }

        private void OnDisable()
        {
            if (LightBudgetManager.HasInstance)
                LightBudgetManager.Instance.DeregisterLight(this);
        }
    }
}
#endif
