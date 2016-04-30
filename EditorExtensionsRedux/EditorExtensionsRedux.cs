using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using System.Reflection;
using KSP.IO;
using UnityEngine;
//using UnityEngine.UI;

namespace EditorExtensionsRedux
{
	public class Constants
	{
		// Following for SelectRoot
		public int SELECTEDPART = 13;
		public int ST_ROOT_SELECT = 77;
		public int ST_ROOT_UNSELECTED = 76;
		public int MODEMSG = 60;
		public int ST_IDLE = 70;
		public int ST_PLACE = 71;
		public int ONMOUSEISOVER = 250;
		public int GET_STATEEVENTS = 0;

		// Following for NoOffsetLimits
		public int ST_OFFSET_TWEAK = 73;
		public int SYMUPDATEATTACHNODE = 108;
		public int GIZMOOFFSET = 66;

		public int UPDATESYMMETRY = 64;
		public int ONOFFSETGIZMOUPDATED = 35;

		public bool Init()
		{
			if (Versioning.version_major == 1 && Versioning.version_minor == 1 && Versioning.Revision == 0 /*&& Versioning.BuildID == 1024 */) {
				// SelectRoot
				SELECTEDPART = 13;
				 ST_ROOT_SELECT = 77;
				 ST_ROOT_UNSELECTED = 76;
				 MODEMSG = 60;
				 ST_IDLE = 70;
				 ST_PLACE = 71;
				 ONMOUSEISOVER = 250;
				 GET_STATEEVENTS = 0;

				// NoOffsetLimits
				 ST_OFFSET_TWEAK = 73;
				 SYMUPDATEATTACHNODE = 108;
				 GIZMOOFFSET = 66;

				 UPDATESYMMETRY = 64;
				 ONOFFSETGIZMOUPDATED = 35;

				return true;
			}
			if (Versioning.version_major == 1 && Versioning.version_minor == 1 && Versioning.Revision == 1 /*&& Versioning.BuildID == 1024 */) {
				// SelectRoot
				 SELECTEDPART = 13;
				 ST_ROOT_SELECT = 80;
				 ST_ROOT_UNSELECTED = 79;
				 MODEMSG = 63;
				 ST_IDLE = 73;
				 ST_PLACE = 74;
				 ONMOUSEISOVER = 250;
				 GET_STATEEVENTS = 0;

				// NoOffsetLimits
				 ST_OFFSET_TWEAK = 76;
				 SYMUPDATEATTACHNODE = 111;
				 GIZMOOFFSET = 69;

				 UPDATESYMMETRY = 62;
				 ONOFFSETGIZMOUPDATED = 35;
				return true;
			}
			return false;
		}
	}

	[KSPAddon (KSPAddon.Startup.EditorAny, false)]
	public class EditorExtensions : MonoBehaviour
	{
		public static EditorExtensions Instance { get; private set; }
		public static bool validVersion = false;
		static bool warningShown;
		const string warning = "This version of Editor Extensions Redux is not compatible with this version of KSP";

		public static Constants c = new Constants ();

		public bool Visible { get; set; }

		#region member vars



		const string ConfigFileName = "config.xml";
		const string DegreesSymbol = "\u00B0";

		EditorLogic editor;
		Version pluginVersion;
		ConfigData cfg;
		string _pluginDirectory;
		string _configFilePath;
		int _symmetryMode = 0;

		SettingsWindow _settingsWindow = null;
		PartInfoWindow _partInfoWindow = null;
		//StrutWindow _strutWindow = null;

		bool enableHotkeys = true;
		//bool _gizmoActive = false;

		Vector3 cameraLookAt = new Vector3(0,15,0);
		bool zoomSelected = false;
		#endregion

	//	public EditorExtensions (){}

		//Unity initialization call, called first
		public void Awake ()
		{
			Log.Debug ("Awake()");
			Log.Debug ("launchSiteName: " + EditorLogic.fetch.launchSiteName);
		}

		#if DEBUG
		// http://stackoverflow.com/a/1615860
		private static string EncodeNonAsciiCharacters(string value) {
			StringBuilder sb = new StringBuilder();
			foreach(char c in value) {
				// This character is too big for ASCII
				string encodedValue = "\\u" + ((int)c).ToString("x4");
				sb.Append(encodedValue);
			}
			return sb.ToString();
		}

