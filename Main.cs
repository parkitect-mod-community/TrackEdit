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

        public static Configuration Configuration;
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
            Global.NO_TRACKBUILDER_RESTRICTIONS = true;

            if (Configuration == null)
            {
                Configuration = new Configuration();
                Configuration.Load();
                Configuration.Save();
            }

//            GameObject go =  ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject;
//            TrackBuilder trackBuilder = go.GetComponent<TrackBuilder>();

            ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject.AddComponent<TrackEditHandler>();
        }

        public void onDisabled()
        {
            Global.NO_TRACKBUILDER_RESTRICTIONS = false;

            Object.Destroy(ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject
                .GetComponent<TrackEditHandler>());
        }

        public string Name => "Track Edit 2";

        public string Description => "Allows the user to modify track paths";

        string IMod.Identifier => "TrackEdit_2";

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
