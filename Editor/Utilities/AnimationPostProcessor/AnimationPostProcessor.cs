using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BioluminescentGames.Utils.Editor
{
    // https://youtu.be/V9_Z6hUOG_8?si=O8QXT479tmp0Qj2S
    public class AnimationPostProcessor : AssetPostprocessor
    {
        private static AnimationPostProcessorSettings _settings;
        private static Avatar _referenceAvatar;
        private static GameObject _referenceFBX;
        private static ModelImporter _referenceImporter;
        private static bool _settingsLoaded;

        private void OnPreprocessModel()
        {
            LoadSettings();
            if (!_settingsLoaded || !_settings.Enabled)
                return;
            
            // Check if asset is in the specified folder
            ModelImporter importer = assetImporter as ModelImporter;
            if (!importer.assetPath.StartsWith(_settings.TargetFolder)) return;
        
            AssetDatabase.ImportAsset(importer.assetPath);
            
            // Extract materials and textures
            if (_settings.ExtractTextures) {
                importer.ExtractTextures(Path.GetDirectoryName(importer.assetPath));
                importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            }
            
            // Extract avatar from the reference FBX if not already specified
            if (_referenceAvatar == null) {
                _referenceAvatar = _referenceImporter.sourceAvatar;
            }
            
            // Set the avatar and rig type of the imported model
            importer.sourceAvatar = _referenceAvatar;
            importer.animationType = _settings.AnimationType;
            
            // Set the animation to Generic if some issue with the avatar
            if (_referenceAvatar == null || !_referenceAvatar.isValid) {
                importer.animationType = ModelImporterAnimationType.Generic;
            }
            
            // Use serialization to set the avatar correctly
            SerializedObject serializedObject = new SerializedObject((UnityEngine.Object) importer.sourceAvatar);
            using (SerializedObject sourceObject = new SerializedObject((UnityEngine.Object) _referenceAvatar))
                CopyHumanDescriptionToDestination(sourceObject, serializedObject);
            serializedObject.ApplyModifiedProperties();
            importer.sourceAvatar = serializedObject.targetObject as Avatar;
            serializedObject.Dispose();
            
            // Translation DoF
            if (_settings.EnableTranslationDoF) {
                HumanDescription importerHumanDescription = importer.humanDescription;
                importerHumanDescription.hasTranslationDoF = true;
                importer.humanDescription = importerHumanDescription;
            }
            
            // Use reflection to instantiate an Editor and call the Apply method as if the Apply button was pressed
            if (_settings.ForceEditorApply) {
                Type editorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ModelImporterEditor");
                const BindingFlags nonPublic = BindingFlags.NonPublic | BindingFlags.Instance;
                UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(importer, editorType);
                editorType.GetMethod("Apply", nonPublic)!.Invoke(editor, null);
                UnityEngine.Object.DestroyImmediate(editor);
            }
        }

        private void OnPreprocessAnimation() {
            LoadSettings();
            if (!_settingsLoaded || !_settings.Enabled) return;

            // Check if asset is in the specified folder
            ModelImporter importer = assetImporter as ModelImporter;
            if (!importer.assetPath.StartsWith(_settings.TargetFolder)) return;

            ModelImporter modelImporter = CopyModelImporterSettings(assetImporter as ModelImporter);
        
            AssetDatabase.ImportAsset(modelImporter.assetPath, ImportAssetOptions.ForceUpdate);
        }
        
        private ModelImporter CopyModelImporterSettings(ModelImporter importer) {
            // model tab
            importer.globalScale = _referenceImporter.globalScale;
            importer.useFileScale = _referenceImporter.useFileScale;
            importer.meshCompression = _referenceImporter.meshCompression;
            importer.isReadable = _referenceImporter.isReadable;
            importer.optimizeMeshPolygons = _referenceImporter.optimizeMeshPolygons;
            importer.optimizeMeshVertices = _referenceImporter.optimizeMeshVertices;
            importer.importBlendShapes = _referenceImporter.importBlendShapes;
            importer.keepQuads = _referenceImporter.keepQuads;
            importer.indexFormat = _referenceImporter.indexFormat;
            importer.weldVertices = _referenceImporter.weldVertices;
            importer.importVisibility = _referenceImporter.importVisibility;
            importer.importCameras = _referenceImporter.importCameras;
            importer.importLights = _referenceImporter.importLights;
            importer.preserveHierarchy = _referenceImporter.preserveHierarchy;
            importer.swapUVChannels = _referenceImporter.swapUVChannels;
            importer.generateSecondaryUV = _referenceImporter.generateSecondaryUV;
            importer.importNormals = _referenceImporter.importNormals;
            importer.normalCalculationMode = _referenceImporter.normalCalculationMode;
            importer.normalSmoothingAngle = _referenceImporter.normalSmoothingAngle;
            importer.importTangents = _referenceImporter.importTangents;
            
            // rig tab
            importer.animationType = _referenceImporter.animationType;
            importer.optimizeGameObjects = _referenceImporter.optimizeGameObjects;
            
            // materials tab
            importer.materialImportMode = _referenceImporter.materialImportMode;
            importer.materialLocation = _referenceImporter.materialLocation;
            importer.materialName = _referenceImporter.materialName;
            
            // naming conventions
            // get the filename of the FBX in case we want to use it for the animation name
            string fileName = Path.GetFileNameWithoutExtension(importer.assetPath);
            
            // animations tab
            // return if there are no clips to copy on the reference importer
            if (_referenceImporter.clipAnimations.Length == 0) return importer;
            
            // Copy the first reference clip settings to all imported clips
            ModelImporterClipAnimation referenceClip = _referenceImporter.clipAnimations[0];
            ModelImporterClipAnimation[] referenceClipAnimations = _referenceImporter.defaultClipAnimations;
            
            
            ModelImporterClipAnimation[] defaultClipAnimations = importer.defaultClipAnimations;
            
            foreach (ModelImporterClipAnimation clipAnimation in defaultClipAnimations) {
                clipAnimation.hasAdditiveReferencePose = referenceClip.hasAdditiveReferencePose;
                if (referenceClip.hasAdditiveReferencePose) {
                    clipAnimation.additiveReferencePoseFrame = referenceClip.additiveReferencePoseFrame;
                }
                
                // Rename it if needed
                if (_settings.RenameClips) {
                    if (referenceClipAnimations.Length == 1) {
                        clipAnimation.name = fileName;
                    } else {
                        clipAnimation.name = fileName + "" + clipAnimation.name;
                    }
                }
                
                // Set loop time
                clipAnimation.loopTime = _settings.LoopTime;

                clipAnimation.maskType = referenceClip.maskType;
                clipAnimation.maskSource = referenceClip.maskSource;

                clipAnimation.keepOriginalOrientation = referenceClip.keepOriginalOrientation;
                clipAnimation.keepOriginalPositionXZ = referenceClip.keepOriginalPositionXZ;
                clipAnimation.keepOriginalPositionY = referenceClip.keepOriginalPositionY;

                clipAnimation.lockRootRotation = referenceClip.lockRootRotation;
                clipAnimation.lockRootPositionXZ = referenceClip.lockRootPositionXZ;
                clipAnimation.lockRootHeightY = referenceClip.lockRootHeightY;

                clipAnimation.mirror = referenceClip.mirror;
                clipAnimation.wrapMode = referenceClip.wrapMode;
            }

            importer.clipAnimations = defaultClipAnimations;
            

            return importer;
        }

        private static void CopyHumanDescriptionToDestination(SerializedObject sourceObject, SerializedObject serializedObject) {
            serializedObject.CopyFromSerializedProperty(sourceObject.FindProperty("m_HumanDescription"));
        }

        private static void LoadSettings() {
            string[] guids = AssetDatabase.FindAssets("t:AnimationPostProcessorSettings");
            if (guids.Length > 0) {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _settings = AssetDatabase.LoadAssetAtPath<AnimationPostProcessorSettings>(path);
            
                _referenceAvatar = _settings.ReferenceAvatar;
                _referenceFBX = _settings.ReferenceFBX;
                _referenceImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_referenceFBX)) as ModelImporter;
                _settingsLoaded = true;
            } else {
                _settingsLoaded = false;
            }
        }
    }
}
