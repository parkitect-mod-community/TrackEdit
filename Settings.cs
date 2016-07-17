using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class Settings : SerializedRawObject 
    {
        [Serialized]
        public String verticalKey{ get; set;}

        public Settings ()
        {
            verticalKey = KeyCode.LeftControl.ToString ();
        }
    }
}

