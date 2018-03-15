using System;
using UnityEngine;
using ClickThroughFix;

namespace EditorExtensionsRedux
{
	public abstract class GUIWindow : MonoBehaviour
	{
		//public static SettingsWindow Instance { get; private set; }
		//public bool Visible { get; set; }

		public delegate void WindowDisabledEventHandler();
		public event WindowDisabledEventHandler WindowDisabled;
		protected virtual void OnWindowDisabled() 
		{
			if (WindowDisabled != null)
				WindowDisabled();
		}

		internal string _windowTitle = string.Empty;

		internal Rect _windowRect = new Rect () {
			xMin = Screen.width/2 - 100,
			xMax = Screen.width/2 + 100,
			yMin = Screen.height/2 - 50,
			yMax = Screen.height/2 + 50 //0 height, GUILayout resizes it
		};

		//ctor
		public GUIWindow ()
		{
			//start disabled
			this.enabled = false;
		}

		internal virtual void Awake ()
		{
		}

		internal virtual void Update ()
		{
		}

		internal virtual void OnEnable ()
		{
		}

		internal virtual void CloseWindow(){
			this.enabled = false;
			OnWindowDisabled ();
		}

		internal virtual void OnDisable(){
		}

		internal virtual void OnGUI ()
		{
			if (Event.current.type == EventType.Layout) {
				_windowRect.yMax = _windowRect.yMin;
				_windowRect = ClickThruBlocker.GUILayoutWindow (this.GetInstanceID (), _windowRect, WindowContent, _windowTitle);
			}
		}

		void OnDestroy ()
		{
		}

		/// <summary>
		/// Initializes the window content and enables it
		/// </summary>
		public void Show ()
		{
			this.enabled = true;
		}

		internal abstract void WindowContent (int windowID);
	}
}