		void localdumpReflection()
		{
			//Log.Debug("States:");
			//foreach (var f in EditorLogic.fetch.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
			//	if (f.FieldType == typeof(KFSMState)) {
			//		Log.Debug ("State: " + ((KFSMState)f.GetValue (EditorLogic.fetch)).name + " + " + EncodeNonAsciiCharacters (f.Name));
			//	}
			//}
			//Log.Debug("Events:");
			//foreach (var f in EditorLogic.fetch.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
			//	if (f.FieldType == typeof(KFSMEvent)) {
			//		Log.Debug ("State: " + ((KFSMEvent)f.GetValue (EditorLogic.fetch)).name + " + " +  EncodeNonAsciiCharacters (f.Name));
			//	}
			//}

			//Log.Debug("State/Event enumeration done.");
			EditorLogic el = EditorLogic.fetch;
			int c = 0;
			foreach (FieldInfo FI in el.GetType().GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
				Log.Info ("EditorLogic Field name[" + c.ToString() + "]: " + FI.Name + "    Fieldtype: " + FI.FieldType.ToString());
				c++;
			}

			KFSMEvent ke = new KFSMEvent("a");
			c = 0;
			foreach (FieldInfo FI in ke.GetType().GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
				Log.Info ("KFSMEvent KFSMEvent Field name[" + c.ToString() + "]: " + FI.Name + "    Fieldtype: " + FI.FieldType.ToString());
				c++;
			}

			MethodInfo[] leMethods = typeof(EditorLogic).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			c = 0;
			foreach (MethodInfo FI in leMethods) {
				Log.Info ("MethodInfo  EditorLogic methods name[" + c.ToString() + "]: " + FI.Name );
				c++;
			}

			MethodInfo[] parts = typeof(Part).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			c = 0;
			foreach (MethodInfo FI in parts) {
				Log.Info ("MethodInfo  Part  name[" + c.ToString() + "]: " + FI.Name );
				c++;
			}

			MethodInfo[] kfe = typeof(KFSMEvent).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			c = 0;
			foreach (MethodInfo FI in kfe) {
				Log.Info ("MethodInfo KFSMEvent  methods name[" + c.ToString() + "]: " + FI.Name );
				c++;
			}


			MethodInfo[] ks = typeof(KFSMState).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			c = 0;
			foreach (MethodInfo FI in ks) {
				Log.Info ("MethodInfo KFSMState  methods name[" + c.ToString() + "]: " + FI.Name + "   " + FI.ToString() );
				c++;
			}


		}
		#endif

		//Unity, called after Awake()
		public void Start()
		{
			Log.Debug ("Start()");
			#if DEBUG
			localdumpReflection ();
			#endif
			editor = EditorLogic.fetch;
			Instance = this;
			InitConfig ();
			if (!validVersion)
				return;
			InitializeGUI ();
			GameEvents.onEditorPartEvent.Add (EditorPartEvent);
			GameEvents.onEditorSymmetryModeChange.Add (EditorSymmetryModeChange);

		}

		//Unity OnDestroy
		void OnDestroy ()
		{
			Log.Debug ("OnDestroy()");
			//if (_settingsWindow != null)
			//	_settingsWindow.enabled = false;
			//if (_partInfoWindow != null)
			//	_partInfoWindow.enabled = false;

			GameEvents.onEditorPartEvent.Remove (EditorPartEvent);
			GameEvents.onEditorSymmetryModeChange.Remove (EditorSymmetryModeChange);
		}

		void EditorPartEvent(ConstructionEventType eventType, Part part)
		{
			if (eventType == ConstructionEventType.PartRotating || eventType == ConstructionEventType.PartOffsetting)
				return;

			if (eventType == ConstructionEventType.PartDragging) {
				return;
			}			

			Log.Debug (string.Format ("EditorPartEvent {0} part {1}", eventType, part));

			if (eventType == ConstructionEventType.PartAttached) {
				if (part.parent != null) {
					Log.Debug ("Part parent: " + part.parent.name);
					Log.Debug ("Node attached: " + IsPartNodeAttached (part).ToString ());
				} else {
					Log.Debug ("Part parent is null");
				}
			}
		}

		void EditorSymmetryModeChange(int symMode)
		{
			Log.Debug ("EditorSymmetryModeChange: " + symMode.ToString ());
		}

