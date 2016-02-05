using UnityEngine;
namespace HelloMod
{
    public class Main : IMod
    {
        private GameObject _go;
        
        public string Identifier { get; set; }
        
        public void onEnabled()
        {
			 
			var t = ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.AddComponent <PreciseModify>();
        }

        public void onDisabled()
        {
        }

        public string Name
        {
            get { return "Hello Mod"; }
        }

        public string Description
        {
            get { return "Validates if mods are working on your PC"; }
        }
    }
}
