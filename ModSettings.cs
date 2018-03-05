using UnityEngine;

namespace TrackEdit
{
    public class ModSettings : SerializedRawObject
    {
        public ModSettings()
        {
            VerticalKey = KeyCode.LeftShift;
        }

        [Serialized] public KeyCode VerticalKey { get; set; }
    }
}