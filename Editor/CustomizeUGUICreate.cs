//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年03月22日-03:20
//IcMusicPlayer.Editor

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace IcMusicPlayer.Editors
{
    /// <summary>
    /// ugui 创建工具扩展
    /// </summary>
    public class CustomizeUGUICreate : EditorWindow
    {
        /// <summary>
        /// 默认材质路径 Key
        /// </summary>
        public const string Uguiexdefaultmaterialpath_String = "UGUIExDefaultMaterialPath";

        /// <summary>
        /// 默认精灵图路径 Key
        /// </summary>
        public const string Uguiexdefaultspritepath_String = "UGUIExDefaultSpritePath";

        /// <summary>
        /// 是否是射线目标 Key
        /// </summary>
        public const string Uguiexisraycasttarget_Bool = "UGUIExIsRayCastTarget";

        /// <summary>
        /// 是否富文本支持 key
        /// </summary>
        public const string Uguiexisrich_Bool = "UGUIExIsRich";

        #region Language Var
        private static string _notFindMaterial  = "The set Material could not be found";
        private static string _notSprite = "The set Sprite could not be found";
        private static string _titile = "UGUI Settings";
        private static string _selectDefaultMaterialLabel = "Set Default Material:";
        private static string _selectDefaultSpriteLabel = "Set Default Sprite";
        private static string _isRayCshTargetLabel = "Open RayCastTarget(Interactive elements are not affected) Default Value: false";
        private static string _isOpenRich = "Open Rich Support,Default Value: false";
        private static string _optimizeWarning = "If {0} is not set, Unity will set the default value at runtime, which will cause efficiency problems. It is recommended to manually select to avoid runtime assignment. Click to locate the object.";
        private static string _notFindAssetError = "{0},Path:{1}";
        #endregion

        
        public static Material GetDefalutMaterial(GameObject go)
        {
            return _loadAsset<Material>(Uguiexdefaultmaterialpath_String, _notFindMaterial,go);
        }

        public static Sprite GetDefalutSprite(GameObject go)
        {
            return _loadAsset<Sprite>(Uguiexdefaultspritepath_String, _notSprite,go);
        }

        private static T _loadAsset<T>(string key, string errorMessage = null,GameObject go = null) where T : Object
        {
            var path = EditorPrefs.GetString(key);

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (!asset)
            {
                if (string.IsNullOrEmpty(path))
                {
                    if(typeof(T) == typeof(Material))
                        Debug.LogWarningFormat(_optimizeWarning,typeof(T).Name,go);   
                }
                
                if (!string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(path))
                {
                    Debug.LogErrorFormat(_notFindAssetError,errorMessage,path);
                }
            }

            return asset;
        }

        public static bool IsRayCastTarget => EditorPrefs.GetBool(Uguiexisraycasttarget_Bool);
        
        public static bool IsRich => EditorPrefs.GetBool(Uguiexisrich_Bool);

        [MenuItem("Icarus/UGUI/Optimized Element Setting",false,33)]
        static void _uGUISetting()
        {
            var win = GetWindow<CustomizeUGUICreate>(true, _titile);

            win.Show();
        }

        static void _setDefaultMaterial(Material material)
        {
            _setAssetPathToEditorPrefsString(material, Uguiexdefaultmaterialpath_String);
        }

        static void _setDefaultSprite(Sprite sprite)
        {
            _setAssetPathToEditorPrefsString(sprite, Uguiexdefaultspritepath_String);
        }

        static void _setAssetPathToEditorPrefsString<T>(T asset, string key) where T : Object
        {
            string path = string.Empty;

            if (asset)
            {
                path = AssetDatabase.GetAssetPath(asset);
            }
            
            EditorPrefs.SetString(key, path);
        }

        static void _setRayCastEnableState(bool enable)
        {
            EditorPrefs.SetBool(Uguiexisraycasttarget_Bool, enable);
        }
        
        static void _setRichEnableState(bool enable)
        {
            EditorPrefs.SetBool(Uguiexisrich_Bool, enable);
        }

        private Material _material;
        private Sprite _sprite;
        private bool _isRayCastTarget,_isRich;


        private void Awake()
        {
            _material = _loadAsset<Material>(Uguiexdefaultmaterialpath_String);
            _sprite = _loadAsset<Sprite>(Uguiexdefaultspritepath_String);
            _isRayCastTarget = IsRayCastTarget;
            _isRich = IsRich;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(_selectDefaultMaterialLabel);

                _material = (Material) EditorGUILayout.ObjectField(_material, typeof(Material), false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(_selectDefaultSpriteLabel);

                _sprite = (Sprite) EditorGUILayout.ObjectField(_sprite, typeof(Sprite), false);
            }
            EditorGUILayout.EndHorizontal();

            _isRayCastTarget = EditorGUILayout.ToggleLeft(_isRayCshTargetLabel, _isRayCastTarget);
            
            _isRich = EditorGUILayout.ToggleLeft(_isOpenRich, _isRich);
            
            _setDefaultMaterial(_material);
            _setDefaultSprite(_sprite);
            _setRayCastEnableState(_isRayCastTarget);
            _setRichEnableState(_isRich);
            
//            if (GUILayout.Button("复制 Unity 默认 Sprite 到项目"))
//            {
//                string kKnobPath = "UI/Skin/Knob.psd";
//
//                //var path = EditorUtility.SaveFilePanel("Select Save Folder", Application.dataPath,"uiSprite", ".png");
//
//                var path = EditorUtility.OpenFolderPanel("s", Application.dataPath, "");
//                
//                if (string.IsNullOrEmpty(path))
//                {
//                    Debug.LogError("取消了复制");
//                    return;
//                }

//                Object[] UnityAssets = AssetDatabase.LoadAllAssetsAtPath("Resources/unity_builtin_extra");
//                foreach (var asset in UnityAssets)
//                {
//                    path = path.Replace(Application.dataPath,"Assets/");
//                    if (asset is Texture2D)
//                    {
//                        AssetDatabase.CreateAsset(asset, Path.Combine(path, "uiSprite.asset"));
//                    }
//                }

            // var texture2D = AssetDatabase.GetBuiltinExtraResource<Texture2D>(kKnobPath);
//                var sp = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
//                    new Vector2(0.5f, 0.5f));
//
//                var bys = sp.texture.EncodeToPNG();
//
//                path = path.Replace(Application.dataPath,"Assets/");

            //var bys = CombineTextures(texture2D).EncodeToPNG();

            //File.WriteAllBytes(path,bys);

//                Graphics.CopyTexture(texture2D.);

//                AssetDatabase.CreateAsset(texture2D, Path.Combine(path, "uiSprite.png"));

//                AssetDatabase.Refresh();
//            }
        }
    }
}