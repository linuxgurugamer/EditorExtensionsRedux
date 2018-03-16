using System;
using UnityEngine;

namespace EditorExtensionsRedux
{
	public class LineDrawer : MonoBehaviour
	{
		public LineDrawer ()
		{}

		const string shaderName = "Particles/Alpha Blended";
		Material material;

		protected virtual void Awake ()
		{
			material = new Material (Shader.Find (shaderName));
		}

		protected virtual void Start ()
		{

		}

		protected virtual void LateUpdate ()
		{
		}

		protected LineRenderer newLine ()
		{
			var obj = new GameObject("EditorExtensions.LineDrawer");
			var lr = obj.AddComponent<LineRenderer>();
			obj.transform.parent = gameObject.transform;
			obj.transform.localPosition = Vector3.zero;
			lr.material = material;
			return lr;
		}

		void DrawLine(Vector3 start, Vector3 end)
		{
			LineRenderer line = gameObject.AddComponent<LineRenderer>();
			//line.SetColors (Color.blue, Color.blue);
            line.startColor = Color.blue;
            line.endColor = Color.blue;
			//line.useWorldSpace = false;
			//line.SetVertexCount (2);
            line.positionCount = 2;
			line.SetPosition(0, start);
			line.SetPosition(1, end);
			//line.SetWidth (0.5f, 0.1f);
            line.startWidth = 0.5f;
            line.endWidth = 0.1f;
			line.material = material;

		}
	}
}