		void InitConfig ()
		{
			validVersion = c.Init ();

			if (!validVersion) {
				return;
			}

				
			try {
				//get location and version info of the plugin
				Assembly execAssembly = Assembly.GetExecutingAssembly ();
				pluginVersion = execAssembly.GetName ().Version;
				_pluginDirectory = Path.GetDirectoryName (execAssembly.Location);

				//dll's path + filename for the config file
				_configFilePath = Path.Combine (_pluginDirectory, ConfigFileName);

				//check if the config file is there and create if its missing
				if (ConfigManager.FileExists (_configFilePath)) {

					cfg = ConfigManager.LoadConfig (_configFilePath);

					if (cfg == null) {
						//failed to load config, create new
						cfg = ConfigManager.CreateDefaultConfig (_configFilePath, pluginVersion.ToString ());
					} else {
						//check config file version
						Version fileVersion = new Version ();

						if (cfg.FileVersion != null) {
							Log.Debug ("Config v" + cfg.FileVersion + " Mod v" + pluginVersion.ToString ());

							try {
								fileVersion = new Version (cfg.FileVersion);
							} catch (Exception ex) {
								Log.Error ("Error parsing version from config file: " + ex.Message);
							}
						}

						#if DEBUG
						//for debug, replace if version isn't exactly the same
						bool versionMismatch = (cfg.FileVersion == null || fileVersion != pluginVersion);
						#else
						//replace if x.x doesn't match
						bool versionMismatch = (cfg.FileVersion == null || fileVersion.Major < pluginVersion.Major || (fileVersion.Major == pluginVersion.Major && fileVersion.Minor < pluginVersion.Minor));
						#endif

						if (versionMismatch) {
							Log.Info ("Config file version mismatch, replacing with new defaults");
							cfg = ConfigManager.CreateDefaultConfig (_configFilePath, pluginVersion.ToString ());
						} else {
							Log.Debug ("Config file is current");
						}
					}

				} else {
					cfg = ConfigManager.CreateDefaultConfig (_configFilePath, pluginVersion.ToString ());
					Log.Info ("No existing config found, created new default config");
				}

				Log.Debug ("Initializing version " + pluginVersion.ToString ());
			} catch (Exception ex) {
				Log.Debug ("FATAL ERROR - Unable to initialize: " + ex.Message);
				//_abort = true;
				return;
			}
		}

		//Unity update
		void Update ()
		{
			if (!validVersion)
				return;
			//if (editor.shipNameField.Focused || editor.shipDescriptionField.Focused)
			//	return;

			//ignore hotkeys while settings window is open
			//if (_settingsWindow != null && _settingsWindow.enabled)
			//	return;

			//hotkeyed editor functions
			if (enableHotkeys) {

				//check for the configured modifier key
				bool modKeyDown = GameSettings.MODIFIER_KEY.GetKey();
				//check for configured editor fine key
				bool fineKeyDown = GameSettings.Editor_fineTweak.GetKey();

				Camera cam = editor.editorCamera;
				//Zoom selected part - rotate camera around part
				if (Input.GetKeyDown (cfg.KeyMap.ZoomSelected)) {
					Part p = Utility.GetPartUnderCursor ();
					if (p != null) {
						zoomSelected = true;
						cameraLookAt = p.transform.position;
						cam.transform.position = new Vector3 (cam.transform.position.x, p.transform.position.y, cam.transform.position.z);
						OSDMessage (string.Format ("Zoom Camera on {0}", p.name));
					} else {
						cameraLookAt = new Vector3(0,15,0);
						OSDMessage ("Default Camera");
						ResetCamera ();
						zoomSelected = false;
					}
				}

				if (zoomSelected) {
					cam.transform.LookAt(cameraLookAt);
				}					

				// U - strut/fuel line alignment
				// U - snap heights on both parts
				// mod-U level/perpendicular to parent part
				if (Input.GetKeyDown (cfg.KeyMap.CompoundPartAlign)) {
					Part p = Utility.GetPartUnderCursor ();
					if (p != null && p.GetType () == typeof(CompoundPart)) {
						AlignCompoundPart ((CompoundPart)p, !modKeyDown);
					}
				}
			
				// V - Vertically align part under cursor with the part it is attached to
				if (Input.GetKeyDown (cfg.KeyMap.VerticalSnap)) {
					VerticalAlign ();
					return;
				}

				// H - Horizontally align part under cursor with the part it is attached to
				if (Input.GetKeyDown (cfg.KeyMap.HorizontalSnap)) {
					HorizontalAlign ();
					return;
				}     

				//Space - when no part is selected, reset camera
				if (Input.GetKeyDown (cfg.KeyMap.ResetCamera) && !EditorLogic.SelectedPart) {
					ResetCamera ();
					return;
				}
	
				// T: Surface attachment toggle
				if (Input.GetKeyDown (cfg.KeyMap.AttachmentMode)) {
					SurfaceAttachToggle ();
					return;
				}
	
				// ALT+Z : Toggle part clipping (From cheat options)
				if (modKeyDown && Input.GetKeyDown (cfg.KeyMap.PartClipping)) {
					PartClippingToggle ();
					return;
				}

				//KSP v1.0.3: Change angle snap and symmetry mode actions to GetKeyUp() so that it fires after internal editor actions

				//using gamesettings keybinding Input.GetKeyDown (cfg.KeyMap.AngleSnap)
				// C, Shift+C : Increment/Decrement Angle snap
				if (GameSettings.Editor_toggleAngleSnap.GetKeyUp()) {
					AngleSnapCycle (modKeyDown, fineKeyDown);
					return;	
				}
	
				//using gamesettings keybinding Input.GetKeyDown (cfg.KeyMap.Symmetry)
				// X, Shift+X : Increment/decrement symmetry mode
				if (GameSettings.Editor_toggleSymMode.GetKeyUp()) {
					SymmetryModeCycle (modKeyDown, fineKeyDown);
					return;
				}

			}//end if(enableHotKeys)
		}

