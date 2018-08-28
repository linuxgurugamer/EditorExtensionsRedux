using System;
using UnityEngine;
using ClickThroughFix;

#if true
namespace EditorExtensionsRedux
{
	public class FineAdjustWindow : MonoBehaviour
	{
		Rect _windowRect = new Rect () {
			xMin = Screen.width - 325,
			xMax = Screen.width - 50,
			yMin = 50,
			yMax = 50 //0 height, GUILayout resizes it
		};


		string _windowTitle = string.Empty;

		public static FineAdjustWindow Instance { get; private set; }

		void Awake ()
		{
			Log.Debug ("FineAdjustWindow Awake()");
			this.enabled = false;
			Instance = this;
		}

		void Start()
		{
			
		}

		void OnEnable ()
		{
			Log.Debug ("FineAdjustWindow OnEnable()");
		
		}

		public bool isEnabled()
		{
			return this.enabled;
		}

		void CloseWindow ()
		{
			this.enabled = false;
			Log.Info ("CloseWindow enabled: " + this.enabled.ToString ());
		}

		void OnDisable ()
		{
			
		}

		void OnGUI ()
		{
			if (isEnabled() ) {
				_windowTitle = string.Format ("Fine Adjustments");
				var tstyle = new GUIStyle (GUI.skin.window);

				if (fineAdjustActive) {
					_windowTitle = _windowTitle + " Active";
					tstyle.normal.textColor = Color.yellow;
				}
				//_windowRect.yMax = _windowRect.yMin;
				_windowRect = ClickThruBlocker.GUILayoutWindow (this.GetInstanceID (), _windowRect, WindowContent, _windowTitle, tstyle);
			}
		}

		enum AdjustmentType
		{
			translation,
			rotation,
		};

		Part activePuc;
		Part oldActivePuc;
		bool fineAdjustActive = false;


		AdjustmentType adjType = AdjustmentType.translation;
		string adjTypeStr = "Translation";
		public float offset = 0.01f;
		public float rotation = 1.0f;
		public int offsetDeltaIndex = 2;
		public int rotationdeltaIndex = 0;

		float getDelta(int i) {
			switch (i) {
			case 0:
				return 1.0f;
			case 1:
				return 0.1f;
			case 2:
				return 0.01f;
			case 3:
				return 0.001f;
			}
			return 0.1f;
		}

//		private string[] _toolbarStrings = { "Translation", "Rotation" };
//		int toolbarInt = 0;


