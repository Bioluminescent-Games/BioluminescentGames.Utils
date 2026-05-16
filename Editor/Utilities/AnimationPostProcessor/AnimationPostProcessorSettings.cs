using UnityEditor;
using UnityEngine;

namespace BioluminescentGames.Utils.Editor
{
    [CreateAssetMenu(fileName = "AnimationPostProcessorSettings", menuName = "Scriptable Objects/AnimationPostProcessorSettings")]
    public class AnimationPostProcessorSettings : ScriptableObject
    {
        [field: SerializeField] public bool Enabled { get; private set; } = true;
        [field: SerializeField] public string TargetFolder { get; private set; } = "Assets/_Project/Animations";
        [field: SerializeField] public Avatar ReferenceAvatar { get; private set; }
        [field: SerializeField] public GameObject ReferenceFBX { get; private set; }

        [field: SerializeField] public bool EnableTranslationDoF { get; private set; } = true;
        [field: SerializeField] public ModelImporterAnimationType AnimationType { get; private set; } = ModelImporterAnimationType.Human;
        [field: SerializeField] public bool LoopTime { get; private set; } = true;
        [field: SerializeField] public bool RenameClips { get; private set; } = true;
        [field: SerializeField] public bool ForceEditorApply { get; private set; } = true;
        [field: SerializeField] public bool ExtractTextures { get; private set; } = true;
    }
}
