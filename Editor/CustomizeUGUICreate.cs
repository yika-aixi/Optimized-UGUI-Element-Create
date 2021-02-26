#if PACKAGE_UGUI
//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年03月22日-03:20
//IcMusicPlayer.Editor

using System;
using System.IO;
using CabinIcarus.EditorFrame.Attributes;
using CabinIcarus.EditorFrame.Base.Editor;
using CabinIcarus.EditorFrame.Config;
using CabinIcarus.EditorFrame.Localization;
using CabinIcarus.EditorFrame.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CabinIcarus.UGUI.OptimizedElement
{
    [InitializeOnRun(-1)]
    static class Init
    {
        static Init()
        {
            EditorApplication.update += _loadLanguage;
        }

        private static bool _load;
        private static void _loadLanguage()
        {
            if (_load)
            {
                return;
            }
            
            var path = typeof(CustomizeUGUICreate).GetTypeProjectFolderPath(false);
            
            LocalizationManager.Instance.LoadCsvLanguageConfig(PathUtil.GetDataPathCombinePath($"{Path.GetDirectoryName(path)}/Localzation/Window")
                ,1);

            _load = true;
        }
    }
    
    /// <summary>
    /// ugui 创建工具扩展
    /// </summary>
    public class CustomizeUGUICreate : LocalizationEditorWindow
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
        /// 滚动视图Mask转RectMask Key
        /// </summary>
        public const string DoNotMaskToRectMask_Bool = "ScrollViewViewportMaskToRectMask";

        /// <summary>
        /// 是否富文本支持 key
        /// </summary>
        public const string Uguiexisrich_Bool = "UGUIExIsRich";
        
        /// <summary>
        /// 默认字体
        /// </summary>
        public const string UguiexDefaultFontPath = "UguiexDefaultFontPath";

        #region Language Var
        private static string _notFindMaterial            => LocalizationManager.Instance.GetValue("NotFindMaterial",                    out  _);
        private static string _notFindFont                => LocalizationManager.Instance.GetValue("NotFindFont",                        out  _);
        private static string _titile                     => LocalizationManager.Instance.GetValue("OptimizedElementSettingWindowTitle", out  _);
        private static string _selectDefaultMaterialLabel => LocalizationManager.Instance.GetValue("SetDefaultMaterial",                 out  _);
        private static string _selectDefaultFontlLabel    => LocalizationManager.Instance.GetValue("SetDefaultFont",                     out  _);
        private static string _selectDefaultSpriteLabel   => LocalizationManager.Instance.GetValue("SetDefaultSprite",                   out  _);
        private static string _isRayCastTargetLabel       => LocalizationManager.Instance.GetValue("OpenRayCastTarget",                  out  _);
        private static string _isOpenRich                 => LocalizationManager.Instance.GetValue("OpenRich",                           out  _);
        private static string _optimizeWarning            => LocalizationManager.Instance.GetValue("OptimizeWarning",                    out  _);
        private static string _notFindAssetError          => LocalizationManager.Instance.GetValue("NotFindAsset",                       out  _);
        private static string _doNotMaskToRectMask        => LocalizationManager.Instance.GetValue("DoNotMaskToRectMask",                out  _);
        
        private static string _spriteSetting => LocalizationManager.Instance.GetValue("SpriteSetting", "Sprite Setting");
        #endregion

        public static Font GetDefaultFont(GameObject go)
        {
            return _loadAsset<Font>(UguiexDefaultFontPath, _notFindFont, go);
        }
        
        public static Material GetDefaultMaterial(GameObject go)
        {
            return _loadAsset<Material>(Uguiexdefaultmaterialpath_String, _notFindMaterial, go);
        }

        private static T _loadAsset<T>(string key, string errorMessage = null, GameObject go = null) where T : Object
        {
            var path = Cfg.CSVEncrypting.GetValue<string>(key);

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (!asset)
            {
                if (string.IsNullOrEmpty(path))
                {
                    if(typeof(T) == typeof(Material))
                        Debug.LogWarningFormat(_optimizeWarning, typeof(T).Name, go);   
                }
                
                if (!string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(path))
                {
                    Debug.LogErrorFormat(_notFindAssetError, errorMessage, path);
                }
            }

            return asset;
        }

        public static bool IsRayCastTarget => Cfg.CSVEncrypting.GetValue<bool>(Uguiexisraycasttarget_Bool);
        
        public static bool IsRich => Cfg.CSVEncrypting.GetValue<bool>(Uguiexisrich_Bool);
        
        public static bool NoToRectMask => Cfg.CSVEncrypting.GetValue<bool>(DoNotMaskToRectMask_Bool);

        private static string _getSpriteKey(string name)
        {
            return $"{Uguiexdefaultspritepath_String}_{name}";
        }

        private static Sprite _getSprite(Sprite sprite, string name, string builtinExtraResourcePath)
        {
            if (!sprite)
            {
                sprite = _loadAsset<Sprite>(_getSpriteKey(name));
            }

            if (!sprite)
            {
                sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(builtinExtraResourcePath);
            }

            return sprite;
        }

        #region Sprite

        public static Sprite UiSprite
        {
            get
            {
                if (!_uiSprite)
                {
                    _uiSprite = _getSprite(_uiSprite, "UISprite", UGUISource_MenuOptions.kStandardSpritePath);
                }

                return _uiSprite;
            }
        }

        public static Sprite BackgroundSprite
        {
            get
            {
                if (!_backgroundSprite)
                {
                    _backgroundSprite = _getSprite(_backgroundSprite, "Background", UGUISource_MenuOptions.kBackgroundSpritePath);
                }

                return _backgroundSprite;
            }
        }

        public static Sprite InputFieldBackground
        {
            get
            {
                if (!_inputFieldBackground)
                {
                    _inputFieldBackground = _getSprite(_inputFieldBackground, "InputFieldBackground", UGUISource_MenuOptions.kInputFieldBackgroundPath);
                }
                
                return _inputFieldBackground;
            }
        }

        public static Sprite Knob
        {
            get
            {
                if (!_knob)
                {
                    _knob = _getSprite(_knob, "Knob", UGUISource_MenuOptions.kKnobPath);
                }

                return _knob;
            }
        }

        public static Sprite Checkmark
        {
            get
            {
                if (!_checkmark)
                {
                    _checkmark = _getSprite(_checkmark, "Checkmark", UGUISource_MenuOptions.kCheckmarkPath);
                }
                
                return _checkmark;
            }
        }

        public static Sprite DropdownArrow
        {
            get
            {
                if (!_dropdownArrow)
                {
                    _dropdownArrow = _getSprite(_dropdownArrow, "DropdownArrowPath", UGUISource_MenuOptions.kDropdownArrowPath);
                }
                
                return _dropdownArrow;
            }
        }

        public static Sprite UIMask
        {
            get
            {
                if (!_uIMask)
                {
                    _uIMask = _getSprite(_uIMask, "MaskPath", UGUISource_MenuOptions.kMaskPath);
                }
                
                return _uIMask;
            }
        }

        #endregion

        private Vector2 _minSize = new Vector2(50, 50);
        private float   _sizeScale;
        public Vector2 SpriteFieldSize
        {
            get => _spriteFieldSize;
            set
            {
                _spriteFieldSize = value;

                _spriteFieldSize[0] = Math.Max(_minSize[0], value[0]);

                _spriteFieldSize[1] = Math.Max(_minSize[1], value[1]);
            }
        }

        [MenuItem("Icarus/UGUI/Optimized Element Setting", false, 33)]
        static void _uGUISetting()
        {
            var win = GetWindow<CustomizeUGUICreate>(true, _titile);

            win.Show();
        }

        void _setDefaultFont()
        {
            _saveObjectPathToCfg(_font, UguiexDefaultFontPath);
        }
        
        static void _setDefaultMaterial(Material material)
        {
            _saveObjectPathToCfg(material, Uguiexdefaultmaterialpath_String);
        }

        static void _setDefaultSprite(Sprite sprite, string key)
        {
            _saveObjectPathToCfg(sprite, _getSpriteKey(key));
        }

        static void _saveObjectPathToCfg<T>(T asset, string key) where T : Object
        {
            string path = string.Empty;

            if (asset)
            {
                path = AssetDatabase.GetAssetPath(asset);
            }
            
            Cfg.CSVEncrypting.SetValue(key, path);
        }

        static void _setRayCastEnableState(bool enable)
        {
            Cfg.CSVEncrypting.SetValue(Uguiexisraycasttarget_Bool, enable);
        }
        
        static void _setRichEnableState(bool enable)
        {
            Cfg.CSVEncrypting.SetValue(Uguiexisrich_Bool, enable);
        }

        private        Material _material;
        private        Font     _font;
        private static Sprite   _uiSprite;
        private static Sprite   _backgroundSprite;
        private static Sprite   _inputFieldBackground;
        private static Sprite   _knob;
        private static Sprite   _checkmark;
        private static Sprite   _dropdownArrow;
        private static Sprite   _uIMask;
        
        private bool _isRayCastTarget, _isRich, _donotMaskToRMask;

        private bool _spriteSet;

        private void Awake()
        {
            _material        = _loadAsset<Material>(Uguiexdefaultmaterialpath_String);
            _font            = _loadAsset<Font>(UguiexDefaultFontPath);
            _isRayCastTarget = IsRayCastTarget;
            _isRich          = IsRich;
        }

        protected override void On_Enable()
        {
            //init
            var t = UiSprite;
            t = UIMask;
            t = Knob;
            t = Checkmark;
            t = BackgroundSprite;
            t = DropdownArrow;
            t = InputFieldBackground;
        }

        private Vector2 _spriteFieldSize;
        private Vector2 _spriteFieldPos;
        private void OnGUI()
        {
            DrawLocalizationSelect();
            
            EditorGUILayout.Space();
            EditorGUILayoutUtil.DrawUILine(Color.cyan, width: position.width);
            EditorGUILayout.Space();

            _drawSetField(ref _material, _selectDefaultMaterialLabel);
            _drawSetField(ref _font,     _selectDefaultFontlLabel);
            
            _spriteSet = EditorGUILayout.Foldout(_spriteSet, _spriteSetting, true);
            
            if (_spriteSet)
            {
                SpriteFieldSize = EditorGUILayout.Vector2Field("Size", SpriteFieldSize);
                _sizeScale      = EditorGUILayout.Slider("SizeScale", _sizeScale, 1, 3);

                _spriteFieldPos = EditorGUILayout.BeginScrollView(_spriteFieldPos);
                {
                    _spriteHandle("UISprite", UGUISource_MenuOptions.kStandardSpritePath, ref _uiSprite);
                    _spriteHandle("Background", UGUISource_MenuOptions.kBackgroundSpritePath,
                        ref _backgroundSprite);
                    _spriteHandle("InputFieldBackground", UGUISource_MenuOptions.kInputFieldBackgroundPath,
                        ref _inputFieldBackground);
                    _spriteHandle("Knob",          UGUISource_MenuOptions.kKnobPath,          ref _knob);
                    _spriteHandle("Checkmark",     UGUISource_MenuOptions.kCheckmarkPath,     ref _checkmark);
                    _spriteHandle("DropdownArrow", UGUISource_MenuOptions.kDropdownArrowPath, ref _dropdownArrow);
                    _spriteHandle("UIMask",        UGUISource_MenuOptions.kMaskPath,          ref _uIMask);
                }
                EditorGUILayout.EndScrollView();
            }

            _isRayCastTarget = EditorGUILayout.ToggleLeft(_isRayCastTargetLabel, _isRayCastTarget);
            
            _isRich = EditorGUILayout.ToggleLeft(_isOpenRich, _isRich);
            
            _donotMaskToRMask = EditorGUILayout.ToggleLeft(_doNotMaskToRectMask, _donotMaskToRMask);

            _setDefaultFont();
            _setDefaultMaterial(_material);
            _setRayCastEnableState(_isRayCastTarget);
            _setRichEnableState(_isRich);
            Cfg.CSVEncrypting.SetValue(DoNotMaskToRectMask_Bool, _donotMaskToRMask);
        }

        void _drawSetField<T>(ref T obj, string label) where T : Object
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label);

                obj = (T) EditorGUILayout.ObjectField(obj, typeof(T), false);
            }
            EditorGUILayout.EndHorizontal();
        }

        private string _lastSelectPath;
        
        private void _spriteHandle(string title, string builtinExtraResourcePath, ref Sprite cache)
        {
            if (string.IsNullOrWhiteSpace(_lastSelectPath))
            {
                _lastSelectPath = Application.dataPath;
            }
            
            EditorGUILayout.BeginHorizontal("box");
            {
                EditorGUILayout.LabelField(_selectDefaultSpriteLabel, title);

                Sprite builtinExtraResource = AssetDatabase.GetBuiltinExtraResource<Sprite>(builtinExtraResourcePath);

                EditorGUILayout.BeginVertical();
                {
                    GUI.enabled = false;
                    {
                        EditorGUILayout.ObjectField(builtinExtraResource, typeof(Sprite), false, GUILayout.Width(SpriteFieldSize.x * _sizeScale), GUILayout.Height(SpriteFieldSize.y * _sizeScale));
                    }
                    GUI.enabled = true;

                    if (GUILayout.Button("Export", GUILayout.Width(SpriteFieldSize.x * _sizeScale)))
                    {
                        var temp = builtinExtraResource.texture.ToWritableAndRead();

                        var png = temp.EncodeToPNG();

                        var path = EditorUtility.OpenFolderPanel("save Folder", _lastSelectPath, "");

                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            _lastSelectPath = path;
                            path            = PathUtil.GetCombinePath(path, $"{builtinExtraResource.name}.png");
                            File.WriteAllBytes(path, png);
                            AssetDatabase.Refresh();
                            
                            var ass = (TextureImporter) AssetUtil.SelectAsset(path);
                            ass.isReadable  = true;
                            ass.textureType = TextureImporterType.Sprite;

                            bool isMulti = builtinExtraResource.rect.position.sqrMagnitude > 0;
                            
                            ass.spriteImportMode = isMulti ? SpriteImportMode.Multiple : SpriteImportMode.Single;

                            if (!isMulti)
                            {
                                ass.spriteBorder = builtinExtraResource.border;
                                ass.spritePivot  = builtinExtraResource.pivot;
                            }
                            else
                            {
                                ass.spritesheet = new[]
                                {
                                    new SpriteMetaData
                                    {
                                        alignment = 9,
                                        border    = builtinExtraResource.border,
                                        pivot     = builtinExtraResource.pivot,
                                        rect      = builtinExtraResource.rect,
                                        name      = builtinExtraResource.name
                                    }, 
                                };
                            }
                            
                            ass.SaveAndReimport();
                            AssetDatabase.Refresh();                      
                            var objects = AssetDatabase.LoadAllAssetsAtPath(ass.assetPath);
                            cache = (Sprite) objects[1];
                            _setDefaultSprite(cache, title);
                            Repaint();
                            return;
                        }
                    }
                }
                EditorGUILayout.EndVertical();
                
                EditorGUI.BeginChangeCheck();
                cache = (Sprite) EditorGUILayout.ObjectField(cache, typeof(Sprite), false, GUILayout.Width(SpriteFieldSize.x * _sizeScale), GUILayout.Height(SpriteFieldSize.y * _sizeScale));
                if (EditorGUI.EndChangeCheck())
                {
                    _setDefaultSprite(cache, title);
                    
                    if (!cache)
                    {
                        cache = builtinExtraResource;
                    }
                    
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif