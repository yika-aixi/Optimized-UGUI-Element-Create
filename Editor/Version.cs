//Create: Icarus
//ヾ(•ω•`)o
//2021-02-07 11:36
//CabinIcarus.UGUI.OptimizedElement

using CabinIcarus.EditorFrame.Utils;
using UnityEditor;

namespace CabinIcarus.UGUI.OptimizedElement
{
    public static class Version
    {
        public const string Symbol = "CABNINICARUS_UGUI_OPTIMIZEDELEMENT";
        
        [InitializeOnLoadMethod]
        static void _addSymbol()
        {
            ScriptingDefineUtil.AddScriptingDefine(Symbol);
        }
    }
}