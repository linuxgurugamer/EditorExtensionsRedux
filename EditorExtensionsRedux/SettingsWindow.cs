using System;
using UnityEngine;

namespace EditorExtensionsRedux
{
	public class SettingsWindow : MonoBehaviour
	{
		public delegate void WindowDisabledEventHandler();
		public event WindowDisabledEventHandler WindowDisabled;
		protected virtual void OnWindowDisabled() 
		{
			if (WindowDisabled != null)
				WindowDisabled();
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

			if(_config == null || string.IsNullOrEmpty(_configFilePath)){
				this.enabled = false;
			}
		}

		void CloseWindow(){
			this.enabled = false;
			OnWindowDisabled ();
		}

		void OnDisable(){
		}

		void OnGUI ()
		{
			if (Event.current.type == EventType.Layout) {
				_windowRect.yMax = _windowRect.yMin;
				_windowRect = GUILayout.Window (this.GetInstanceID (), _windowRect, WindowContent, _windowTitle);
			}
		}

		void OnDestroy ()
		{
		}

		/// <summary>
		/// Initializes the window content and enables it
		/// </summary>
		public void Show (ConfigData config, string configFilePath, Version version)
		{
			Log.Debug ("SettingsWindow Show()");
			_config = config;
			_configFilePath = configFilePath;
			_windowTitle = string.Format ("Editor Extensions v{0}.{1}", version.Major.ToString (), version.Minor.ToString ());;
			_version = version.ToString();
			this.enabled = true;
		}

		private int toolbarInt = 0;
		private string[] _toolbarStrings = { "Settings", "Angle Snap"};
		string keyMapToUpdate = string.Empty;
		string newAngleString = string.Empty;
		public int angleGridIndex = -1;
		public string[] angleStrings = new string[] { string.Empty };
		object anglesLock = new object ();
		GUILayoutOption[] settingsLabelLayout = new GUILayoutOption[] { GUILayout.MinWidth (150) };

		void WindowContent (int windowID)
		{
			toolbarInt = GUILayout.Toolbar (toolbarInt, _toolbarStrings);

			GUILayout.BeginVertical ("box");

			#region Settings
			if (toolbarInt == 0) {

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Version: " + _version.ToString());
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

				if (keyMapToUpdate == string.Empty) {
					GUILayout.Label ("Click button and press key to change");
				} else {
					GUILayout.Label ("Waiting for key");
				}

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
			}
			#endregion

			#region angle snap values settings
			if (toolbarInt == 1) {

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
				_config = ConfigManager.CreateDefaultConfig (_configFilePath, _version);;
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

