using UnityEngine;
namespace HelloMod
{
    public class Main : IMod
    {
        public string Identifier { get; set; }
        
        public void onEnabled()
        {
			ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.AddComponent <PreciseModify>();

		}

        public void onDisabled()
        {
			UnityEngine.Object.Destroy (ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.GetComponent<PreciseModify> ());
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
