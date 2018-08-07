using System;
using UnityEngine;
using ClickThroughFix;

namespace EditorExtensionsRedux
{
	public class SettingsWindow : MonoBehaviour
	{
		public delegate void WindowDisabledEventHandler ();

		public event WindowDisabledEventHandler WindowDisabled;

		protected virtual void OnWindowDisabled ()
		{
			if (WindowDisabled != null)
				WindowDisabled ();
		}

		ConfigData _config;
		string _configFilePath;
		KeyCode _lastKeyPressed = KeyCode.None;
		string _windowTitle = string.Empty;
		string _version = string.Empty;

		Rect _windowRect = new Rect () {
			xMin = Screen.width - 325,
			xMax = Screen.width - 50,
			yMin = 50,
			yMax = 50 //0 height, GUILayout resizes it
		};

		//ctor
		public SettingsWindow ()
		{
			//start disabled
			this.enabled = false;
		}

		void Awake ()
		{
			Log.Debug ("SettingsWindow Awake()");
		}

		void Update ()
		{
			if (Event.current.isKey) { 
				_lastKeyPressed = Event.current.keyCode;
			}
		}

		void OnEnable ()
		{
			Log.Debug ("SettingsWindow OnEnable()");

			if (_config == null || string.IsNullOrEmpty (_configFilePath)) {
				this.enabled = false;
			}
		}

		void CloseWindow ()
		{
			this.enabled = false;
			OnWindowDisabled ();
		}

		//void OnDisable ()
		//{
		//}

		void OnGUI ()
		{
			if (Event.current.type == EventType.Layout) {
				//_windowRect.yMax = _windowRect.yMin;
				_windowRect = ClickThruBlocker.GUILayoutWindow (this.GetInstanceID (), _windowRect, WindowContent, _windowTitle);
			}
		}

		//void OnDestroy ()
		//{
		//}

		/// <summary>
		/// Initializes the window content and enables it
		/// </summary>
		public void Show (ConfigData config, string configFilePath, Version version)
		{
			Log.Debug ("SettingsWindow Show()");
			_config = config;
			_configFilePath = configFilePath;
			_windowTitle = string.Format ("Editor Extensions v{0}.{1}", version.Major.ToString (), version.Minor.ToString ());
			
			_version = version.ToString ();
			this.enabled = true;
		}

		private int toolbarInt = 0;
		private string[] _toolbarStrings = { "Settings 1", "Settings 2", "Angle Snap" };
		string keyMapToUpdate = string.Empty;
		string newAngleString = string.Empty;
		public int angleGridIndex = -1;
		public string[] angleStrings = new string[] { string.Empty };
		object anglesLock = new object ();
		GUILayoutOption[] settingsLabelLayout = new GUILayoutOption[] { GUILayout.MinWidth (150) };

