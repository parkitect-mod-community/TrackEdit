using UnityEngine;
namespace RollercoasterEdit
{
    public class Main : IMod
    {
        public string Identifier { get; set; }
		public static AssetBundleManager AssetBundleManager = null;
        public static Configuration Configeration = null;

        private bool _yDragToggle = false;

        public void onEnabled()
        {
            if (Main.Configeration == null) {
                Configeration = new Configuration ();
                Configeration.Load (Path);
            }

			if (Main.AssetBundleManager == null) {

				AssetBundleManager = new AssetBundleManager (this);
			}
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


		public string Path { get; set; }

		public void onDrawSettingsUI()
		{

            GUILayout.BeginHorizontal();
            GUILayout.Label ("Vertical Drag Key");
            _yDragToggle = GUILayout.Toggle (_yDragToggle, Configeration.VerticalKey.ToString ());
            if (_yDragToggle) {
                KeyCode key;
                if (FetchKey (out key)) {
                    Configeration.VerticalKey = key;
                }
            }
            
           GUILayout.EndHorizontal();

		}

        private bool FetchKey(out KeyCode outKey)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown (key)) {
                    outKey = key;
                    return true;
                }
            }
            outKey = KeyCode.A;
            return false;
        }

		public void onSettingsOpened()
		{

		}

		public void onSettingsClosed()
		{
            Main.Configeration.Save (Path);
		}
    }
}
