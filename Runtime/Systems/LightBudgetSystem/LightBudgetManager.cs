#if URP
using System;
using System.Collections.Generic;
using System.Reflection;
using BioluminescentGames.Utils.Core;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
#if EDITOR_ATTRIBUTES
using EditorAttributes;
#endif
using UnityEngine;
using BioluminescentGames.Utils.Utilities;
using UnityEngine.Profiling;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif
using Random = UnityEngine.Random;

namespace BioluminescentGames.Utils.Runtime
{
    public class LightBudgetManager : MonoSingleton<LightBudgetManager>
    {
        private enum Resolution
        {
            Low = 0,
            Medium = 1,
            High = 2,
        }
        
        [Serializable]
        private struct ShadowTier
        {
            public Resolution resolution;
            public LightShadows shadowType;
            public int amount;
        }
        
        [Header("Light budget")]
#if EDITOR_ATTRIBUTES
        private bool ShouldShowWarning {
            get
            {
                int amountOfShadowLights = shadowTiers
#if ZLINQ
                    .AsValueEnumerable()
#endif
                    .Sum(t => t.amount);

                return maxEnabledLights < amountOfShadowLights;
            }
        }
        [MessageBox("There are fewer max enabled lights than the total amount of lights with shadows", nameof(ShouldShowWarning), MessageMode.Warning)]
#endif
        [SerializeField] private int maxEnabledLights = 64;
        [SerializeField] private ShadowTier[] shadowTiers = Array.Empty<ShadowTier>();

        [Header("Update rate")]
        [Tooltip("How fast the light budget updates which lights should be enabled (in seconds)")]
        [SerializeField] private float enabledUpdateRateSeconds = 0.25f;
        
        [Tooltip("How fast the light budget updates which lights should have which shadow tiers (in seconds)")]
        [SerializeField] private float shadowsUpdateRateSeconds = 0.5f;

        [Header("Advanced")]
        [Tooltip("How much lights within the view frustum are favored. If a light is within the view frustum, the system will subtract the light's distance to the camera by the value of this field, which hereby favors lights within the view frustum.")]
        [SerializeField] private float lightWithinViewFrustumBonus = 10.0f;
        
#if UNITY_EDITOR
#if EDITOR_ATTRIBUTES
        private bool ShowMessageBox => !Application.isPlaying && drawGizmos;
        [HelpBox("Gizmos legend:\n" +
                 "<color=black>Black</color> - disabled.\n" +
                 "<color=red>Red</color> - shadows disabled.\n" +
                 "<color=blue>Blue</color> - custom shadow resolution.\n" +
                 "<color=orange>Orange</color> - low shadow resolution.\n" +
                 "<color=yellow>Yellow</color> - medium shadow resolution.\n" +
                 "<color=green>Green</color> - high shadow resolution.")]
        [MessageBox("Gizmos only work during PlayMode.", nameof(ShowMessageBox), MessageMode.Warning)]
#endif
        [SerializeField] private bool drawGizmos = true;
#endif

        private readonly HashSet<LightBudgetLight> _lights = new();
        
        private static FieldInfo _tierFieldCache;

        private void Start()
        {
            float randomOffset = Random.Range(0.0f, enabledUpdateRateSeconds);
            InvokeRepeating(nameof(UpdateLightsEnabled), randomOffset, enabledUpdateRateSeconds);
            
            randomOffset = Random.Range(0.0f, shadowsUpdateRateSeconds);
            InvokeRepeating(nameof(UpdateLightsShadows), randomOffset, shadowsUpdateRateSeconds);
        }

        internal void RegisterLight(LightBudgetLight light)
        {
            if (!_lights.Add(light))
            {
                Debug.LogWarning("LightBudgetManager already has light: " + light.gameObject.name);
                return;
            }
        }

        internal void DeregisterLight(LightBudgetLight light)
        {
            _lights.Remove(light);
        }

        private void UpdateLightsEnabled()
        {
            Profiler.BeginSample($"{nameof(LightBudgetManager)}.{nameof(UpdateLightsEnabled)}");

            var sortedLights = _lights
#if ZLINQ
                .AsValueEnumerable()
#endif
                .OrderBy(GetLightOrdering);
            
            foreach (LightBudgetLight light in sortedLights.Take(maxEnabledLights))
                light.Light.enabled = true;
            
            foreach (LightBudgetLight light in sortedLights.Skip(maxEnabledLights))
                light.Light.enabled = false;
            
            Profiler.EndSample();
        }

        private float GetLightOrdering(LightBudgetLight light)
        {
            return VectorUtils.Distance(light.transform.position,
                GameInterface.Instance.GetCurrentCamera().transform.position) - GetLightBias(light);
        }

        private float GetLightBias(LightBudgetLight light)
        {
            Bounds bounds = light.GetBounds();
            return CameraUtils.IsObjectWithinViewFrustum(bounds) ? lightWithinViewFrustumBonus : 0.0f;
        }

        private void UpdateLightsShadows()
        {
            Profiler.BeginSample($"{nameof(LightBudgetManager)}.{nameof(UpdateLightsShadows)}");

            var sortedLights = _lights
#if ZLINQ
                .AsValueEnumerable()
#endif
                .OrderBy(GetLightOrdering);
            
            int accumulator = 0;
            foreach (ShadowTier shadowTier in shadowTiers)
            {
                foreach (LightBudgetLight lightInTier in sortedLights.Skip(accumulator).Take(shadowTier.amount))
                {
                    lightInTier.Light.shadows = shadowTier.shadowType;
                    lightInTier.AdditionalLightData.additionalLightsShadowResolutionTier = (int)shadowTier.resolution;
                }
                
                accumulator += shadowTier.amount;
            }

            foreach (LightBudgetLight remainingLight in sortedLights.Skip(accumulator))
                remainingLight.Light.shadows = LightShadows.None;
            
            Profiler.EndSample();
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (LightBudgetLight light in _lights)
            {
                if (!light.Light.enabled)
                {
                    DrawLightGizmo(light, Color.black);
                    continue;
                }

                if (light.Light.shadows == LightShadows.None)
                {
                    DrawLightGizmo(light, Color.red);
                    continue;
                }

                switch (light.AdditionalLightData.additionalLightsShadowResolutionTier)
                {
                    case (int)Resolution.Low:
                        DrawLightGizmo(light, Color.orangeRed);
                        continue;
                    
                    case (int)Resolution.Medium:
                        DrawLightGizmo(light, Color.yellow);
                        continue;
                    
                    case (int)Resolution.High:
                        DrawLightGizmo(light, Color.green);
                        continue;
                    
                    default:
                        DrawLightGizmo(light, Color.blue);
                        continue;
                }
            }
        }

        private static void DrawLightGizmo(LightBudgetLight light, Color color)
        {
            const float epsilon = 0.0001f;
            Gizmos.color = color;
            Gizmos.DrawCube(light.transform.parent.position - epsilon * Vector3.up, light.transform.parent.lossyScale - epsilon * 2 * Vector3.one);
        }
#endif
    }
}
#endif