		void WindowContent (int windowID)
		{
			int newToolbarInt = GUILayout.Toolbar (toolbarInt, _toolbarStrings);
            if (newToolbarInt != toolbarInt)
            {
                _windowRect.yMax = _windowRect.yMin;
                toolbarInt = newToolbarInt;
            }

			GUILayout.BeginVertical ("box");

			#region Settings
			if (toolbarInt == 0)
            {

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Version: " + _version.ToString ());
				GUILayout.EndHorizontal ();

#if DEBUG
				GUILayout.Label ("Debug Build");
				GUILayout.Label ("_lastKeyPressed: " + _lastKeyPressed.ToString ());
#endif

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Message delay:", settingsLabelLayout);
				if (GUILayout.Button ("-")) {
					_config.OnScreenMessageTime -= 0.5f;
				}
				GUILayout.Label (_config.OnScreenMessageTime.ToString (), "TextField");
				if (GUILayout.Button ("+")) {
					_config.OnScreenMessageTime += 0.5f;
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Max symmetry:", settingsLabelLayout);
				if (GUILayout.Button ("-")) {
					_config.MaxSymmetry--;
				}
				GUILayout.Label (_config.MaxSymmetry.ToString (), "TextField");
				if (GUILayout.Button ("+")) {
					_config.MaxSymmetry++;
				}
				GUILayout.EndHorizontal ();

// Following contributed by Fwiffo

				GUILayout.BeginHorizontal (GUILayout.ExpandWidth (true));
				_config.RapidZoom = GUILayout.Toggle (_config.RapidZoom, new GUIContent ("Tap then hold for rapid zoom" /* , "Tap the zoom hotkey then quickly hold it to zoom faster"*/));
				GUILayout.EndHorizontal ();

                //GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                ////GUILayout.Label("Double tap zoom to rapidly cycle:", settingsLabelLayout);
                //_config.ZoomCycling = GUILayout.Toggle(_config.ZoomCycling, new GUIContent("Double tap for rapid zoom", "Double tapping zoom keys cycles through preset distances"));
                //GUILayout.EndHorizontal();

// End of Fwiffo
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                bool b = _config.ReRootEnabled;
                _config.ReRootEnabled = GUILayout.Toggle(_config.ReRootEnabled, new GUIContent("ReRoot enabled"));
                if (!b && _config.ReRootEnabled)
                {
                    EditorExtensions.Instance.EnableSelectRoot();
                    EditorExtensions.Instance.ReRootActive = true;
                }
                if (b && !_config.ReRootEnabled)
                {
                    EditorExtensions.Instance.DisableSelectRoot();
                    EditorExtensions.Instance.ReRootActive = false;
                }

                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                b = _config.NoOffsetLimitEnabled;
                _config.NoOffsetLimitEnabled = GUILayout.Toggle(_config.NoOffsetLimitEnabled, new GUIContent("No Offset Limit enabled"));
                if (!b && _config.NoOffsetLimitEnabled)
                    EditorExtensions.Instance.fob = gameObject.AddComponent<NoOffsetBehaviour.FreeOffsetBehaviour>();

                if (b && !_config.NoOffsetLimitEnabled)
                {
                    Destroy(EditorExtensions.Instance.fob);
                    NoOffsetBehaviour.FreeOffsetBehaviour.Instance = null;
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                b = _config.FineAdjustEnabled;
                _config.FineAdjustEnabled = GUILayout.Toggle(_config.FineAdjustEnabled, new GUIContent("Fine Adjust enabled"));
                GUILayout.EndHorizontal();
                

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                _config.AnglesnapModIsToggle = GUILayout.Toggle(_config.AnglesnapModIsToggle, new GUIContent("Anglesnap + Mod Toggles")); 
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                _config.CycleSymmetryModeModIsToggle = GUILayout.Toggle(_config.CycleSymmetryModeModIsToggle, new GUIContent("Cycle Symmetry Mode + Mod Toggles"));
                GUILayout.EndHorizontal();

            }
            #endregion
            #region Fine Adjust settings
            if (toolbarInt == 1)
            {

                if (keyMapToUpdate == string.Empty) {
					GUILayout.Label ("Click button and press key to change");
				} else {
					GUILayout.Label ("Waiting for key");
				}

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Reset Symmetry Mode & Angle Snap keys"))
                {
                    // Editor_toggleSymMode = X
                    // Editor_toggleAngleSnap = C
                   
                    // First reset the HotkeyEditor settings
                    EditorExtensions.Instance.HotkeyEditor_toggleSymModePrimary = new KeyCodeExtended(KeyCode.X);
                    EditorExtensions.Instance.HotkeyEditor_toggleSymModeSecondary = new KeyCodeExtended(KeyCode.None);
                    EditorExtensions.Instance.HotkeyEditor_toggleAngleSnapPrimary = new KeyCodeExtended(KeyCode.C);
                    EditorExtensions.Instance.HotkeyEditor_toggleAngleSnapSecondary = new KeyCodeExtended(KeyCode.None);
                    // Now reset the GameSettings
                    GameSettings.Editor_toggleSymMode.primary = EditorExtensions.Instance.HotkeyEditor_toggleSymModePrimary;
                    GameSettings.Editor_toggleSymMode.secondary = EditorExtensions.Instance.HotkeyEditor_toggleSymModeSecondary;
                    GameSettings.Editor_toggleAngleSnap.primary = EditorExtensions.Instance.HotkeyEditor_toggleAngleSnapPrimary;
                    GameSettings.Editor_toggleAngleSnap.secondary = EditorExtensions.Instance.HotkeyEditor_toggleAngleSnapSecondary;
                    // and Save the game settings
                    GameSettings.SaveSettings();

                    //Finally,  set the Gamesetting key to null (see other locations for info)
                    GameSettings.Editor_toggleSymMode.primary = new KeyCodeExtended(KeyCode.None);
                    GameSettings.Editor_toggleSymMode.secondary = new KeyCodeExtended(KeyCode.None);
                    GameSettings.Editor_toggleAngleSnap.primary = new KeyCodeExtended(KeyCode.None);
                    GameSettings.Editor_toggleAngleSnap.secondary = new KeyCodeExtended(KeyCode.None);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal ();
				GUILayout.Label ("Surface attachment:", settingsLabelLayout);
				if (keyMapToUpdate == "am" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.AttachmentMode = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.AttachmentMode.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "am";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Vertical snap:", settingsLabelLayout);
				if (keyMapToUpdate == "vs" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.VerticalSnap = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.VerticalSnap.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "vs";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Horizontal snap:", settingsLabelLayout);
				if (keyMapToUpdate == "hs" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.HorizontalSnap = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.HorizontalSnap.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "hs";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Strut/fuel align:", settingsLabelLayout);
				if (keyMapToUpdate == "cpa" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.CompoundPartAlign = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.CompoundPartAlign.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "cpa";
				}
				GUILayout.EndHorizontal ();


                GUILayout.BeginHorizontal();
                GUILayout.Label("Toggle ReRoot:", settingsLabelLayout);
                if (keyMapToUpdate == "reroot" && _lastKeyPressed != KeyCode.None)
                {
                    _config.KeyMap.ToggleReRoot = _lastKeyPressed;
                    keyMapToUpdate = string.Empty;
                }
                if (GUILayout.Button(_config.KeyMap.ToggleReRoot.ToString()))
                {
                    _lastKeyPressed = KeyCode.None;
                    keyMapToUpdate = "reroot";
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Toggle No Offset Limit:", settingsLabelLayout);
                if (keyMapToUpdate == "nooffsetlimit" && _lastKeyPressed != KeyCode.None)
                {
                    _config.KeyMap.ToggleNoOffsetLimit = _lastKeyPressed;
                    keyMapToUpdate = string.Empty;
                }
                if (GUILayout.Button(_config.KeyMap.ToggleNoOffsetLimit.ToString()))
                {
                    _lastKeyPressed = KeyCode.None;
                    keyMapToUpdate = "nooffsetlimit";
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal ();
				GUILayout.Label ("Reset camera:", settingsLabelLayout);
				if (keyMapToUpdate == "rc" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.ResetCamera = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.ResetCamera.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "rc";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Zoom Selected:", settingsLabelLayout);
				if (keyMapToUpdate == "zs" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.ZoomSelected = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.ZoomSelected.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "zs";
				}
				GUILayout.EndHorizontal ();
#if true

                GUILayout.Label ("Fine Adjust Keys");


				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Up:", settingsLabelLayout);
				if (keyMapToUpdate == "up" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Up = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Up.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "up";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Down:", settingsLabelLayout);
				if (keyMapToUpdate == "down" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Down = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Down.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "down";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Left:", settingsLabelLayout);
				if (keyMapToUpdate == "left" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Left = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Left.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "left";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Right:", settingsLabelLayout);
				if (keyMapToUpdate == "right" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Right = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Right.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "right";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Forward:", settingsLabelLayout);
				if (keyMapToUpdate == "fwd" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Forward = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Forward.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "fwd";
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Back:", settingsLabelLayout);
				if (keyMapToUpdate == "back" && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Back = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Back.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = "back";
				}
				GUILayout.EndHorizontal ();
#endif
			}

#endregion

#region angle snap values settings
			if (toolbarInt == 2) {

				try {
					lock (anglesLock) {
						foreach (float a in _config.AngleSnapValues) {
							if (a != 0.0f) {
								GUILayout.BeginHorizontal ();
								GUILayout.Label (a.ToString (), settingsLabelLayout);
								if (GUILayout.Button ("Remove")) {
									_config.AngleSnapValues.Remove (a);
								}
								GUILayout.EndHorizontal ();
							}
						}
					}

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Add angle: ");
					newAngleString = GUILayout.TextField (newAngleString);
					if (GUILayout.Button ("Add")) {
						float newAngle = 0.0f;

						if (!string.IsNullOrEmpty (newAngleString) && float.TryParse (newAngleString, out newAngle)) {
							lock (anglesLock) {
								if (newAngle > 0.0f && newAngle <= 90.0f && _config.AngleSnapValues.IndexOf (newAngle) == -1) {
									_config.AngleSnapValues.Add (newAngle);
									_config.AngleSnapValues.Sort ();
								}
							}
						}

					}
					GUILayout.EndHorizontal ();

				}
#if DEBUG
				catch (Exception ex) {
					//potential for some intermittent locking/threading issues here	
					//Debug only to avoid log spam
					Log.Error ("Error updating AngleSnapValues: " + ex.Message);
				}
#else
				catch(Exception){
					//just ignore the error and continue since it's non-critical
				}
#endif
			}

#endregion

			GUILayout.EndVertical ();//end main content

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Close")) {
				//reload config to reset any unsaved changes?
				//_config = ConfigManager.LoadConfig (_configFilePath);
				CloseWindow ();
			}

			if (GUILayout.Button ("Defaults")) {
				_config = ConfigManager.CreateDefaultConfig (_configFilePath, _version);				
			}

			if (GUILayout.Button ("Save")) {
				ConfigManager.SaveConfig (_config, _configFilePath);
				CloseWindow ();
			}
			GUILayout.EndHorizontal ();

			GUI.DragWindow ();
		}

	}
}

