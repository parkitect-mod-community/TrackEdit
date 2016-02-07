using UnityEngine;
namespace HelloMod
{
    public class Main : IMod
    {
        public string Identifier { get; set; }
        
        public void onEnabled()
        {
			ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.AddComponent <TrackUIHandle>();

		}

        public void onDisabled()
        {
			UnityEngine.Object.Destroy (ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.GetComponent<TrackUIHandle> ());
        }

        public string Name
        {
            get { return "Track Edit"; }
        }

        public string Description
        {
            get { return "Allows the User to modify track Path"; }
        }
    }
}
