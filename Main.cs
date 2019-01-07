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