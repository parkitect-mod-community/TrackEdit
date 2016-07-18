using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class ModSettings : SerializedRawObject 
    {
        [Serialized]
        public KeyCode verticalKey{ get; set;}

        public ModSettings ()
        {
            verticalKey = KeyCode.LeftShift;
        }

    }
}

