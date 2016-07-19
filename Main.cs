using UnityEngine;
using Parkitect.UI;
using UnityEngine.UI;


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


            GameObject container = UnityEngine.GameObject.Instantiate (Main.AssetBundleManager.UiContainerWindowGo);
            container.transform.SetParent(ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.transform);


            /*GameObject hider = new GameObject ();
            hider.AddComponent<RectTransform> ();
            hider.AddComponent<Mask> ();
            hider.AddComponent<LayoutElement> ().minHeight = 0;

            hider.transform.SetParent (ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.transform);*/
            //ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.transform.Find ("UpperModules").gameObject.AddComponent<Mask>();
            //ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.transform.Find ("UpperModules").gameObject.GetComponent<LayoutElement> ().minHeight = 1;

            //ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.transform.Find ("UpperModules").gameObject.AddComponent<Mask> ();
            //ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.transform.Find ("UpperModules").gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 0);

           // ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.GetComponent<UIWindowSettings> ().resizeability = UIWindowSettings.Resizeability.Horizontal | UIWindowSettings.Resizeability.Vertical;

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
            get { return "Allows the user to modify track paths"; }
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
