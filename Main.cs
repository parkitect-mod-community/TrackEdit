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

    public class Main : AbstractMod, IModSettings
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


        public override string getName()
        {
            return "Track Edit 2";
        }

        public override string getDescription()
        {
            return "Allows the user to modify track paths";
        }

        public override string getVersionNumber()
        {
            return "1.0";
        }

        public override string getIdentifier()
        {
            return "TrackEdit_2";
        }

        public override void onEnabled()
        {
            base.onEnabled();

            Global.NO_TRACKBUILDER_RESTRICTIONS = true;

            if (Configuration == null)
            {
                Configuration = new Configuration();
                Configuration.Load();
                Configuration.Save();
            }

            ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject
                .AddComponent<TrackEditHandler>();
        }

        public override void onDisabled()
        {
            Global.NO_TRACKBUILDER_RESTRICTIONS = false;

            Object.Destroy(ScriptableSingleton<UIAssetManager>.Instance.trackBuilderWindowGO.gameObject
                .GetComponent<TrackEditHandler>());
        }

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
