//Create: Icarus
//ヾ(•ω•`)o
//2020-12-17 05:54
//CabinIcarus.UGUI.OptimizedElement

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CabinIcarus.UGUI.OptimizedElement
{
    public static class Menus
    {
        [MenuItem("GameObject/UI/Select All Current Scene Null Source Image", false, -9999)]
        static void _selectCurrentOpenSceneAllNullSourceImage(MenuCommand menuCommand)
        {
            List<GameObject> objs = new List<GameObject>();
            
            foreach (var image in _getAllOpenSceneComponents<Image>())
            {
                if (image.sprite == null)
                {
                    objs.Add(image.gameObject);
                }
            }

            Selection.objects = objs.ToArray();
        }
        
        [MenuItem("GameObject/UI/Select All Current Scene Image", false, -9999)]
        static void _selectCurrentOpenSceneAllImage(MenuCommand menuCommand)
        {
            List<GameObject> objs = new List<GameObject>();
            
            foreach (var image in _getAllOpenSceneComponents<Image>())
            {
                objs.Add(image.gameObject);
            }

            Selection.objects = objs.ToArray();
        }
        
        
        [MenuItem("GameObject/UI/Select All Current Scene Text", false, -9999)]
        static void _selectCurrentOpenSceneAllText(MenuCommand menuCommand)
        {
            List<GameObject> objs = new List<GameObject>();
            
            foreach (var component in _getAllOpenSceneComponents<Text>())
            {
                objs.Add(component.gameObject);
            }

            Selection.objects = objs.ToArray();
        }

        static IEnumerable<T> _getAllOpenSceneComponents<T>(bool includeInactive = true)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                var roots = scene.GetRootGameObjects();
                
                foreach (var root in roots)
                {
                    var componets = root.GetComponentsInChildren<T>(includeInactive);

                    foreach (var component in componets)
                    {
                        yield return component;
                    }
                }
            }
        }
    }
}