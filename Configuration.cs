using System;
using System.Collections.Generic;
using System.IO;
using MiniJSON;
using UnityEngine;

namespace TrackEdit
{
    public class Configuration
    {
        private static GUIStyle _toggleButtonStyleNormal;
        private static GUIStyle _toggleButtonStyleToggled;
        private readonly string _path;

        private int _keySelectionId = -1;

        public Configuration()
        {
            _path = FilePaths.getFolderPath("track_edit.config");
            Settings = new ModSettings();
        }

        public ModSettings Settings { get; }

        public void Save()
        {
            var context = new SerializationContext(SerializationContext.Context.Savegame);

            using (var stream = new FileStream(_path, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(Json.Serialize(Serializer.serialize(context, Settings)));
                }
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(_path))
                    using (var reader = new StreamReader(_path))
                    {
                        string jsonString;

                        var context = new SerializationContext(SerializationContext.Context.Savegame);
                        while ((jsonString = reader.ReadLine()) != null)
                        {
                            var values = (Dictionary<string, object>) Json.Deserialize(jsonString);
                            Serializer.deserialize(context, Settings, values);
                        }

                        reader.Close();
                    }
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't properly load settings file! " + e);
            }
        }


        public void DrawGui()
        {
            if (_toggleButtonStyleNormal == null)
            {
                _toggleButtonStyleNormal = "Button";
                _toggleButtonStyleToggled = new GUIStyle(_toggleButtonStyleNormal);
                _toggleButtonStyleToggled.normal.background = _toggleButtonStyleToggled.active.background;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Vertical Drag:");
            Settings.VerticalKey = KeyToggle(Settings.VerticalKey, 0);
            GUILayout.EndHorizontal();
        }

        public KeyCode KeyToggle(KeyCode character, int id)
        {
            if (GUILayout.Button(character.ToString(),
                _keySelectionId == id ? _toggleButtonStyleToggled : _toggleButtonStyleNormal)) _keySelectionId = id;

            if (_keySelectionId == id)
            {
                KeyCode e;
                if (FetchKey(out e))
                {
                    _keySelectionId = -1;
                    return e;
                }
            }

            return character;
            //selectedKey = Keyb
        }

        private bool FetchKey(out KeyCode outKey)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                if (Input.GetKeyDown(key))
                {
                    outKey = key;
                    return true;
                }

            outKey = KeyCode.A;
            return false;
        }
    }
}