		void WindowContent (int windowID)
		{
			
			//GUI.skin = HighLogic.Skin;
			var lstyle = new GUIStyle (GUI.skin.label);
			//var errstyle = new GUIStyle (GUI.skin.label);
			//errstyle.normal.textColor = Color.red;
			if (fineAdjustActive  && (DateTime.Now.Second % 2 == 0)  ) {
				lstyle.normal.textColor = Color.yellow;
			}
			Part puc = null; 

			if (fineAdjustActive)
				puc = activePuc;
			else
				puc = EditorLogic.SelectedPart; //Utility.GetPartUnderCursor ();
			
//			toolbarInt = GUILayout.Toolbar (toolbarInt, _toolbarStrings);
			//adjTypeStr = "None";
			if (GizmoEvents.offsetGizmoActive) {
				adjType = AdjustmentType.translation;
				adjTypeStr = "Translation";
			} 
			if (GizmoEvents.rotateGizmoActive) {
				adjType = AdjustmentType.rotation;
				adjTypeStr = "Rotation";
			}
		
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Adjustment Type: ", lstyle);
			GUILayout.Label (adjTypeStr, lstyle);
			GUILayout.EndHorizontal ();



	//		if (!GizmoEvents.offsetGizmoActive && !GizmoEvents.rotateGizmoActive)
	//			return;

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Current Part:", lstyle);
			GUILayout.Label (puc ? puc.name : "none", lstyle);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Symmetry Method: ", lstyle);
			if (puc != null)
				GUILayout.Label (puc.symMethod.ToString ());
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (adjType != AdjustmentType.translation || puc != EditorLogic.RootPart) {
				GUILayout.Label ("Delta amount:", lstyle, GUILayout.MinWidth (150));
				if (GUILayout.Button ("<", GUILayout.Width (20))) {
					switch (adjType) {
					case AdjustmentType.rotation:
						rotationdeltaIndex++;
						if (rotationdeltaIndex > 3)
							rotationdeltaIndex = 3;

						break;
					case AdjustmentType.translation:
						offsetDeltaIndex++;
						if (offsetDeltaIndex > 3)
							offsetDeltaIndex = 3;
						break;
					}
				}
				switch (adjType) {
				case AdjustmentType.rotation:
					GUILayout.Label (getDelta (rotationdeltaIndex).ToString (), "TextField");
					break;
				case AdjustmentType.translation:
					GUILayout.Label (getDelta (offsetDeltaIndex).ToString (), "TextField");
					break;
				}

				if (GUILayout.Button (">", GUILayout.Width (20))) {
					switch (adjType) {
					case AdjustmentType.rotation:
						rotationdeltaIndex--;
						if (rotationdeltaIndex < 0)
							rotationdeltaIndex = 0;
					
						break;
					case AdjustmentType.translation:
						offsetDeltaIndex--;
						if (offsetDeltaIndex < 0)
							offsetDeltaIndex = 0;
						break;
					}

				}
			}
			GUILayout.EndHorizontal ();


			GUILayout.BeginHorizontal ();
			if (adjType != AdjustmentType.translation || puc != EditorLogic.RootPart) {
				GUILayout.Label (adjTypeStr + " amount:", lstyle, GUILayout.MinWidth (150));
				if (GUILayout.Button ("-", GUILayout.Width (20))) {
					switch (adjType) {
					case AdjustmentType.rotation:
						rotation -= getDelta (rotationdeltaIndex);
						if (rotation <= 0.0f)
							rotation = getDelta (rotationdeltaIndex);
					
						break;
					case AdjustmentType.translation:
						offset -= getDelta (offsetDeltaIndex);
						if (offset <= 0.0f)
							offset = getDelta (offsetDeltaIndex);
					
						break;
					}
				}
				switch (adjType) {
				case AdjustmentType.rotation:
					GUILayout.Label (rotation.ToString (), "TextField");
					break;
				case AdjustmentType.translation:
					GUILayout.Label (offset.ToString (), "TextField");
					break;
				}

				if (GUILayout.Button ("+", GUILayout.Width (20))) {
					switch (adjType) {
					case AdjustmentType.rotation:
						rotation += getDelta (rotationdeltaIndex);
						break;
					case AdjustmentType.translation:
						offset += getDelta (offsetDeltaIndex);
						break;
					}

				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Done")) {
				Log.Info ("Done");
				fineAdjustActive = false;
				InputLockManager.RemoveControlLock ("EEX_FA");
				CloseWindow ();
			}
			GUILayout.EndHorizontal ();
			GUI.DragWindow ();
		}


		void LateUpdate ()
		{
			//Part sp = EditorLogic.SelectedPart;

			/* if (sp == null) */ {
				if (!fineAdjustActive) {
					activePuc = EditorLogic.SelectedPart; //Utility.GetPartUnderCursor ();
					if (activePuc != oldActivePuc) {
						oldActivePuc = activePuc;
//						if (HighLogic.FindObjectsOfType<EditorGizmos.GizmoOffset> ().Length > 0) {
//						if (GizmoEvents.offsetGizmoActive) {
//							toolbarInt = 0;
//						}
//						if (HighLogic.FindObjectsOfType<EditorGizmos.GizmoRotate> ().Length > 0) { 
//						if (GizmoEvents.rotateGizmoActive) {
//							toolbarInt = 1;
//						}
					}
				}
				if (activePuc != null) {

					if (Input.GetKey (EditorExtensions.Instance.cfg.KeyMap.Down) || Input.GetKey (EditorExtensions.Instance.cfg.KeyMap.Up)
						|| Input.GetKey (EditorExtensions.Instance.cfg.KeyMap.Left) || Input.GetKey (EditorExtensions.Instance.cfg.KeyMap.Right)
						||  Input.GetKey (EditorExtensions.Instance.cfg.KeyMap.Forward) || Input.GetKey (EditorExtensions.Instance.cfg.KeyMap.Back) ) {
						
						if (!fineAdjustActive) {
							fineAdjustActive = true;
							InputLockManager.SetControlLock (ControlTypes.CAMERACONTROLS, "EEX_FA");
						}

					}
					if ( Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse0)) {
						fineAdjustActive = false;
						InputLockManager.RemoveControlLock ("EEX_FA");
					}

				} else {
					InputLockManager.RemoveControlLock ("EEX_FA");
				}
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
			Log.Debug ("FineAdjustWindow Show()");
			this.enabled = true;
		}

	}

}

#endif
