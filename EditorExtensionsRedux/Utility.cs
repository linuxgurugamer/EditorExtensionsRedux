using System;
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
}

