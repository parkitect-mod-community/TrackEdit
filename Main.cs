using UnityEngine;
namespace RollercoasterEdit
{
    public class Main : IMod , IModSettings 
    {
        public string Identifier { get; set; }
		public static AssetBundleManager AssetBundleManager = null;
        public static Configuration configuration = null;


        public void onEnabled()
        {
            if (Main.configuration == null) {
                Main.configuration = new Configuration (Path);
                Main.configuration.Load ();
                Main.configuration.Save ();

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

        public void onDrawSettingsUI() {
            Main.configuration.DrawGUI ();
        }

        public void onSettingsOpened() {
            if (Main.configuration == null)
                Main.configuration = new Configuration (this.Path);
            Main.configuration.Load ();

        }
        public void onSettingsClosed() {
            Main.configuration.Save ();
        }
    }
}
