using KSP.Localization;
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
		}

		void Awake ()
		{
			//start disabled
			this.enabled = false;
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
		private string[] _toolbarStrings = { Localizer.Format("#LOC_EEX_93"), Localizer.Format("#LOC_EEX_94"), Localizer.Format("#LOC_EEX_95") };
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

			GUILayout.BeginVertical (Localizer.Format("#LOC_EEX_66"));

			#region Settings
			if (toolbarInt == 0)
            {

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_96") + _version.ToString ());
				GUILayout.EndHorizontal ();

#if DEBUG
				GUILayout.Label (Localizer.Format("#LOC_EEX_97"));
				GUILayout.Label (Localizer.Format("#LOC_EEX_98") + _lastKeyPressed.ToString ());
#endif

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_99"), settingsLabelLayout);
				if (GUILayout.Button ("-")) {
					_config.OnScreenMessageTime -= 0.5f;
				}
				GUILayout.Label (_config.OnScreenMessageTime.ToString (), Localizer.Format("#LOC_EEX_62"));
				if (GUILayout.Button ("+")) {
					_config.OnScreenMessageTime += 0.5f;
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_100"), settingsLabelLayout);
				if (GUILayout.Button ("-")) {
					_config.MaxSymmetry--;
				}
				GUILayout.Label (_config.MaxSymmetry.ToString (), Localizer.Format("#LOC_EEX_62"));
				if (GUILayout.Button ("+")) {
					_config.MaxSymmetry++;
				}
				GUILayout.EndHorizontal ();

// Following contributed by Fwiffo

				GUILayout.BeginHorizontal (GUILayout.ExpandWidth (true));
				_config.RapidZoom = GUILayout.Toggle (_config.RapidZoom, new GUIContent (Localizer.Format("#LOC_EEX_101") /* , "Tap the zoom hotkey then quickly hold it to zoom faster"*/));
				GUILayout.EndHorizontal ();

                //GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                ////GUILayout.Label("Double tap zoom to rapidly cycle:", settingsLabelLayout);
                //_config.ZoomCycling = GUILayout.Toggle(_config.ZoomCycling, new GUIContent("Double tap for rapid zoom", "Double tapping zoom keys cycles through preset distances"));
                //GUILayout.EndHorizontal();

// End of Fwiffo
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                bool b = _config.ReRootEnabled;
                _config.ReRootEnabled = GUILayout.Toggle(_config.ReRootEnabled, new GUIContent(Localizer.Format("#LOC_EEX_102")));
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
                _config.FineAdjustEnabled = GUILayout.Toggle(_config.FineAdjustEnabled, new GUIContent(Localizer.Format("#LOC_EEX_103")));
                GUILayout.EndHorizontal();
                

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                _config.AnglesnapModIsToggle = GUILayout.Toggle(_config.AnglesnapModIsToggle, new GUIContent(Localizer.Format("#LOC_EEX_104"))); 
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                _config.CycleSymmetryModeModIsToggle = GUILayout.Toggle(_config.CycleSymmetryModeModIsToggle, new GUIContent(Localizer.Format("#LOC_EEX_105")));
                GUILayout.EndHorizontal();

            }
            #endregion
            #region Fine Adjust settings
            if (toolbarInt == 1)
            {

                if (keyMapToUpdate == string.Empty) {
					GUILayout.Label (Localizer.Format("#LOC_EEX_106"));
				} else {
					GUILayout.Label (Localizer.Format("#LOC_EEX_107"));
				}

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Localizer.Format("#LOC_EEX_108")))
                {
                    // Editor_toggleSymMode = X
                    // Editor_toggleAngleSnap = C
                   
                    // First reset the HotkeyEditor settings
                    EditorExtensions.Instance.HotkeyEditor_toggleSymModePrimary = new KeyCodeExtended(KeyCode.X);
                    EditorExtensions.Instance.HotkeyEditor_toggleSymModeSecondary = new KeyCodeExtended(KeyCode.None);
                    EditorExtensions.Instance.HotkeyEditor_toggleAngleSnapPrimary = new KeyCodeExtended(KeyCode.C);
                    EditorExtensions.Instance.HotkeyEditor_toggleAngleSnapSecondary = new KeyCodeExtended(KeyCode.None);
                    // Now reset the GameSettings
                    EditorExtensions.Instance.SetKeysToSavedSettings();
                    // and Save the game settings
                    EditorExtensions.Instance.SafeWriteSettings();

                    //Finally,  set the Gamesetting key to null (see other locations for info)
                    EditorExtensions.Instance.SetKeysToNoneValue();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_109"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_110") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.AttachmentMode = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.AttachmentMode.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_110");
				}
				GUILayout.EndHorizontal ();

                
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_EEX_111"), settingsLabelLayout);
                if (keyMapToUpdate == Localizer.Format("#LOC_EEX_112") && _lastKeyPressed != KeyCode.None)
                {
                    _config.KeyMap.HorizontalCenter = _lastKeyPressed;
                    keyMapToUpdate = string.Empty;
                }
                if (GUILayout.Button(_config.KeyMap.HorizontalCenter.ToString()))
                {
                    _lastKeyPressed = KeyCode.None;
                    keyMapToUpdate = Localizer.Format("#LOC_EEX_112");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_113"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_114") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.VerticalSnap = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.VerticalSnap.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_114");
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_115"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_116") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.HorizontalSnap = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.HorizontalSnap.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_116");
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_117"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_118") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.CompoundPartAlign = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.CompoundPartAlign.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_118");
				}
				GUILayout.EndHorizontal ();


                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_EEX_119"), settingsLabelLayout);
                if (keyMapToUpdate == Localizer.Format("#LOC_EEX_120") && _lastKeyPressed != KeyCode.None)
                {
                    _config.KeyMap.ToggleReRoot = _lastKeyPressed;
                    keyMapToUpdate = string.Empty;
                }
                if (GUILayout.Button(_config.KeyMap.ToggleReRoot.ToString()))
                {
                    _lastKeyPressed = KeyCode.None;
                    keyMapToUpdate = Localizer.Format("#LOC_EEX_120");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_EEX_121"), settingsLabelLayout);
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
				GUILayout.Label (Localizer.Format("#LOC_EEX_122"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_123") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.ResetCamera = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.ResetCamera.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_123");
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_124"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_125") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.ZoomSelected = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.ZoomSelected.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_125");
				}
				GUILayout.EndHorizontal ();