		bool IsPartNodeAttached(Part p)
		{
			if (p.parent == null || p == null)
				return false;

			foreach (AttachNode n in p.attachNodes) {
				if (n.attachedPart == p.parent) {
					Log.Debug (string.Format ("Part {0} is attached via node on parent {1}", p.name, p.parent.name));
					return true;
				}
			}

			return false;
		}

		#region Alignments

		void AlignToTopOfParent(Part p)
		{
			if (p.parent.GetPartRendererBound ().extents.y >= p.GetPartRendererBound ().extents.y) {
				CenterVerticallyOnParent (p);
				return;
			}

			float newHeight = p.parent.GetPartRendererBound ().extents.y - p.GetPartRendererBound ().extents.y;
			if (p.transform.localPosition.y < 0)
				newHeight = -newHeight;

			VertialPositionOnParent (p, newHeight);

			throw new NotImplementedException ();
		}

		void CenterVerticallyOnParent(Part p)
		{
			VertialPositionOnParent (p, 0f);
		}

		void VertialPositionOnParent(Part p, float position)
		{
			if (p.parent != null) {
				Log.Debug (string.Format ("Positioning {0} vertically on parent {1}", p.name, p.parent.name));
				p.transform.localPosition = new Vector3 (p.transform.localPosition.x, position, p.transform.localPosition.z);
				p.attPos0.y = position;
			}
		}

		void CenterOnParent(Part p)
		{
			//check for orientation of parent, if it's on the end of the parent, center on the end
			//on the surface, center lengthwise
			AttachNode an = p.parent.findAttachNodeByPart (p);
			if (an.nodeType == AttachNode.NodeType.Surface) {

			} else if (an.nodeType == AttachNode.NodeType.Stack) {

			} else if (an.nodeType == AttachNode.NodeType.Dock) {

			}

			throw new NotImplementedException ();
		}

		void CenterHorizontallyOnParent(Part p) {
			p.transform.localPosition = new Vector3 (p.transform.localPosition.x, p.transform.localPosition.y, 0f);
			p.attPos0.z = 0f;
		}

		void VerticalAlign(){
			try {
				Part sp = Utility.GetPartUnderCursor ();

				if (sp != null && sp.srfAttachNode != null && sp.srfAttachNode.attachedPart != null && !GizmoActive() && !IsPartNodeAttached(sp)) {

					//move hovered part
					CenterVerticallyOnParent(sp);

					//move any symmetry siblings/counterparts
					foreach (Part symPart in sp.symmetryCounterparts) {
						CenterVerticallyOnParent(symPart);
					}

					AddUndo();
				}
			} catch (Exception ex) {
				Log.Error ("Error trying to vertically align: " + ex.Message);
			}

			return;
		}

		void HorizontalAlign(){
			try {
				Part sp = Utility.GetPartUnderCursor ();

				if (sp != null && sp.srfAttachNode != null && sp.srfAttachNode.attachedPart != null && !GizmoActive() && !IsPartNodeAttached(sp)) {

					//Part ap = sp.srfAttachNode.attachedPart;
					List<Part> symParts = sp.symmetryCounterparts;

					Log.Debug ("symmetryCounterparts to move: " + symParts.Count.ToString ());

					//move selected part
					CenterHorizontallyOnParent(sp);

					//move any symmetry siblings/counterparts
					foreach (Part symPart in symParts) {
						CenterHorizontallyOnParent(symPart);
					}

					//Add edit to undo history
					AddUndo();
				}
			} catch (Exception ex) {
				Log.Error ("Error trying to Horizontally align: " + ex.Message);
			}
			return;
		}

