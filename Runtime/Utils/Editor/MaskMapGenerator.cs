using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace BioluminescentGames.Utils.Editor
{
    public class MaskMapGenerator : EditorWindow
    {
        [MenuItem("Tools/Mask Map Generator")]
        public static void ShowWindow()
        {
            GetWindow<MaskMapGenerator>("Mask Map Generator");
        }

        private string _metallicPath = "";
        private string _roughnessPath = "";
        private string _aoPath = "";
        private string _detailPath = "";
        private string _savePath = "Assets/generated_maskmap.png";

        private void OnGUI()
        {
            GUILayout.Label("Select Texture Maps (All Optional)", EditorStyles.boldLabel);

            _metallicPath = DrawPathSelector("Metallic Map (R)", _metallicPath);
            _aoPath = DrawPathSelector("AO Map (G)", _aoPath);
            _detailPath = DrawPathSelector("Detail Mask (B)", _detailPath);
            _roughnessPath = DrawPathSelector("Roughness Map (A - Inverted)", _roughnessPath);

            GUILayout.Space(10);
            _savePath = EditorGUILayout.TextField("Save As", _savePath);

            if (GUILayout.Button("Generate Mask Map"))
            {
                GenerateMaskMap();
            }
        }

        private static string DrawPathSelector(string label, string path)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(150));
            path = GUILayout.TextField(path);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string file = EditorUtility.OpenFilePanel(label, "Assets", "png,jpg,tga");
                if (!string.IsNullOrEmpty(file))
                {
                    if (file.StartsWith(Application.dataPath))
                        path = "Assets" + file[Application.dataPath.Length..];
                    else
                        Debug.LogWarning("Selected file must be inside Assets folder.");
                }
            }
            GUILayout.EndHorizontal();
            return path;
        }

        private void GenerateMaskMap()
        {
            Texture2D metallic = LoadTexture(_metallicPath);
            Texture2D roughness = LoadTexture(_roughnessPath);
            Texture2D ao = LoadTexture(_aoPath);
            Texture2D detail = LoadTexture(_detailPath);

            int width = GetFirstAvailableWidth(metallic, roughness, ao, detail);
            int height = GetFirstAvailableHeight(metallic, roughness, ao, detail);

            if (width == 0 || height == 0)
            {
                Debug.LogError("You must provide at least one texture to determine the output resolution.");
                return;
            }

            Texture2D maskMap = new(width, height, TextureFormat.RGBA32, false);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float r = GetPixelSafe(metallic, x, y, 0f);          // metallic → default 0
                    float g = GetPixelSafe(ao, x, y, 1f);                 // AO → default 1
                    float b = GetPixelSafe(detail, x, y, 1f);            // detail → default 1
                    float a = 1f - GetPixelSafe(roughness, x, y, 0.5f);    // smoothness = 1 - roughness → default smooth = 0.5

                    maskMap.SetPixel(x, y, new Color(r, g, b, a));
                }
            }

            maskMap.Apply();

            byte[] pngData = maskMap.EncodeToPNG();
            File.WriteAllBytes(_savePath, pngData);
            AssetDatabase.Refresh();

            Debug.Log("Mask Map generated at: " + _savePath);
        }

        private static Texture2D LoadTexture(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (!tex)
            {
                Debug.LogError("Could not load texture at " + path);
                return null;
            }

            string assetPath = AssetDatabase.GetAssetPath(tex);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null || importer.isReadable) return tex;
            
            importer.isReadable = true;
            importer.SaveAndReimport();

            return tex;
        }

        private static float GetPixelSafe(Texture2D tex, int x, int y, float defaultValue)
        {
            return tex != null ? tex.GetPixel(x, y).r : defaultValue;
        }

        private static int GetFirstAvailableWidth(params Texture2D[] textures)
        {
            return (from tex in textures where tex != null select tex.width).FirstOrDefault();
        }

        private static int GetFirstAvailableHeight(params Texture2D[] textures)
        {
            return (from tex in textures where tex != null select tex.height).FirstOrDefault();
        }
    }
}
