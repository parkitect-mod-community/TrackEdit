using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;


namespace RollercoasterEdit
{
    public class Configuration
    {
        private static GUIStyle ToggleButtonStyleNormal = null;
        private static GUIStyle ToggleButtonStyleToggled = null;

        public Settings settings { get; private set; }
        private string path;

        public int keySelectionId = -1;


        public Configuration(string path)
        {
            this.path =  path + System.IO.Path.DirectorySeparatorChar + "Config.json"; 
            settings = new Settings ();

        }

        public void Save()
        {
            SerializationContext context = new SerializationContext(SerializationContext.Context.Savegame);

            using (var stream = new FileStream(this.path, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(Json.Serialize(Serializer.serialize(context, settings)));
                }
            }
        }

        public void Load()
        {
            try {
                if (File.Exists(this.path)) {
                    using (StreamReader reader = new StreamReader(this.path)) {
                        string jsonString;

                        SerializationContext context = new SerializationContext(SerializationContext.Context.Savegame);
                        while((jsonString = reader.ReadLine()) != null) {
                            Dictionary<string,object> values = (Dictionary<string,object>)Json.Deserialize(jsonString);
                            Serializer.deserialize(context, settings, values);
                        }

                        reader.Close();
                    }
                }

            }
            catch (System.Exception e) {

                UnityEngine.Debug.Log("Couldn't properly load settings file! " + e.ToString());
            }
        }


        public void DrawGUI()
        {
            if ( ToggleButtonStyleNormal == null )
            {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }

            Event e = Event.current;
            GUILayout.BeginHorizontal();
            GUILayout.Label ("Vertical Drag:");
            settings.verticalKey =  KeyToggle (settings.verticalKey,e,0);
            GUILayout.EndHorizontal();
        }

        public string KeyToggle(string character,Event e,int id)
        {
            if ( GUILayout.Button(character , keySelectionId == id ? ToggleButtonStyleToggled : ToggleButtonStyleNormal ) )
            {
                keySelectionId = id;
            }

            if (keySelectionId == id) {
                if (e.isKey) {
                    keySelectionId = -1;
                    return e.keyCode.ToString ();
                }
            }
            return character;
            //selectedKey = Keyb
        }


    }
}

