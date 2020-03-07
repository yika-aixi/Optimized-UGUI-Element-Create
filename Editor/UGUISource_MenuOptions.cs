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

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                parent = GetOrCreateCanvasGameObject();
            }

            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
            element.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, parent);
            if (parent != menuCommand.context) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(),
                    element.GetComponent<RectTransform>());

            Selection.activeGameObject = element;
        }

        // Graphic elements

        [MenuItem("GameObject/UI/New Text", false, -2000)]
        static public void AddText(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateText(GetStandardResources());
            Text text = go.GetComponent<Text>();
            text.supportRichText = CustomizeUGUICreate.IsRich;
            text.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            text.material = CustomizeUGUICreate.GetDefalutMaterial(text.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(text.gameObject) : text.material;
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/New Image", false, -2001)]
        static public void AddImage(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateImage(GetStandardResources());
            var image = go.GetComponent<Image>();
            image.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            image.material = CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) : image.material;
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/New Raw Image", false, -2002)]
        static public void AddRawImage(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateRawImage(GetStandardResources());
            var rawImage = go.GetComponent<RawImage>();
            rawImage.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            rawImage.material = CustomizeUGUICreate.GetDefalutMaterial(rawImage.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(rawImage.gameObject) : rawImage.material;
            PlaceUIElementRoot(go, menuCommand);
        }

        // Controls

        // Button and toggle are controls you just click on.

        [MenuItem("GameObject/UI/New Button", false, -2030)]
        static public void AddButton(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateButton(GetStandardResources());

            var image = go.GetComponent<Image>();
            image.material = CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) : image.material;

            var text = go.transform.GetChild(0).GetComponent<Text>();
            text.supportRichText = CustomizeUGUICreate.IsRich;
            text.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            text.material = CustomizeUGUICreate.GetDefalutMaterial(text.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(text.gameObject) : text.material;
            
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/New Toggle", false, -2031)]
        static public void AddToggle(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateToggle(GetStandardResources());

            var background = go.transform.Find("Background").GetComponent<Image>();
            background.type = Image.Type.Simple;
            background.material = CustomizeUGUICreate.GetDefalutMaterial(background.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(background.gameObject) : background.material;

            var checkmark = background.transform.Find("Checkmark").GetComponent<Image>();
            
            checkmark.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            checkmark.material = CustomizeUGUICreate.GetDefalutMaterial (checkmark.gameObject)? CustomizeUGUICreate.GetDefalutMaterial(checkmark.gameObject) : checkmark.material;
            
            var lable = go.transform.Find("Label").GetComponent<Text>();
            lable.supportRichText = CustomizeUGUICreate.IsRich;
            lable.material = CustomizeUGUICreate.GetDefalutMaterial(lable.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(lable.gameObject) : lable.material;
            
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
            
            Background.material = CustomizeUGUICreate.GetDefalutMaterial(Background.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Background.gameObject) : Background.material;
            Background.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            
            Fill.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Fill.material = CustomizeUGUICreate.GetDefalutMaterial(Fill.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Fill.gameObject) : Fill.material;
            
            Handle.material = CustomizeUGUICreate.GetDefalutMaterial(Handle.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Handle.gameObject) : Handle.material;
            
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
            image.material = CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) : image.material;

            var handle = scrollbar.transform.Find("Sliding Area").GetChild(0).GetComponent<Image>();

            handle.material = CustomizeUGUICreate.GetDefalutMaterial(handle.gameObject)? CustomizeUGUICreate.GetDefalutMaterial(handle.gameObject) : handle.material;
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
            
            Label.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Label.supportRichText = CustomizeUGUICreate.IsRich;
            Label.material = CustomizeUGUICreate.GetDefalutMaterial(Item_Label.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Item_Label.gameObject) : Label.material;
            
            Item_Label.material = CustomizeUGUICreate.GetDefalutMaterial(Item_Label.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Item_Label.gameObject) : Item_Label.material;
            Item_Label.supportRichText = CustomizeUGUICreate.IsRich;
            
            Arrow.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Arrow.material = CustomizeUGUICreate.GetDefalutMaterial(Arrow.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Arrow.gameObject) : Arrow.material;
            
            Template.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Template.material = CustomizeUGUICreate.GetDefalutMaterial(Template.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Template.gameObject) : Template.material;
            
            Viewport.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Viewport.material = CustomizeUGUICreate.GetDefalutMaterial(Viewport.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Viewport.gameObject) : Viewport.material;
            
            Item_Background.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Item_Background.material = CustomizeUGUICreate.GetDefalutMaterial(Item_Background.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Item_Background.gameObject) : Item_Background.material;
            
            Item_Checkmark.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Item_Checkmark.material = CustomizeUGUICreate.GetDefalutMaterial(Item_Checkmark.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Item_Checkmark.gameObject) : Item_Checkmark.material;
            
            var goImage = go.GetComponent<Image>();

            goImage.material = CustomizeUGUICreate.GetDefalutMaterial(goImage.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(goImage.gameObject) : goImage.material;
        }

        [MenuItem("GameObject/UI/New Input Field", false, -2036)]
        public static void AddInputField(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateInputField(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var Placeholder = go.transform.Find("Placeholder").GetComponent<Text>();
            Placeholder.supportRichText = CustomizeUGUICreate.IsRich;
            Placeholder.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Placeholder.material = CustomizeUGUICreate.GetDefalutMaterial(Placeholder.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Placeholder.gameObject) : Placeholder.material;

            var Text = go.transform.Find("Text").GetComponent<Text>();
            Text.supportRichText = CustomizeUGUICreate.IsRich;
            Text.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Text.material = CustomizeUGUICreate.GetDefalutMaterial(Text.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Text.gameObject): Text.material;

            var goImage = go.GetComponent<Image>();

            goImage.material = CustomizeUGUICreate.GetDefalutMaterial(goImage.gameObject)? CustomizeUGUICreate.GetDefalutMaterial(goImage.gameObject) : goImage.material;
        }

        [MenuItem("GameObject/UI/New Panel", false, -2061)]
        static public void AddPanel(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreatePanel(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var image = go.GetComponent<Image>();
            image.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            image.material = CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) : image.material;
            
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

            if (CustomizeUGUICreate.NoToRectMask)
            {
                var viewportImage = viewport.GetComponent<Image>();

                viewportImage.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;

                Material material = CustomizeUGUICreate.GetDefalutMaterial(viewport.gameObject);

                if (!material)
                {
                    material = viewportImage.material;
                }

                viewportImage.material = material;

            }
            else
            {
                Object.DestroyImmediate(viewport.GetComponent<Image>());
                Object.DestroyImmediate(viewport.GetComponent<Mask>());
                viewport.AddComponent<RectMask2D>();
            }


            var goImage = go.GetComponent<Image>();

            goImage.material = CustomizeUGUICreate.GetDefalutMaterial(goImage.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(goImage.gameObject) : goImage.material;

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