		void AlignCompoundPart(CompoundPart part, bool snapHeights)
		{
			if (part.target != null && part.parent != null) {
				CompoundPartUtil.AlignCompoundPart(part, snapHeights);

				List<Part> symParts = part.symmetryCounterparts;
				//move any symmetry siblings/counterparts
				foreach (CompoundPart symPart in symParts) {
					CompoundPartUtil.AlignCompoundPart(symPart, snapHeights);
				}
				AddUndo ();
			}
		}

		#endregion

		#region Editor Actions

		void AddUndo()
		{
			//need to verify this is the right way, it does seem to work
			editor.SetBackup();
		}

		void ResetCamera(){
			if (!GizmoActive()) {

				//editor.editorCamera

				VABCamera VABcam = Camera.main.GetComponent<VABCamera> ();
				VABcam.camPitch = 0;
				VABcam.camHdg = 0;
				//VABcam.transform.position.y = 9f;

				SPHCamera SPHcam = Camera.main.GetComponent<SPHCamera> ();
				SPHcam.camPitch = 0;
				SPHcam.camHdg = 0;
				//SPHcam.transform.position.y = 9f;
			}
			return;
		}

		void SurfaceAttachToggle(){
			if (EditorLogic.SelectedPart) {
				//Toggle surface attachment for selected part
				EditorLogic.SelectedPart.attachRules.srfAttach ^= true;

				Log.Debug ("Toggling srfAttach for " + EditorLogic.SelectedPart.name);
				OSDMessage (String.Format ("Surface attachment {0} for {1}"
					, EditorLogic.SelectedPart.attachRules.srfAttach ? "enabled" : "disabled"
					, EditorLogic.SelectedPart.name
				));
			}
			return;
		}

		void PartClippingToggle(){
			CheatOptions.AllowPartClipping ^= true;
			Log.Debug ("AllowPartClipping " + (CheatOptions.AllowPartClipping ? "enabled" : "disabled"));
			OSDMessage ("Part clipping " + (CheatOptions.AllowPartClipping ? "enabled" : "disabled"));
			return;
		}

		void SymmetryModeCycle(bool modKeyDown, bool fineKeyDown){

			//InputLockManager.SetControlLock (ControlTypes.EDITOR_SYM_SNAP_UI, "EEX-SymLock");

			Log.Debug ("Starting symmetryMode: " + editor.symmetryMode.ToString ());
			//only inc/dec symmetry in radial mode, mirror is just 1&2
			if (editor.symmetryMethod == SymmetryMethod.Radial) {
				if (modKeyDown || (_symmetryMode < 2 && fineKeyDown)) {
					//Alt+X or Symmetry is at 1(index 2) or lower
					_symmetryMode = 0;
				} else if (_symmetryMode > cfg.MaxSymmetry - 2 && !fineKeyDown) {
					//Stop adding at max symmetry
					_symmetryMode = cfg.MaxSymmetry - 1;
				} else {
					//inc/dec symmetry
					_symmetryMode = _symmetryMode + (fineKeyDown ? -1 : 1);
				}
				editor.symmetryMode = _symmetryMode;
				Log.Debug ("Setting symmetryMode to " + _symmetryMode.ToString ());
			} else {
				//editor.symmetryMethod == SymmetryMethod.Mirror
				//update var with stock action's result
				_symmetryMode = editor.symmetryMode;
			}

			//GameEvents.onEditorSymmetryModeChange.Fire(_symmetryMode);
			//InputLockManager.RemoveControlLock("EEX-SymLock");

			Log.Debug ("Returning symmetryMode: " + editor.symmetryMode.ToString ());
			return;
		}

		void AngleSnapCycle(bool modKeyDown, bool fineKeyDown){
			if (!modKeyDown) {
				Log.Debug ("Starting srfAttachAngleSnap = " + editor.srfAttachAngleSnap.ToString ());

				int currentAngleIndex = cfg.AngleSnapValues.IndexOf (editor.srfAttachAngleSnap);

				Log.Debug ("currentAngleIndex: " + currentAngleIndex.ToString ());

				//rotate through the angle snap values
				float newAngle;
				if (fineKeyDown) {
					//lower snap
					newAngle = cfg.AngleSnapValues [currentAngleIndex == 0 ? cfg.AngleSnapValues.Count - 1 : currentAngleIndex - 1];
				} else {
					//higher snap
					newAngle = cfg.AngleSnapValues [currentAngleIndex == cfg.AngleSnapValues.Count - 1 ? 0 : currentAngleIndex + 1];
				}

				Log.Debug ("Setting srfAttachAngleSnap to " + newAngle.ToString ());
				editor.srfAttachAngleSnap = newAngle;
			} else {
				Log.Debug ("Resetting srfAttachAngleSnap to 0");
				editor.srfAttachAngleSnap = 0;
			}

			//at angle snap 0, turn off angle snap and show stock circle sprite
			if (editor.srfAttachAngleSnap == 0) {
				GameSettings.VAB_USE_ANGLE_SNAP = false;
			} else {
				GameSettings.VAB_USE_ANGLE_SNAP = true;
			}

			Log.Debug ("Exiting srfAttachAngleSnap = " + editor.srfAttachAngleSnap.ToString ());
			return;
		}

