#if false
using KSP.Localization;
using System;
using UnityEngine;

namespace EditorExtensionsRedux
{
	public class StrutWindozw : GUIWindow
	{
		internal override void Awake ()
		{
			base.Awake ();
			_windowTitle = Localizer.Format("#LOC_EEX_150");
		}

		bool _toggle = false;
		internal override void WindowContent (int windowID)
		{
			_toggle = GUILayout.Toggle (_toggle, _toggle ? Localizer.Format("#LOC_EEX_151") : Localizer.Format("#LOC_EEX_152"), Localizer.Format("#LOC_EEX_153"));

			if (GUILayout.Button (Localizer.Format("#LOC_EEX_51"))) {
				CloseWindow ();
			}

			GUI.DragWindow ();
		}
	}
}

#endif