#if true

                GUILayout.Label (Localizer.Format("#LOC_EEX_126"));


				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_127"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_128") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Up = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Up.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_128");
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_129"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_130") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Down = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Down.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_130");
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_131"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_132") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Left = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Left.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_132");
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_133"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_134") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Right = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Right.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_134");
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_135"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_136") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Forward = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Forward.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_136");
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_137"), settingsLabelLayout);
				if (keyMapToUpdate == Localizer.Format("#LOC_EEX_138") && _lastKeyPressed != KeyCode.None) {
					_config.KeyMap.Back = _lastKeyPressed;
					keyMapToUpdate = string.Empty;
				}
				if (GUILayout.Button (_config.KeyMap.Back.ToString ())) {
					_lastKeyPressed = KeyCode.None;
					keyMapToUpdate = Localizer.Format("#LOC_EEX_138");
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
								if (GUILayout.Button (Localizer.Format("#LOC_EEX_139"))) {
									_config.AngleSnapValues.Remove (a);
								}
								GUILayout.EndHorizontal ();
							}
						}
					}

					GUILayout.BeginHorizontal ();
					GUILayout.Label (Localizer.Format("#LOC_EEX_140"));
					newAngleString = GUILayout.TextField (newAngleString);
					if (GUILayout.Button (Localizer.Format("#LOC_EEX_141"))) {
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
			if (GUILayout.Button (Localizer.Format("#LOC_EEX_51"))) {
				//reload config to reset any unsaved changes?
				//_config = ConfigManager.LoadConfig (_configFilePath);
				CloseWindow ();
			}

			if (GUILayout.Button (Localizer.Format("#LOC_EEX_142"))) {
				_config = ConfigManager.CreateDefaultConfig (_configFilePath, Localizer.Format("#LOC_EEX_6"), _version);				
			}
            if (GUILayout.Button(Localizer.Format("#LOC_EEX_143")))
            {
                _config = ConfigManager.CreateDefaultConfig(_configFilePath, Localizer.Format("#LOC_EEX_144"), _version);
            }
            if (GUILayout.Button (Localizer.Format("#LOC_EEX_145"))) {
				ConfigManager.SaveConfig (_config, _configFilePath);
				CloseWindow ();
			}
			GUILayout.EndHorizontal ();

			GUI.DragWindow ();
		}

	}
}