		bool GizmoActive ()
		{
			try {
				if (HighLogic.FindObjectsOfType<EditorGizmos.GizmoOffset> ().Length > 0 || HighLogic.FindObjectsOfType<EditorGizmos.GizmoRotate> ().Length > 0) {
					return true;
				} else {
					return false;
				}
				#if DEBUG
			} catch (Exception ex) {
				Log.Error ("Error getting active Gizmos: " + ex.Message);
				#else
				} catch (Exception) {
				#endif
				return false;
			}
		}

		#endregion

		#region GUI

		//private Rect _settingsWindowRect;
		GUIStyle osdLabelStyle, symmetryLabelStyle;

		void InitializeGUI ()
		{
			if (!validVersion)
				return;
			_settingsWindow = this.gameObject.AddComponent<SettingsWindow> ();
			_settingsWindow.WindowDisabled += new SettingsWindow.WindowDisabledEventHandler (SettingsWindowClosed);

			_partInfoWindow = this.gameObject.AddComponent<PartInfoWindow> ();

			//_strutWindow = this.gameObject.AddComponent<StrutWindow> ();

			osdLabelStyle = new GUIStyle () {
				stretchWidth = true,
				stretchHeight = true,
				alignment = TextAnchor.MiddleCenter,
				fontSize = 22,
				fontStyle = FontStyle.Bold,
				name = "OSDLabel"
			};
			osdLabelStyle.normal.textColor = Color.yellow;

			symmetryLabelStyle = new GUIStyle () {
				stretchWidth = true,
				stretchHeight = true,
				alignment = TextAnchor.MiddleCenter,
				fontSize = FONTSIZE,
				fontStyle = FontStyle.Bold,
				name = "SymmetryLabel"
			};
			symmetryLabelStyle.normal.textColor = Color.yellow;

			//skin.customStyles = new GUIStyle[]{ osdLabel, symmetryLabel };
		}

		//show the addon's GUI
		public void Show ()
		{
			if (!validVersion)
				return;
			this.Visible = true;
			Log.Debug ("Show()");
			//if (!_settingsWindow.enabled) {
			//	_settingsWindow.Show (cfg, _configFilePath, pluginVersion);
			//}
		}

		//hide the addon's GUI
		public void Hide ()
		{
			if (!validVersion)
				return;
			this.Visible = false;
			//Log.Debug ("Hide()");
			//if (_settingsWindow.enabled) {
			//	_settingsWindow.enabled = false;
			//}
		}

		public void SettingsWindowClosed(){
			Log.Debug ("Settings window closed, reloading config");
			cfg = ConfigManager.LoadConfig (_configFilePath);
			Hide ();
		}

		bool _showMenu = false;
		Rect _menuRect = new Rect ();
		const float _menuWidth = 100.0f;
		const float _menuHeight = 70.0f;
		const int _toolbarHeight = 42; //37

		public void ShowMenu ()
		{
			if (!validVersion)
				return;
			Vector3 position = Input.mousePosition;
				
			_menuRect = new Rect () {
				xMin = position.x - _menuWidth / 2,
				xMax = position.x + _menuWidth / 2,
				yMin = Screen.height - _toolbarHeight - _menuHeight,
				yMax = Screen.height - _toolbarHeight
			};
			_showMenu = true;
		}

		public void HideMenu ()
		{
			_showMenu = false;
		}

