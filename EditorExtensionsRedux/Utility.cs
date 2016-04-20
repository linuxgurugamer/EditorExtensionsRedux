using System;
using System.Reflection;
using UnityEngine;

namespace EditorExtensionsRedux
{
	public static class Utility
	{
		public static Part GetPartUnderCursor ()
		{
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			EditorLogic ed = EditorLogic.fetch;

			if (ed != null && Physics.Raycast (ray, out hit)) {
				return ed.ship.Parts.Find (p => p.gameObject == hit.transform.gameObject);
			}
			return null;
		}
	}

	//		void HighlightPart(Part p){
	//			// old highlighter. Not necessary, but looks nice in combination
	//			p.SetHighlightDefault();
	//			p.SetHighlightType(Part.HighlightType.AlwaysOn);
	//			p.SetHighlight(true, false);
	//			p.SetHighlightColor(Color.red);
	//
	//			// New highlighter
	//			HighlightingSystem.Highlighter hl; // From Assembly-CSharp-firstpass.dll
	//			hl = p.FindModelTransform("model").gameObject.AddComponent<HighlightingSystem.Highlighter>();
	//			hl.ConstantOn(XKCDColors.Rust);
	//			hl.SeeThroughOn();
	//		}

	public static class Refl {
		public static FieldInfo GetField(object obj, string name) {
			var f = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if(f == null) throw new Exception("No such field: " + obj.GetType() + "#" + name);
			return f;
		}
		public static object GetValue(object obj, string name) {
			return GetField(obj, name).GetValue(obj);
		}
		public static void SetValue(object obj, string name, object value) {
			GetField(obj, name).SetValue(obj, value);
		}
		public static MethodInfo GetMethod(object obj, string name) {
			var m = obj.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if(m == null) throw new Exception("No such method: " + obj.GetType() + "#" + name);
			return m;
		}
		public static object Invoke(object obj, string name, params object[] args) {
			return GetMethod(obj, name).Invoke(obj, args);
		}
	}
}

