using System;
using Parkitect.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RollercoasterEdit
{
    public class TrackEditUI : MonoBehaviour
    {
        public Toggle chainToggle;

        void Start()
        {
            chainToggle = this.transform.FindRecursive ("ChainBuilderToggle").GetComponent<Toggle> ();
               
        }
    }
}