		//Unity GUI loop
		void OnGUI ()
		{
			if (!validVersion) {
				if (warningShown)
					return;
				GUIStyle centeredWarningStyle = new GUIStyle (GUI.skin.GetStyle ("Label"));
				Vector2 sizeOfWarningLabel = centeredWarningStyle.CalcSize(new GUIContent(warning));

				Rect _menuRect = new Rect (Screen.width / 2f - (sizeOfWarningLabel.x / 2f), Screen.height / 2 - sizeOfWarningLabel.y, 
					sizeOfWarningLabel.x, sizeOfWarningLabel.y * 2);

				_menuRect = GUILayout.Window (this.GetInstanceID (), _menuRect, ShowWarning, "EEX Menu");
				return;
			}


			//show and update the angle snap and symmetry mode labels
			ShowSnapLabels ();

			if (Event.current.type == EventType.Layout) {
				if (_showMenu || _menuRect.Contains (Event.current.mousePosition))
					_menuRect = GUILayout.Window (this.GetInstanceID (), _menuRect, MenuContent, "EEX Menu");
				else
					_menuRect = new Rect ();
			}
		}

		void MenuContent (int WindowID)
		{
			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Settings")) {
				_settingsWindow.Show (cfg, _configFilePath, pluginVersion);
				this.Visible = true;
			}

			//_strutWindow.enabled = GUILayout.Toggle (_strutWindow.enabled, "Strut Tool", "Button");
			//if (GUILayout.Button ("Strut tool")) {
			//	_strutWindow.Show ();
			//	this.Visible = true;
			//}

			if (cfg.ShowDebugInfo) {
				if (GUILayout.Button ("Position Debug")) {
					_partInfoWindow.Show ();
				}
			}
			GUILayout.EndVertical ();
		}

		void ShowWarning (int WindowID)
		{
			GUILayout.BeginVertical ();
			{
				float offsetY = Mathf.FloorToInt(0.8f * Screen.height);
				GUIStyle centeredWarningStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
				{
					alignment = TextAnchor.UpperCenter,
					fontSize = 16,
					normal = { textColor = Color.yellow }
				};

				Vector2 sizeOfWarningLabel = centeredWarningStyle.CalcSize(new GUIContent(warning));

				GUILayout.Label( warning, centeredWarningStyle);

				offsetY += sizeOfWarningLabel.y;
				if (GUILayout.Button( "Click to open the Forum thread"))
					Application.OpenURL("http://forum.kerbalspaceprogram.com/index.php?/topic/127378-editor-extensions-redux-324-released-for-111-with-selectroot-merge-stripsymmetry-nooffsetlimits/");

				offsetY += 25;



			}
			if (GUILayout.Button ("Close")) {
				warningShown = true;
			}

			GUILayout.EndVertical ();
		}

		void OSDMessage (string message)
		{
			ScreenMessages.PostScreenMessage (message, cfg.OnScreenMessageTime, ScreenMessageStyle.LOWER_CENTER);
		}

		#region Snap labels

		const int FONTSIZE = 14;

		string symmetryLabelValue = string.Empty;
		//symmetry & angle sprite/label size and position

		//const int advancedModeOffset = 34;
		//const int angleSnapLabelSize = 43;
		 int advancedModeOffset = 34;
		 int angleSnapLabelSize = 33;

		//const int angleSnapLabelLeftOffset = 209;
		//const int angleSnapLabelBottomOffset = 61;
		 int angleSnapLabelLeftOffset = 231;
		 int angleSnapLabelBottomOffset = 52;

		//const int symmetryLabelSize = 56;
		 int symmetryLabelSize = 43;

		//const int symmetryLabelLeftOffset = 152;
		//const int symmetryLabelBottomOffset = 63;
		 int symmetryLabelLeftOffset = 175;
		 int symmetryLabelBottomOffset = 50;


		Rect angleSnapLabelRect; /* = new Rect () {
			xMin = angleSnapLabelLeftOffset,
			xMax = angleSnapLabelLeftOffset + angleSnapLabelSize,
			yMin = Screen.height - angleSnapLabelBottomOffset,
			yMax = Screen.height - angleSnapLabelBottomOffset + angleSnapLabelSize
		}; */
		Rect symmetryLabelRect; /* = new Rect () {
			xMin = symmetryLabelLeftOffset,
			xMax = symmetryLabelLeftOffset + symmetryLabelSize,
			yMin = Screen.height - symmetryLabelBottomOffset,
			yMax = Screen.height - symmetryLabelBottomOffset + symmetryLabelSize
		};*/

