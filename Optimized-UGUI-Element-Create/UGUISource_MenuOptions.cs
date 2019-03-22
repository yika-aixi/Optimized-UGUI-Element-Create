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
    public class UGUISource_MenuOptions
    {
        private const string kUILayerName = "UI";

        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string kMaskPath = "UI/Skin/UIMask.psd";

        static private DefaultControls.Resources s_StandardResources;

        static private DefaultControls.Resources GetStandardResources()
        {
            if (s_StandardResources.standard == null)
            {
                s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
                s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
                s_StandardResources.inputField =
                    AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
                s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
                s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
                s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
                s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
            }

            return s_StandardResources;
        }

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
            image.sprite = CustomizeUGUICreate.GetDefalutSprite(image.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(image.gameObject) : image.sprite;
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
            image.sprite = CustomizeUGUICreate.GetDefalutSprite(image.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(image.gameObject) : image.sprite;
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

            background.sprite = CustomizeUGUICreate.GetDefalutSprite(background.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(background.gameObject) : background.sprite;
            background.material = CustomizeUGUICreate.GetDefalutMaterial(background.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(background.gameObject) : background.material;

            var checkmark = background.transform.Find("Checkmark").GetComponent<Image>();
            
            checkmark.sprite = CustomizeUGUICreate.GetDefalutSprite(checkmark.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(checkmark.gameObject) : checkmark.sprite;
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
            
            Background.sprite = CustomizeUGUICreate.GetDefalutSprite(Background.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Background.gameObject) : Background.sprite;
            Background.material = CustomizeUGUICreate.GetDefalutMaterial(Background.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Background.gameObject) : Background.material;

            Fill.sprite = CustomizeUGUICreate.GetDefalutSprite(Fill.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Fill.gameObject) : Fill.sprite;
            Fill.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Fill.material = CustomizeUGUICreate.GetDefalutMaterial(Fill.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Fill.gameObject) : Fill.material;
            
            Handle.sprite = CustomizeUGUICreate.GetDefalutSprite(Handle.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Handle.gameObject) : Handle.sprite;
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
            image.sprite = CustomizeUGUICreate.GetDefalutSprite(image.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(image.gameObject) : image.sprite;
            image.material = CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(image.gameObject) : image.material;

            var Handle = scrollbar.transform.Find("Sliding Area").GetChild(0).GetComponent<Image>();

            Handle.sprite = CustomizeUGUICreate.GetDefalutSprite(Handle.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Handle.gameObject) : Handle.sprite;
            Handle.material = CustomizeUGUICreate.GetDefalutMaterial(Handle.gameObject)? CustomizeUGUICreate.GetDefalutMaterial(Handle.gameObject) : Handle.material;
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
            
            Arrow.sprite = CustomizeUGUICreate.GetDefalutSprite(Arrow.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Arrow.gameObject) : Arrow.sprite;
            Arrow.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Arrow.material = CustomizeUGUICreate.GetDefalutMaterial(Arrow.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Arrow.gameObject) : Arrow.material;
            
            Template.sprite = CustomizeUGUICreate.GetDefalutSprite(Template.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Template.gameObject) : Arrow.sprite;
            Template.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Template.material = CustomizeUGUICreate.GetDefalutMaterial(Template.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Template.gameObject) : Template.material;
            
            Viewport.sprite = CustomizeUGUICreate.GetDefalutSprite(Viewport.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Viewport.gameObject) : Viewport.sprite;
            Viewport.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Viewport.material = CustomizeUGUICreate.GetDefalutMaterial(Viewport.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Viewport.gameObject) : Viewport.material;
            
            Item_Background.sprite = CustomizeUGUICreate.GetDefalutSprite(Item_Background.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Item_Background.gameObject) : Item_Background.sprite;
            Item_Background.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Item_Background.material = CustomizeUGUICreate.GetDefalutMaterial(Item_Background.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Item_Background.gameObject) : Item_Background.material;
            
            Item_Checkmark.sprite = CustomizeUGUICreate.GetDefalutSprite(Item_Checkmark.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Item_Checkmark.gameObject) : Item_Checkmark.sprite;
            Item_Checkmark.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Item_Checkmark.material = CustomizeUGUICreate.GetDefalutMaterial(Item_Checkmark.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Item_Checkmark.gameObject) : Item_Checkmark.material;
            
            var goImage = go.GetComponent<Image>();

            goImage.sprite = CustomizeUGUICreate.GetDefalutSprite(goImage.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(goImage.gameObject) : goImage.sprite;

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

            goImage.sprite = CustomizeUGUICreate.GetDefalutSprite(goImage.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(goImage.gameObject) : goImage.sprite;

            goImage.material = CustomizeUGUICreate.GetDefalutMaterial(goImage.gameObject)? CustomizeUGUICreate.GetDefalutMaterial(goImage.gameObject) : goImage.material;
        }
//
//        // Containers
//
//        [MenuItem("GameObject/UI/Canvas", false, -2060)]
//        static public void AddCanvas(MenuCommand menuCommand)
//        {
//            var go = CreateNewUI();
//            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
//            if (go.transform.parent as RectTransform)
//            {
//                RectTransform rect = go.transform as RectTransform;
//                rect.anchorMin = Vector2.zero;
//                rect.anchorMax = Vector2.one;
//                rect.anchoredPosition = Vector2.zero;
//                rect.sizeDelta = Vector2.zero;
//            }
//
//            Selection.activeGameObject = go;
//        }

        [MenuItem("GameObject/UI/New Panel", false, -2061)]
        static public void AddPanel(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreatePanel(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var image = go.GetComponent<Image>();
            image.sprite = CustomizeUGUICreate.GetDefalutSprite(image.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(image.gameObject) : image.sprite;
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

            var Viewport = go.transform.Find("Viewport").GetComponent<Image>();

            Viewport.sprite = CustomizeUGUICreate.GetDefalutSprite(Viewport.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(Viewport.gameObject) : Viewport.sprite;

            Viewport.raycastTarget = CustomizeUGUICreate.IsRayCastTarget;
            Viewport.material = CustomizeUGUICreate.GetDefalutMaterial(Viewport.gameObject) ? CustomizeUGUICreate.GetDefalutMaterial(Viewport.gameObject) : Viewport.material;

            var goImage = go.GetComponent<Image>();

            goImage.sprite = CustomizeUGUICreate.GetDefalutSprite(goImage.gameObject) ? CustomizeUGUICreate.GetDefalutSprite(goImage.gameObject) : goImage.sprite;
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

        [MenuItem("GameObject/UI/Event System", false, 2100)]
        public static void CreateEventSystem(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            CreateEventSystem(true, parent);
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