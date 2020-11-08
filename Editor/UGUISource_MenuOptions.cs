//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年03月22日-04:00
//IcMusicPlayer.Editor

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IcMusicPlayer.Editors
{
    //name: MenuOptions
    public class UGUISource_MenuOptions
    {
        private const string kUILayerName = "UI";

        internal const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        internal const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        internal const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        internal const string kKnobPath = "UI/Skin/Knob.psd";
        internal const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        internal const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        internal const string kMaskPath = "UI/Skin/UIMask.psd";

#if UNITY_5_3_OR_NEWER
        static private DefaultControls.Resources s_StandardResources;

        static private DefaultControls.Resources GetStandardResources()
        {
            if (s_StandardResources.standard == null)
            {
                s_StandardResources.standard = CustomizeUGUICreate.UiSprite;
                s_StandardResources.background = CustomizeUGUICreate.BackgroundSprite;
                s_StandardResources.inputField = CustomizeUGUICreate.InputFieldBackground;
                s_StandardResources.knob = CustomizeUGUICreate.Knob;
                s_StandardResources.checkmark = CustomizeUGUICreate.Checkmark;
                s_StandardResources.dropdown = CustomizeUGUICreate.DropdownArrow;
                s_StandardResources.mask = CustomizeUGUICreate.UIMask;
            }

            return s_StandardResources;
        }
#endif

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            // Find the best scene view
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform,
                new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) +
                                     itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) +
                                     itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) -
                                     itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) -
                                     itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }

        public static void PlaceUIElementRoot(GameObject element, GameObject parent)
        {
            GameObject argParent = parent;
            
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                parent = GetOrCreateCanvasGameObject();
            }

            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
            element.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(element, "Create "                      + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, parent);
            if (parent != argParent) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(),
                    element.GetComponent<RectTransform>());

            Selection.activeGameObject = element;
        }
        
        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            PlaceUIElementRoot(element, menuCommand.context as GameObject);
        }

        static void _textSetting(Text text,bool isRayCastTarget)
        {
            _graphicSetting(text, isRayCastTarget);
            text.supportRichText = CustomizeUGUICreate.IsRich;
            text.font = CustomizeUGUICreate.GetDefaultFont(text.gameObject) ? CustomizeUGUICreate.GetDefaultFont(text.gameObject) : text.font;
        }

        static void _graphicSetting(Graphic graphic,bool isRayCastTarget)
        {
            graphic.raycastTarget = isRayCastTarget;
            graphic.material = CustomizeUGUICreate.GetDefaultMaterial(graphic.gameObject) ? CustomizeUGUICreate.GetDefaultMaterial(graphic.gameObject) : graphic.material;
        }
        
        // Graphic elements

        [MenuItem("GameObject/UI/New Text", false, -2000)]
        static public void AddText(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateText(GetStandardResources());
            Text text = go.GetComponent<Text>();
            _textSetting(text,CustomizeUGUICreate.IsRayCastTarget);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/New Image", false, -2001)]
        static public void AddImage(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateImage(GetStandardResources());
            var image = go.GetComponent<Image>();
            image.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            image.material = CustomizeUGUICreate.GetDefaultMaterial(image.gameObject) ? CustomizeUGUICreate.GetDefaultMaterial(image.gameObject) : image.material;
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/New Raw Image", false, -2002)]
        static public void AddRawImage(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateRawImage(GetStandardResources());
            var rawImage = go.GetComponent<RawImage>();
            _graphicSetting(rawImage, CustomizeUGUICreate.IsRayCastTarget);
            PlaceUIElementRoot(go, menuCommand);
        }

        // Controls

        // Button and toggle are controls you just click on.

        [MenuItem("GameObject/UI/New Button", false, -2030)]
        static public void AddButton(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateButton(GetStandardResources());

            var image = go.GetComponent<Image>();
            _graphicSetting(image, true);
            
            var text = go.transform.GetChild(0).GetComponent<Text>();
            _textSetting(text,CustomizeUGUICreate.IsRayCastTarget);
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/New Toggle", false, -2031)]
        static public void AddToggle(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateToggle(GetStandardResources());

            var background = go.transform.Find("Background").GetComponent<Image>();
            background.type = Image.Type.Simple;
            _graphicSetting(background, true);

            var checkmark = background.transform.Find("Checkmark").GetComponent<Image>();
            _graphicSetting(checkmark, CustomizeUGUICreate.IsRayCastTarget);

            var lable = go.transform.Find("Label").GetComponent<Text>();
            _textSetting(lable,true);
            PlaceUIElementRoot(go, menuCommand);
        }

        // Slider and Scrollbar modify a number

        [MenuItem("GameObject/UI/New Slider", false, -2033)]
        static public void AddSlider(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateSlider(GetStandardResources());
            var Background = go.transform.Find("Background").GetComponent<Image>();
            var Fill = go.transform.Find("Fill Area").GetChild(0).GetComponent<Image>();
            var Handle = go.transform.Find("Handle Slide Area").GetChild(0).GetComponent<Image>();
            
            _graphicSetting(Background, CustomizeUGUICreate.IsRayCastTarget);

            _graphicSetting(Fill, CustomizeUGUICreate.IsRayCastTarget);

            _graphicSetting(Handle, true);
            
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/New Scrollbar", false, -2034)]
        static public void AddScrollbar(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateScrollbar(GetStandardResources());

            _setScrollbar(go);
            
            PlaceUIElementRoot(go, menuCommand);
        }

        static void _setScrollbar(GameObject scrollbar)
        {
            var image = scrollbar.GetComponent<Image>();
            _graphicSetting(image, true);

            var handle = scrollbar.transform.Find("Sliding Area").GetChild(0).GetComponent<Image>();
            _graphicSetting(handle, true);
        }

        // More advanced controls below

        [MenuItem("GameObject/UI/New Dropdown", false, -2035)]
        static public void AddDropdown(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateDropdown(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var Label = go.transform.Find("Label").GetComponent<Text>();
            var Arrow = go.transform.Find("Arrow").GetComponent<Image>();
            var Template = go.transform.Find("Template").GetComponent<Image>();
            var Viewport = Template.transform.Find("Viewport").GetComponent<Image>();
            var Item = Viewport.transform.Find("Content").transform.Find("Item").GetComponent<Toggle>();
            var Item_Background = Item.transform.Find("Item Background").GetComponent<Image>();
            var Item_Checkmark = Item.transform.Find("Item Checkmark").GetComponent<Image>();
            var Item_Label = Item.transform.Find("Item Label").GetComponent<Text>();
            var goImage = go.GetComponent<Image>();
            
            _textSetting(Label,CustomizeUGUICreate.IsRayCastTarget);
            _textSetting(Item_Label,true);
            _graphicSetting(Arrow, CustomizeUGUICreate.IsRayCastTarget);
            _graphicSetting(Template, CustomizeUGUICreate.IsRayCastTarget);
            _graphicSetting(Viewport, CustomizeUGUICreate.IsRayCastTarget);
            _graphicSetting(Item_Background, CustomizeUGUICreate.IsRayCastTarget);
            _graphicSetting(Item_Checkmark, CustomizeUGUICreate.IsRayCastTarget);
            _graphicSetting(goImage, true);
            
        }

        [MenuItem("GameObject/UI/New Input Field", false, -2036)]
        public static void AddInputField(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateInputField(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var Placeholder = go.transform.Find("Placeholder").GetComponent<Text>();
            _textSetting(Placeholder,CustomizeUGUICreate.IsRayCastTarget);

            var Text = go.transform.Find("Text").GetComponent<Text>();
            _textSetting(Text,CustomizeUGUICreate.IsRayCastTarget);

            var goImage = go.GetComponent<Image>();
            _graphicSetting(goImage, true);
        }

        [MenuItem("GameObject/UI/New Panel", false, -2061)]
        static public void AddPanel(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreatePanel(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var image = go.GetComponent<Image>();
            _graphicSetting(image, CustomizeUGUICreate.IsRayCastTarget);

            // Panel is special, we need to ensure there's no padding after repositioning.
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }

        [MenuItem("GameObject/UI/New Scroll View", false, -2062)]
        static public void AddScrollView(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateScrollView(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var viewport = go.transform.Find("Viewport").gameObject;

            var viewportImage = viewport.GetComponent<Image>();
            _graphicSetting(viewportImage, CustomizeUGUICreate.IsRayCastTarget);
            
            if (!CustomizeUGUICreate.NoToRectMask)
            {
                Object.DestroyImmediate(viewport.GetComponent<Mask>());
                viewport.AddComponent<RectMask2D>();
            }
            
            var goImage = go.GetComponent<Image>();
            _graphicSetting(goImage, true);
            
            var Scrollbar_Horizontal = go.transform.Find("Scrollbar Horizontal").gameObject;
            var Scrollbar_Vertical = go.transform.Find("Scrollbar Vertical").gameObject;
            _setScrollbar(Scrollbar_Horizontal);
            _setScrollbar(Scrollbar_Vertical);
        }

        // Helper methods

        static public GameObject CreateNewUI()
        {
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer(kUILayerName);
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            CreateEventSystem(false);
            return root;
        }

        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            var esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        static public GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return UGUISource_MenuOptions.CreateNewUI();
        }
    }
}