		void AdjustSnapLocations()
		{
			//symmetry & angle sprite/label size and position
			symmetryLabelStyle.fontSize = (int)Math.Round(FONTSIZE * GameSettings.UI_SCALE);
			osdLabelStyle.fontSize = (int)Math.Round(22 * GameSettings.UI_SCALE);

			//const int advancedModeOffset = 34;
			//const int angleSnapLabelSize = 43;
			advancedModeOffset = (int)Math.Floor(33 * GameSettings.UI_SCALE);
			angleSnapLabelSize = (int)Math.Floor(33* GameSettings.UI_SCALE);

			//const int angleSnapLabelLeftOffset = 209;
			//const int angleSnapLabelBottomOffset = 61;
			angleSnapLabelLeftOffset = (int)Math.Floor(231* GameSettings.UI_SCALE);
			angleSnapLabelBottomOffset = (int)Math.Floor(52* GameSettings.UI_SCALE);

			//const int symmetryLabelSize = 56;
			symmetryLabelSize = (int)Math.Floor(43* GameSettings.UI_SCALE);

			//const int symmetryLabelLeftOffset = 152;
			//const int symmetryLabelBottomOffset = 63;
			symmetryLabelLeftOffset = (int)Math.Floor(175* GameSettings.UI_SCALE);
			symmetryLabelBottomOffset = (int)Math.Floor(50* GameSettings.UI_SCALE);


			 angleSnapLabelRect = new Rect () {
				xMin = angleSnapLabelLeftOffset,
				xMax = angleSnapLabelLeftOffset + angleSnapLabelSize,
				yMin = Screen.height - angleSnapLabelBottomOffset,
				yMax = Screen.height - angleSnapLabelBottomOffset + angleSnapLabelSize
			};
			 symmetryLabelRect = new Rect () {
				xMin = symmetryLabelLeftOffset,
				xMax = symmetryLabelLeftOffset + symmetryLabelSize,
				yMin = Screen.height - symmetryLabelBottomOffset,
				yMax = Screen.height - symmetryLabelBottomOffset + symmetryLabelSize
			};
		}
		/// <summary>
		/// Hides the stock angle & symmetry sprites and replaces with textual labels
		/// </summary>
		private void ShowSnapLabels ()
		{
			if (editor == null)
				return;
			AdjustSnapLocations ();
			//editor.symmetryButton.transform.position?

			//Only show angle/symmetry sprites on parts tab
			if (editor.editorScreen == EditorScreen.Parts) {
				if (EditorLogic.Mode == EditorLogic.EditorModes.ADVANCED) {
					//in advanced mode, shift labels to the right
					angleSnapLabelRect.xMin = angleSnapLabelLeftOffset + advancedModeOffset;
					angleSnapLabelRect.xMax = angleSnapLabelLeftOffset + angleSnapLabelSize + advancedModeOffset;
					symmetryLabelRect.xMin = symmetryLabelLeftOffset + advancedModeOffset;
					symmetryLabelRect.xMax = symmetryLabelLeftOffset + symmetryLabelSize + advancedModeOffset;
				} else {
					//EditorLogic.EditorModes.SIMPLE
					//in simple mode, set back to left position
					angleSnapLabelRect.xMin = angleSnapLabelLeftOffset;
					angleSnapLabelRect.xMax = angleSnapLabelLeftOffset + angleSnapLabelSize;
					symmetryLabelRect.xMin = symmetryLabelLeftOffset;
					symmetryLabelRect.xMax = symmetryLabelLeftOffset + symmetryLabelSize;
				}

				//Radial mode 'number+R', mirror mode is 'M'/'MM'
				if (editor.symmetryMethod == SymmetryMethod.Radial) {
					symmetryLabelValue = (editor.symmetryMode + 1) + "R";
				} else if (editor.symmetryMethod == SymmetryMethod.Mirror) {
					symmetryLabelValue = (editor.symmetryMode == 0) ? "M" : "MM";
				}

				//always hide stock symmetry and mirror sprites
				editor.symmetrySprite.gameObject.SetActive (false);
				editor.mirrorSprite.gameObject.SetActive (false);

				// Show Symmetry label
				GUI.Label (symmetryLabelRect, symmetryLabelValue, symmetryLabelStyle);

				//if angle snap is on hide stock sprite
				if (GameSettings.VAB_USE_ANGLE_SNAP) {
					//editor.angleSnapSprite.Hide (true);
					editor.angleSnapSprite.gameObject.SetActive (false);
					GUI.Label (angleSnapLabelRect, editor.srfAttachAngleSnap + DegreesSymbol, symmetryLabelStyle);

				} else {
					//angle snap is off, show stock sprite
//					editor.angleSnapSprite.PlayAnim (0);
					//editor.angleSnapSprite.Hide (false);
					editor.angleSnapSprite.SetState (0);
					editor.angleSnapSprite.gameObject.SetActive (true);
				}
			}
		}

		#endregion

		#endregion
	}
}
