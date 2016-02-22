using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace RollercoasterEdit
{
    public class Configuration
    {
        public UnityEngine.KeyCode VerticalKey{ get; set;}

        public Configuration()
        {
			VerticalKey = UnityEngine.KeyCode.LeftShift;
        }

        public void Save(string path)
        {
            Dictionary<string,object> values = new Dictionary<string, object>();
            values.Add ("OnSelect",  VerticalKey.ToString());
            File.WriteAllText (path+ System.IO.Path.DirectorySeparatorChar + "Config.json", MiniJSON.Json.Serialize (values));
        }

        public void Load(string path)
        {
            if (File.Exists (path + System.IO.Path.DirectorySeparatorChar + "Config.json")) {

                Dictionary<string,object> values = (Dictionary<string,object>)MiniJSON.Json.Deserialize (File.ReadAllText (path + System.IO.Path.DirectorySeparatorChar + "Config.json"));
                VerticalKey = (UnityEngine.KeyCode)Enum.Parse (typeof(UnityEngine.KeyCode), (string)values ["OnSelect"]);
            }
        }

    }
}

