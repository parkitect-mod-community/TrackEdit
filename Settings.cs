using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class Settings : SerializedRawObject 
    {
        [Serialized]
        public KeyCode verticalKey{ get; set;}

        public Settings ()
        {
            verticalKey = KeyCode.LeftControl;
        }

    }
}

