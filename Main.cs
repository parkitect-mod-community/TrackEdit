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


            //GameObject container = UnityEngine.GameObject.Instantiate (Main.AssetBundleManager.UiContainerWindowGo);
            //container.transform.SetParent(ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.transform);



            Transform headerPanel= UnityEngine.Object.Instantiate (Main.AssetBundleManager.UiHeaderPanelGo).transform;
            headerPanel.transform.SetParent( ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.transform);
            headerPanel.transform.name = "HeaderPanel";

            Transform mainBody =  UnityEngine.Object.Instantiate (Main.AssetBundleManager.UiContainerWindowGo).transform;
            mainBody.transform.SetParent ( ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.transform);
            mainBody.SetSiblingIndex (1);
            mainBody.transform.name = "TrackEditPanel";
            UnityEngine.Debug.Log (mainBody.name);

		}

        public void onDisabled()
        {
			UnityEngine.Object.Destroy (ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.GetComponent<TrackUIHandle> ());
            UnityEngine.Object.Destroy (ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.transform.Find(Main.AssetBundleManager.UiContainerWindowGo.name));
            UnityEngine.Object.Destroy (ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.transform.Find(Main.AssetBundleManager.UiHeaderPanelGo.name));
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
