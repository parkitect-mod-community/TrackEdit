/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System.Linq;
using UnityEngine;

namespace TrackEdit
{
    public class Main : IMod, IModSettings
    {
        public static AssetBundleManager AssetBundleManager;
        public static Configuration Configuration;
        public string Identifier { get; set; }

        public string Path
        {
            get
            {
                var path = ModManager.Instance.getModEntries().First(x => x.mod == this).path;
                return path;
            }
        }


        public void onEnabled()
        {
            Debug.Log(Application.unityVersion);


            Global.NO_TRACKBUILDER_RESTRICTIONS = true;

            if (Configuration == null)
            {
                Configuration = new Configuration();
                Configuration.Load();
                Configuration.Save();
            }

            if (AssetBundleManager == null) AssetBundleManager = new AssetBundleManager(this);


            ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.AddComponent<TrackUiHandle>();

            //GameObject container = UnityEngine.GameObject.Instantiate (Main.AssetBundleManager.UiContainerWindowGo);
            //container.transform.SetParent(ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.transform);


            /*Transform headerPanel= UnityEngine.Object.Instantiate (Main.AssetBundleManager.UiHeaderPanelGo).transform;
            headerPanel.transform.SetParent( ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.transform);
            headerPanel.transform.name = "HeaderPanel";

            Transform mainBody =  UnityEngine.Object.Instantiate (Main.AssetBundleManager.UiContainerWindowGo).transform;
            mainBody.transform.SetParent ( ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.transform);
            mainBody.SetSiblingIndex (1);
            mainBody.transform.name = "TrackEditPanel";
            UnityEngine.Debug.Log (mainBody.name);*/
        }

        public void onDisabled()
        {
            Global.NO_TRACKBUILDER_RESTRICTIONS = false;

            Object.Destroy(ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject
                .GetComponent<TrackUiHandle>());
            /*Object.Destroy(
                ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.transform.Find(AssetBundleManager
                    .UiContainerWindowGo.name));
            Object.Destroy(
                ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.transform.Find(AssetBundleManager
                    .UiHeaderPanelGo.name));*/
        }

        public string Name => "Track Edit";

        public string Description => "Allows the user to modify track paths";

        string IMod.Identifier => "TrackEdit";

        public void onDrawSettingsUI()
        {
            Configuration.DrawGui();
        }

        public void onSettingsOpened()
        {
            if (Configuration == null)
                Configuration = new Configuration();
            Configuration.Load();
        }

        public void onSettingsClosed()
        {
            Configuration.Save();
        }
    }
}