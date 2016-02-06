using System;
using UnityEngine;

namespace HelloMod
{
	public class VectorUI
	{
		public CubicBezier Value{ get;private set;}

		private string[] p0 = {"","",""};
		private string[] p1 = {"","",""};
		private string[] p2 = {"","",""};
		private string[] p3 = {"","",""};


		public VectorUI (CubicBezier value)
		{
			this.Value = value;
			this.p0 =new string[]{Value.p0.x.ToString(),Value.p0.y.ToString(),Value.p0.z.ToString()};
			this.p1 = new string[]{Value.p1.x.ToString(),Value.p1.y.ToString(),Value.p1.z.ToString()};;
			this.p2 = new string[]{Value.p2.x.ToString(),Value.p2.y.ToString(),Value.p2.z.ToString()};;
			this.p3 = new string[]{Value.p3.x.ToString(),Value.p3.y.ToString(),Value.p3.z.ToString()};;
		}
			
		public void GUI()
		{
			VectorGUI (p0);
			VectorGUI (p1);
			VectorGUI (p2);
			VectorGUI (p3);
		}


		private void VectorGUI(string[] v)
		{

			GUILayout.BeginHorizontal ();
			v[0]=GUILayout.TextField (v[0]);
			v[1]=GUILayout.TextField (v[1]);
			v[2]=GUILayout.TextField (v[2]);
			GUILayout.EndHorizontal ();

		}

		public void UpdateVector()
		{
			Value.p0 = GetVector(this.p0);
			Value.p1 = GetVector(this.p1);
			Value.p2 = GetVector(this.p2);
			Value.p3 = GetVector(this.p3);


		}

		private Vector3 GetVector(string[] value)
		{
			float result;
			Vector3 output = new Vector3 ();
			if (float.TryParse (value[0], out result)) {
				output.x = result;
			}
			if (float.TryParse (value[1], out result)) {
				output.y = result;
			}
			if (float.TryParse (value[2], out result)) {
				output.z = result;
			}
			return output;
		}


	}
}

