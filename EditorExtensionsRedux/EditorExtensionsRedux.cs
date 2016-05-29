using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using System.Reflection;
using KSP.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

		public int GRIDSNAPINTERVAL = 1;
		public int GRIDSNAPINTERFALFINE = 2;


		// gizmo offsets
		public int GIZMOROTATE_ONHANDLEROTATESTART = 8;
		public int GIZMOROTATE_ONHANDLEROTATE = 9;
		public int GIZMOROTATE_ONHANDLEROTATEEND = 10;

		public int GIZMOOFFSET_ONHANDLEMOVESTART = 8;
		public int GIZMOOFFSET_ONHANDLEMOVE = 9;
		public int GIZMOOFFSET_ONHANDLEMOVEEND = 10;


		public bool Init ()
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
			if (Versioning.version_major == 1 && Versioning.version_minor == 1 && (Versioning.Revision == 1 || Versioning.Revision == 2) /*&& Versioning.BuildID == 1024 */) {
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

				/* Gizmo offsets
				 * 
					1 gridSnapInterval
					2 gridSnapIntervalFine
					3 useAngleSnap
					4 refCamera    
					5 pivot    
					6 rot0    
					7 hostRot0    
					8 host    
					9 onGizmoRotate    
					10 onGizmoRotated    
					11 isDragging    
					12 ssScaling    

				 * 
				 */
				GRIDSNAPINTERVAL = 1;
				GRIDSNAPINTERFALFINE = 2;

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
		public ConfigData cfg;
		string _pluginDirectory;
		string _configFilePath;
		int _symmetryMode = 0;

		SettingsWindow _settingsWindow = null;
		PartInfoWindow _partInfoWindow = null;
		FineAdjustWindow _fineAdjustWindow = null;
		//StrutWindow _strutWindow = null;

		bool enableHotkeys = true;
		//bool _gizmoActive = false;

		Vector3 cameraLookAt = new Vector3 (0, 15, 0);
		bool zoomSelected = false;

		Part oldSelectedPart = null;
		// Fwiffo
		bool rapidZoomActive = false;
		// RK
		float orgVabZoomSens = 0;
		float orgSphZoomSens = 0;
		// End Fwiffo

		static float lastSrfAttachAngleSnap = 15.0f;
		static bool last_VAB_USE_ANGLE_SNAP = true;
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
		private static string EncodeNonAsciiCharacters (string value)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (char c in value) {
				// This character is too big for ASCII
				string encodedValue = "\\u" + ((int)c).ToString ("x4");
				sb.Append (encodedValue);
			}
			return sb.ToString ();
		}

		void localdumpReflection ()
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
				Log.Info ("EditorLogic Field name[" + c.ToString () + "]: " + FI.Name + "    Fieldtype: " + FI.FieldType.ToString ());
				c++;
			}
			
			KFSMEvent ke = new KFSMEvent ("a");
			c = 0;
			foreach (FieldInfo FI in ke.GetType().GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
				Log.Info ("KFSMEvent KFSMEvent Field name[" + c.ToString () + "]: " + FI.Name + "    Fieldtype: " + FI.FieldType.ToString ());
				c++;
			}

			MethodInfo[] leMethods = typeof(EditorLogic).GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			c = 0;
			foreach (MethodInfo FI in leMethods) {
				Log.Info ("MethodInfo  EditorLogic methods name[" + c.ToString () + "]: " + FI.Name);
				c++;
			}

			MethodInfo[] parts = typeof(Part).GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			c = 0;
			foreach (MethodInfo FI in parts) {
				Log.Info ("MethodInfo  Part  name[" + c.ToString () + "]: " + FI.Name);
				c++;
			}

			MethodInfo[] kfe = typeof(KFSMEvent).GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			c = 0;
			foreach (MethodInfo FI in kfe) {
				Log.Info ("MethodInfo KFSMEvent  methods name[" + c.ToString () + "]: " + FI.Name);
				c++;
			}


			MethodInfo[] ks = typeof(KFSMState).GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			c = 0;
			foreach (MethodInfo FI in ks) {
				Log.Info ("MethodInfo KFSMState  methods name[" + c.ToString () + "]: " + FI.Name + "   " + FI.ToString ());
				c++;
			}


		}
		#endif

		//Unity, called after Awake()
		public void Start ()
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


//			editor.srfAttachAngleSnap = 0;
			editor.srfAttachAngleSnap = lastSrfAttachAngleSnap;
			GameSettings.VAB_USE_ANGLE_SNAP = last_VAB_USE_ANGLE_SNAP;
			Log.Info ("editor.srfAttachAngleSnap: " + editor.srfAttachAngleSnap.ToString ());

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

		ConstructionEventType lastEventType = ConstructionEventType.Unknown;
		void EditorPartEvent (ConstructionEventType eventType, Part part)
		{
			Log.Info ("EditorPartEvent  eventType: " + eventType.ToString ());
			lastEventType = eventType;
			if (eventType == ConstructionEventType.PartRotating)
			{
				Log.Info ("eulerAngles attRotation: " + part.attRotation.eulerAngles);
				Log.Info ("eulerAngles attRotation0: " + part.attRotation0.eulerAngles);
				updateGizmoSnaps ();
				return;
				//	  UpdateRotationGizmos(); // RK
			}

			if (eventType == ConstructionEventType.PartOffsetting) {
				return;
			}

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

		void EditorSymmetryModeChange (int symMode)
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

//		EditorGizmos.GizmoOffsetHandle gizmoOffsetHandle = null;
//		EditorGizmos.GizmoRotateHandle gizmoRotateHandle = null;
		//Unity update
		void Update ()
		{
			if (!validVersion)
				return;
			//if (editor.shipNameField.Focused || editor.shipDescriptionField.Focused)
			//	return;
			GameObject obj = EventSystem.current.currentSelectedGameObject;
			bool inputFieldIsFocused = (obj != null && obj.GetComponent<InputField> () != null && obj.GetComponent<InputField> ().isFocused);
			if (inputFieldIsFocused)
				return;
			//ignore hotkeys while settings window is open
			//if (_settingsWindow != null && _settingsWindow.enabled)
			//	return;

			//hotkeyed editor functions
			if (enableHotkeys) {

				//check for the configured modifier key
				bool modKeyDown = GameSettings.MODIFIER_KEY.GetKey ();
				//check for configured editor fine key
				bool fineKeyDown = GameSettings.Editor_fineTweak.GetKey ();

				Camera cam = editor.editorCamera;
				// Fwiffo
				VABCamera vabCam = Camera.main.GetComponent<VABCamera> (); // or EditorDriver.fetch.vabCamera; // RK
				SPHCamera sphCam = Camera.main.GetComponent<SPHCamera> (); // or EditorDriver.fetch.sphCamera;

				// Zoom cycling - tap then quickly hold a zoom key zoom more rapidly (original idea was to double tap to rapidly cycle through presets)
				//if (GameSettings.ZOOM_IN.GetDoubleTapDown()) CycleZoom(cam, true);
				//else if (GameSettings.ZOOM_OUT.GetDoubleTapDown()) CycleZoom(cam, false);
				if (rapidZoomActive && (GameSettings.ZOOM_IN.GetKeyUp () || GameSettings.ZOOM_OUT.GetKeyUp ())) {
					//GameSettings.VAB_CAMERA_ZOOM_SENS = orgZoomSens;
					vabCam.mouseZoomSensitivity = orgVabZoomSens;
					sphCam.mouseZoomSensitivity = orgSphZoomSens;
					rapidZoomActive = false;
					//Debug.Log("Rapid zoom deactivated; VAB_CAMERA_ZOOM_SENS = " + GameSettings.VAB_CAMERA_ZOOM_SENS);
					Debug.Log ("Rapid zoom deactivated; sensitivity = " +
					((EditorDriver.editorFacility == EditorFacility.VAB) ? orgVabZoomSens : orgSphZoomSens).ToString ());
				} else if (cfg.RapidZoom
				           && !rapidZoomActive
					&& (GameSettings.ZOOM_IN.GetDoubleTapDown () || GameSettings.ZOOM_OUT.GetDoubleTapDown ())) {
					//orgZoomSens = GameSettings.VAB_CAMERA_ZOOM_SENS;
					//GameSettings.VAB_CAMERA_ZOOM_SENS = 1;
					orgVabZoomSens = vabCam.mouseZoomSensitivity;
					orgSphZoomSens = sphCam.mouseZoomSensitivity;
					vabCam.mouseZoomSensitivity *= 5;
					sphCam.mouseZoomSensitivity *= 5;
					rapidZoomActive = true;
					Debug.Log ("Rapid zoom activated; sensitivity = " +	((EditorDriver.editorFacility == EditorFacility.VAB)
               										? vabCam.mouseZoomSensitivity : sphCam.mouseZoomSensitivity).ToString ());
				}
// Fwiffo end

				//Zoom selected part - rotate camera around part
				if (Input.GetKeyDown (cfg.KeyMap.ZoomSelected)) {
					Part p = Utility.GetPartUnderCursor ();
					if (p != null) {
						zoomSelected = true;
						cameraLookAt = p.transform.position;
						cam.transform.position = new Vector3 (cam.transform.position.x, p.transform.position.y, cam.transform.position.z);
						OSDMessage (string.Format ("Zoom Camera on {0}", p.name));
					} else {
						cameraLookAt = new Vector3 (0, 15, 0);
						OSDMessage ("Default Camera");
						ResetCamera ();
						zoomSelected = false;
					}
				}

				if (zoomSelected) {
					cam.transform.LookAt (cameraLookAt);
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
					HorizontalAlign (fineKeyDown);
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
				if (GameSettings.Editor_toggleAngleSnap.GetKeyUp ()) {
					AngleSnapCycle (modKeyDown, fineKeyDown);
					return;	
				}
			
				//using gamesettings keybinding Input.GetKeyDown (cfg.KeyMap.Symmetry)
				// X, Shift+X : Increment/decrement symmetry mode
				if (GameSettings.Editor_toggleSymMode.GetKeyUp ()) {
					SymmetryModeCycle (modKeyDown, fineKeyDown);
					return;
				}


				if (_fineAdjustWindow.isEnabled ()) {
					Vector3 axis;
//					var gizmosOffset = HighLogic.FindObjectsOfType<EditorGizmos.GizmoOffset> ();
//					if (gizmosOffset.Length > 0) {
					if (GizmoEvents.offsetGizmoActive) {
						GizmoEvents.gizmoRotateHandle = null;
						GizmoEvents.rotateGizmoActive = false;
						if (EditorLogic.SelectedPart != null) {
							//var gizmosOffset = HighLogic.FindObjectsOfType<EditorGizmos.GizmoOffset> ();
//							if (gizmoOffsetHandle == null)
//								gizmoOffsetHandle = HighLogic.FindObjectOfType<EditorGizmos.GizmoOffsetHandle> ();

							if (GameSettings.VAB_USE_ANGLE_SNAP)
								GameEvents.onEditorSnapModeChange.Fire (false);

							float offset = FineAdjustWindow.Instance.offset;

							Log.Info ("\nmoving part:  EditorLogic.SelectedPart.attPos: " + EditorLogic.SelectedPart.attPos);
							Log.Info ("moving part:  EditorLogic.SelectedPart.attPos0: " + EditorLogic.SelectedPart.attPos0);
							/*
					 * From WASD:

						public class Config
						{
							public KeyCode keyForward;
							public KeyCode keyBack;
							public KeyCode keyRight;
							public KeyCode keyLeft;
							public KeyCode keyUp;
							public KeyCode keyDown;
							public KeyCode keyRun;
							public KeyCode keySneak;
							public KeyCode keySwitchMode;

					 */

							if (Input.GetKey (cfg.KeyMap.Down)) {
								axis = Vector3.down;
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVESTART, GizmoEvents.gizmoOffsetHandle, axis);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVE, GizmoEvents.gizmoOffsetHandle, axis, offset);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVEEND, GizmoEvents.gizmoOffsetHandle, axis, 0.0f);
							}
							if (Input.GetKey (cfg.KeyMap.Up)) {
								axis = Vector3.up;
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVESTART, GizmoEvents.gizmoOffsetHandle, axis);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVE, GizmoEvents.gizmoOffsetHandle, axis, offset);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVEEND, GizmoEvents.gizmoOffsetHandle, axis, 0.0f);
							}

							if (Input.GetKey (cfg.KeyMap.Left)) {
								if (EditorDriver.editorFacility == EditorFacility.VAB)
									axis = Vector3.forward;
								else
									axis = Vector3.right;
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVESTART, GizmoEvents.gizmoOffsetHandle, axis);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVE, GizmoEvents.gizmoOffsetHandle, axis, offset);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVEEND, GizmoEvents.gizmoOffsetHandle, axis, 0.0f);
							}
							if (Input.GetKey (cfg.KeyMap.Right)) {
								if (EditorDriver.editorFacility == EditorFacility.VAB)
									axis = Vector3.back;
								else
									axis = Vector3.left;
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVESTART, GizmoEvents.gizmoOffsetHandle, axis);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVE, GizmoEvents.gizmoOffsetHandle, axis, offset);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVEEND, GizmoEvents.gizmoOffsetHandle, axis, 0.0f);

							}

							if (Input.GetKey (cfg.KeyMap.Forward)) {
								if (EditorDriver.editorFacility == EditorFacility.VAB)
									axis = Vector3.right;
								else
									axis = Vector3.back;
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVESTART, GizmoEvents.gizmoOffsetHandle, axis);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVE, GizmoEvents.gizmoOffsetHandle, axis, offset);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVEEND, GizmoEvents.gizmoOffsetHandle, axis, 0.0f);

							}
							if (Input.GetKey (cfg.KeyMap.Back)) {
								if (EditorDriver.editorFacility == EditorFacility.VAB)
									axis = Vector3.left;
								else
									axis = Vector3.forward;
								
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVESTART, GizmoEvents.gizmoOffsetHandle, axis);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVE, GizmoEvents.gizmoOffsetHandle, axis, offset);
								Refl.Invoke (GizmoEvents.gizmosOffset [0], EditorExtensions.c.GIZMOOFFSET_ONHANDLEMOVEEND, GizmoEvents.gizmoOffsetHandle, axis, 0.0f);

							}
						} else {
							GizmoEvents.gizmoOffsetHandle = null;
							GizmoEvents.offsetGizmoActive = false;
						}
					} else {
						GizmoEvents.gizmoOffsetHandle = null;
						GizmoEvents.offsetGizmoActive = false;
//						var gizmosRotate = HighLogic.FindObjectsOfType<EditorGizmos.GizmoRotate> ();
//						if (gizmosRotate.Length > 0) {
						if (GizmoEvents.rotateGizmoActive) {
							if (EditorLogic.SelectedPart != null) {
								//var gizmosRotate = HighLogic.FindObjectsOfType<EditorGizmos.GizmoRotate> ();
//								if (gizmoRotateHandle == null)
//									gizmoRotateHandle = HighLogic.FindObjectOfType<EditorGizmos.GizmoRotateHandle> ();
								float rotation = FineAdjustWindow.Instance.rotation;

								if (Input.GetKey (cfg.KeyMap.Down)) {
									if (EditorDriver.editorFacility == EditorFacility.VAB)
										axis = Vector3.forward;
									else
										axis = Vector3.left;
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATESTART, GizmoEvents.gizmoRotateHandle, axis);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATE, GizmoEvents.gizmoRotateHandle, axis, rotation);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATEEND, GizmoEvents.gizmoRotateHandle, axis, 0.0f);
								}
								if (Input.GetKey (cfg.KeyMap.Up)) {
									if (EditorDriver.editorFacility == EditorFacility.VAB)
										axis = Vector3.back;
									else
										axis = Vector3.right;
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATESTART, GizmoEvents.gizmoRotateHandle, axis);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATE, GizmoEvents.gizmoRotateHandle, axis, rotation);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATEEND, GizmoEvents.gizmoRotateHandle, axis, 0.0f);
								}
								if (Input.GetKey (cfg.KeyMap.Left)) {
									if (EditorDriver.editorFacility == EditorFacility.VAB)
										axis = Vector3.right;
									else
										axis = Vector3.forward;
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATESTART, GizmoEvents.gizmoRotateHandle, axis);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATE, GizmoEvents.gizmoRotateHandle, axis, rotation);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATEEND, GizmoEvents.gizmoRotateHandle, axis, 0.0f);
								}
								if (Input.GetKey (cfg.KeyMap.Right)) {
									if (EditorDriver.editorFacility == EditorFacility.VAB)
										axis = Vector3.left;
									else
										axis = Vector3.back;
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATESTART, GizmoEvents.gizmoRotateHandle, axis);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATE, GizmoEvents.gizmoRotateHandle, axis, rotation);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATEEND, GizmoEvents.gizmoRotateHandle, axis, 0.0f);

								}
								if (Input.GetKey (cfg.KeyMap.Forward)) {
								
									axis = Vector3.up;
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATESTART, GizmoEvents.gizmoRotateHandle, axis);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATE, GizmoEvents.gizmoRotateHandle, axis, rotation);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATEEND, GizmoEvents.gizmoRotateHandle, axis, 0.0f);

								}
								if (Input.GetKey (cfg.KeyMap.Back)) {
									axis = Vector3.down;
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATESTART, GizmoEvents.gizmoRotateHandle, axis);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATE, GizmoEvents.gizmoRotateHandle, axis, rotation);
									Refl.Invoke (GizmoEvents.gizmosRotate [0], EditorExtensions.c.GIZMOROTATE_ONHANDLEROTATEEND, GizmoEvents.gizmoRotateHandle, axis, 0.0f);

								}
							} else {
								GizmoEvents.gizmoRotateHandle = null;
								GizmoEvents.rotateGizmoActive = false;
							}
						}
						else
							{
							GizmoEvents.gizmoRotateHandle = null;
							GizmoEvents.rotateGizmoActive = false;
						}
					}
				}


			}//end if(enableHotKeys)

			if (oldSelectedPart != EditorLogic.SelectedPart) {
				oldSelectedPart = EditorLogic.SelectedPart;
				updateGizmoSnaps ();
			}
		}

		bool IsPartNodeAttached (Part p)
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

		void AlignToTopOfParent (Part p)
		{
			if (p.parent.GetPartRendererBound ().extents.y >= p.GetPartRendererBound ().extents.y) {
				CenterVerticallyOnParent (p);
				return;
			}

			float newHeight = p.parent.GetPartRendererBound ().extents.y - p.GetPartRendererBound ().extents.y;
			if (p.transform.localPosition.y < 0)
				newHeight = -newHeight;

			VerticalPositionOnParent (p, newHeight);

			throw new NotImplementedException ();
		}

		void CenterVerticallyOnParent (Part p)
		{
			VerticalPositionOnParent (p, 0f);
		}

		public void AdjustVerticalPositionOnParent(Part p, float position)
		{
			//move hovered part
			VerticalPositionOnParent (p, position);

			//move any symmetry siblings/counterparts
			foreach (Part symPart in p.symmetryCounterparts) {
				VerticalPositionOnParent (symPart, position);
			}

		}

		public void VerticalPositionOnParent (Part p, float position)
		{
			if (p.parent != null) {
				Log.Debug (string.Format ("Positioning {0} vertically on parent {1}", p.name, p.parent.name));
				p.transform.localPosition = new Vector3 (p.transform.localPosition.x, position, p.transform.localPosition.z);
				p.attPos0.y = position;
			}
		}

		public enum horizontaltype { leftright, forwardback};
		public void AdjustHorizontalPositionOnParent(Part p,horizontaltype type, float position, float adj)
		{
			
			HorizontalPositionOnParent (p, type, position);
			foreach (Part sympart in p.symmetryCounterparts) {
				switch (type) {
				case horizontaltype.leftright:
					HorizontalPositionOnParent (sympart, type, sympart.attPos0.z - adj);
					break;
				case horizontaltype.forwardback:
					HorizontalPositionOnParent (sympart, type, sympart.attPos0.x - adj);
					break;
				}
				//HorizontalPositionOnParent (sympart, type, position);
			}
		}
		public void HorizontalPositionOnParent (Part p,horizontaltype type, float position)
		{
			if (p.parent != null) {
				switch (type) {
				case horizontaltype.forwardback:
					p.transform.localPosition = new Vector3 (position, p.transform.localPosition.y,  p.transform.localPosition.z);
					p.attPos0.x = position;
					break;
				case horizontaltype.leftright:
					p.transform.localPosition = new Vector3 (p.transform.localPosition.x, p.transform.localPosition.y, position);
					p.attPos0.z = position;
					break;
				}
			}
		}

		public enum axis {x, y, z};
		public void RotatePartOnParent(Part p, axis xyz, float amt)
		{
			RotatePart(p, xyz, amt);
			foreach (Part sympart in p.symmetryCounterparts) {
				RotatePart (sympart, xyz, amt);
			}
		}
		public void RotatePart(Part p, axis xyz, float amt)
		{
			Quaternion r = p.attRotation0;
			switch (xyz) {
			case axis.x:
				r *= Quaternion.Euler (Vector3.up * amt);
				break;
			case axis.y:
				r *= Quaternion.Euler (Vector3.left * amt);
				break;
			case axis.z:
				r *= Quaternion.Euler (Vector3.forward * amt);
				break;
			}
			Log.Info ("RotatePart  attRotation0: " + p.attRotation0.eulerAngles);
			Log.Info ("RotatePart  r: " + r.eulerAngles);
			//p.orgRot = r;
			p.transform.localRotation = r;
		}




		void CenterOnParent (Part p)
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

		void CenterHorizontallyOnParent (Part p, bool otherHorizontal = false)
		{
            if (otherHorizontal) {
                p.transform.localPosition = new Vector3(0f, p.transform.localPosition.y, p.transform.localPosition.z);
                p.attPos0.x = 0f;
            } else {
                p.transform.localPosition = new Vector3(p.transform.localPosition.x, p.transform.localPosition.y, 0f);
                p.attPos0.z = 0f;
            }
        }

		void VerticalAlign ()
		{
			try {
				Part sp = Utility.GetPartUnderCursor ();

				if (sp != null && sp.srfAttachNode != null && sp.srfAttachNode.attachedPart != null && !GizmoActive () && !IsPartNodeAttached (sp)) {

					//move hovered part
					CenterVerticallyOnParent (sp);

					//move any symmetry siblings/counterparts
					foreach (Part symPart in sp.symmetryCounterparts) {
						CenterVerticallyOnParent (symPart);
					}

					AddUndo ();
				}
			} catch (Exception ex) {
				Log.Error ("Error trying to vertically align: " + ex.Message);
			}

			return;
		}

		void HorizontalAlign (bool otherHorizontal = false)
		{
			try {
				Part sp = Utility.GetPartUnderCursor ();

				if (sp != null && sp.srfAttachNode != null && sp.srfAttachNode.attachedPart != null && !GizmoActive () && !IsPartNodeAttached (sp)) {

					//Part ap = sp.srfAttachNode.attachedPart;
					List<Part> symParts = sp.symmetryCounterparts;

					Log.Debug ("symmetryCounterparts to move: " + symParts.Count.ToString ());

					//move selected part
					CenterHorizontallyOnParent (sp, otherHorizontal);

					//move any symmetry siblings/counterparts
					foreach (Part symPart in symParts) {
						CenterHorizontallyOnParent (symPart, otherHorizontal);
					}

					//Add edit to undo history
					AddUndo ();
				}
			} catch (Exception ex) {
				Log.Error ("Error trying to Horizontally align: " + ex.Message);
			}
			return;
		}

		void AlignCompoundPart (CompoundPart part, bool snapHeights)
		{
			if (part.target != null && part.parent != null) {
				CompoundPartUtil.AlignCompoundPart (part, snapHeights);

				List<Part> symParts = part.symmetryCounterparts;
				//move any symmetry siblings/counterparts
				foreach (CompoundPart symPart in symParts) {
					CompoundPartUtil.AlignCompoundPart (symPart, snapHeights);
				}
				AddUndo ();
			}
		}

		#endregion

		#region Editor Actions

		void AddUndo ()
		{
			//need to verify this is the right way, it does seem to work
			editor.SetBackup ();
		}

		void ResetCamera ()
		{
			if (!GizmoActive ()) {

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

		void SurfaceAttachToggle ()
		{
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

		void PartClippingToggle ()
		{
			CheatOptions.AllowPartClipping ^= true;
			Log.Debug ("AllowPartClipping " + (CheatOptions.AllowPartClipping ? "enabled" : "disabled"));
			OSDMessage ("Part clipping " + (CheatOptions.AllowPartClipping ? "enabled" : "disabled"));
			return;
		}

		void SymmetryModeCycle (bool modKeyDown, bool fineKeyDown)
		{

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

		void updateGizmoSnaps ()
		{
			// Following code  contributed by Fwiffo

			// Look for active rotation gizmo and change its snap resolution
			var gizmosRotate = HighLogic.FindObjectsOfType<EditorGizmos.GizmoRotate> ();
			#if false
			if (gizmosRotate.Length > 0) {
				
				// Chunk to find the magic variable we need to adjust
			
				var gizmoRotate = gizmosRotate [0];

				int cnt = 0;
				var fields = gizmoRotate.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				foreach (var f in fields) {
					Log.Info ("EditorLogic Gizmo Rotate Field name[" + cnt.ToString () + "]: " + f.Name + "    Fieldtype: " + f.GetValue(gizmoRotate).ToString());
					cnt++;

					// Debug.Log(String.Format("{0}: {1}", f.Name, f.GetValue(gizmo).ToString()));
				}


				MethodInfo[] egMethods = typeof(EditorGizmos.GizmoRotate).GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				cnt = 0;
				foreach (MethodInfo EG in egMethods) {
					Log.Info ("EditorLogic Gizmo Rotate methods name[" + cnt.ToString () + "]: " + EG.Name + "   " + EG.ReturnType.ToString());
					cnt++;
				
				}

			}
			#endif
			#if false
			var gizmosOffsets = HighLogic.FindObjectsOfType<EditorGizmos.GizmoOffset> ();
			if (gizmosOffsets.Length > 0) {
				var gizmoOffset = gizmosOffsets [0];

				var cnt = 0;
				var gizmoOffsetFields = gizmoOffset.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				foreach (var f in gizmoOffsetFields) {
					Log.Info ("EditorLogic Gizmo Offset Field name[" + cnt.ToString () + "]: " + f.Name + "    Fieldtype: " + f.GetValue(gizmoOffset).ToString());
					cnt++;

					// Debug.Log(String.Format("{0}: {1}", f.Name, f.GetValue(gizmo).ToString()));
				}


				MethodInfo[] gizmoOffsetMethods = typeof(EditorGizmos.GizmoOffset).GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				cnt = 0;
				foreach (MethodInfo EG in gizmoOffsetMethods) {
					Log.Info ("EditorLogic Gizmo Offset methods name[" + cnt.ToString () + "]: " + EG.Name + "   " + EG.ReturnType.ToString());
					cnt++;
				}
				
			}
			#endif
			if (gizmosRotate.Length > 0) {
				var g = gizmosRotate [0];
//fix1
				g.useAngleSnap = editor.srfAttachAngleSnap != 0 && GameSettings.VAB_USE_ANGLE_SNAP;
				// Unfortunately the SnapDegrees property is read-only; some reflection hackery is needed
				if (editor.srfAttachAngleSnap != 0 && GameSettings.VAB_USE_ANGLE_SNAP) {


//					var field = gizmo.GetType ().GetField ("gridSnapInterval", BindingFlags.NonPublic | BindingFlags.Instance);
//					field.SetValue (gizmo, editor.srfAttachAngleSnap);
//					field = gizmo.GetType ().GetField ("gridSnapIntervalFine", BindingFlags.NonPublic | BindingFlags.Instance);
					float fine = editor.srfAttachAngleSnap / 3f;
//					field.SetValue (gizmo, fine);
					//fine = (float)field.GetValue(gizmo);

					Refl.SetValue (g, EditorExtensions.c.GRIDSNAPINTERVAL, editor.srfAttachAngleSnap);
					Refl.SetValue (g, EditorExtensions.c.GRIDSNAPINTERFALFINE, fine);

					Debug.Log (String.Format ("Gizmo SnapDegrees = {0}, fine = {1}", g.SnapDegrees.ToString (), fine.ToString ()));
				}
			}
		}

		void AngleSnapCycle (bool modKeyDown, bool fineKeyDown)
		{
			if (!modKeyDown) {
				Log.Debug ("Starting srfAttachAngleSnap = " + editor.srfAttachAngleSnap.ToString ());

				int currentAngleIndex = cfg.AngleSnapValues.IndexOf (editor.srfAttachAngleSnap);
				#if false
	if (currentAngleIndex < 0)
		currentAngleIndex = 0;
				#endif
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
				#if false
editor.srfAttachAngleSnap = 0.01f;
GameSettings.VAB_USE_ANGLE_SNAP = true;
editor.angleSnapSprite.gameObject.SetActive (false);
				#endif
			} else {
				GameSettings.VAB_USE_ANGLE_SNAP = true;
			}

			lastSrfAttachAngleSnap = editor.srfAttachAngleSnap;
			last_VAB_USE_ANGLE_SNAP = GameSettings.VAB_USE_ANGLE_SNAP;

			updateGizmoSnaps ();
// Fwiffo
			// Fix offset gizmo if shown
			var gizmos = HighLogic.FindObjectsOfType<EditorGizmos.GizmoOffset> ();
			if (gizmos.Length > 0) {
				var gizmo = gizmos [0];
				if (editor.srfAttachAngleSnap == 0 && gizmo.useGrid)
					gizmo.useGrid = false;
				else if (editor.srfAttachAngleSnap != 0 && !gizmo.useGrid)
					gizmo.useGrid = true;
				Debug.Log ("Offset snap interval = " + gizmo.SnapInterval.ToString () + ", using grid = " + gizmo.useGrid);
			}
// Fwiffo end
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
			_fineAdjustWindow = this.gameObject.AddComponent<FineAdjustWindow> ();

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

		public void SettingsWindowClosed ()
		{
			Log.Debug ("Settings window closed, reloading config");
			cfg = ConfigManager.LoadConfig (_configFilePath);
			Hide ();
		}

		bool _showMenu = false;
		Rect _menuRect = new Rect ();
		const float _menuWidth = 100.0f;
		const float _menuHeight = 100.0f;
		const int _toolbarHeight = 42;
		//37

		public void ShowMenu ()
		{
			if (!validVersion)
				return;
			Vector3 position = Input.mousePosition;
			int toolbarHeight = (int)(_toolbarHeight * GameSettings.UI_SCALE);
			_menuRect = new Rect () {
				xMin = position.x - _menuWidth / 2,
				xMax = position.x + _menuWidth / 2,
				yMin = Screen.height - toolbarHeight - _menuHeight,
				yMax = Screen.height - toolbarHeight
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
				string kspVersion = Versioning.version_major.ToString () + "." + Versioning.version_minor.ToString () + "." + Versioning.Revision.ToString ();
				string warning2 = warning + "\nKSP version: " + kspVersion;
				Vector2 sizeOfWarningLabel = centeredWarningStyle.CalcSize (new GUIContent (warning2));


				Rect _menuRect = new Rect (Screen.width / 2f - (sizeOfWarningLabel.x / 2f), Screen.height / 2 - sizeOfWarningLabel.y, 
					                 sizeOfWarningLabel.x, sizeOfWarningLabel.y * 2);

				_menuRect = GUILayout.Window (this.GetInstanceID (), _menuRect, ShowWarning, "EEX Menu");
				return;
			}


			//show and update the angle snap and symmetry mode labels
			ShowSnapLabels ();

			if (Event.current.type == EventType.Layout) {
				if (_showMenu || _menuRect.Contains (Event.current.mousePosition) || (Time.fixedTime - lastTimeShown <0.5f))
					_menuRect = GUILayout.Window (this.GetInstanceID (), _menuRect, MenuContent, "EEX Menu");
				else
					_menuRect = new Rect ();
			}
		}
		float lastTimeShown = 0.0f;

		void MenuContent (int WindowID)
		{
			if (_showMenu || _menuRect.Contains (Event.current.mousePosition))
				lastTimeShown = Time.fixedTime;
			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Settings")) {
				_settingsWindow.Show (cfg, _configFilePath, pluginVersion);
				this.Visible = true;
			}
			if (GUILayout.Button ("Fine Adjust")) {
				_fineAdjustWindow.Show ();

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
				float offsetY = Mathf.FloorToInt (0.8f * Screen.height);
				GUIStyle centeredWarningStyle = new GUIStyle (GUI.skin.GetStyle ("Label")) {
					alignment = TextAnchor.UpperCenter,
					fontSize = 16,
					normal = { textColor = Color.yellow }
				};

				Vector2 sizeOfWarningLabel = centeredWarningStyle.CalcSize (new GUIContent (warning));

				GUILayout.Label (warning, centeredWarningStyle);

				offsetY += sizeOfWarningLabel.y;
				if (GUILayout.Button ("Click to open the Forum thread"))
					Application.OpenURL ("http://forum.kerbalspaceprogram.com/index.php?/topic/127378-editor-extensions-redux-324-released-for-111-with-selectroot-merge-stripsymmetry-nooffsetlimits/");

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


		Rect angleSnapLabelRect;
		/* = new Rect () {
			xMin = angleSnapLabelLeftOffset,
			xMax = angleSnapLabelLeftOffset + angleSnapLabelSize,
			yMin = Screen.height - angleSnapLabelBottomOffset,
			yMax = Screen.height - angleSnapLabelBottomOffset + angleSnapLabelSize
		}; */
		Rect symmetryLabelRect;
		/* = new Rect () {
			xMin = symmetryLabelLeftOffset,
			xMax = symmetryLabelLeftOffset + symmetryLabelSize,
			yMin = Screen.height - symmetryLabelBottomOffset,
			yMax = Screen.height - symmetryLabelBottomOffset + symmetryLabelSize
		};*/

		void AdjustSnapLocations ()
		{
			
			if (editor.srfAttachAngleSnap == 0) {
				GameSettings.VAB_USE_ANGLE_SNAP = false;
			} else {
			//	GameSettings.VAB_USE_ANGLE_SNAP = true;
			}
			last_VAB_USE_ANGLE_SNAP = GameSettings.VAB_USE_ANGLE_SNAP;

			//symmetry & angle sprite/label size and position
			symmetryLabelStyle.fontSize = (int)Math.Round (FONTSIZE * GameSettings.UI_SCALE);
			osdLabelStyle.fontSize = (int)Math.Round (22 * GameSettings.UI_SCALE);

			//const int advancedModeOffset = 34;
			//const int angleSnapLabelSize = 43;
			advancedModeOffset = (int)Math.Floor (33 * GameSettings.UI_SCALE);
			angleSnapLabelSize = (int)Math.Floor (33 * GameSettings.UI_SCALE);

			//const int angleSnapLabelLeftOffset = 209;
			//const int angleSnapLabelBottomOffset = 61;
			angleSnapLabelLeftOffset = (int)Math.Floor (231 * GameSettings.UI_SCALE);
			angleSnapLabelBottomOffset = (int)Math.Floor (52 * GameSettings.UI_SCALE);

			//const int symmetryLabelSize = 56;
			symmetryLabelSize = (int)Math.Floor (43 * GameSettings.UI_SCALE);

			//const int symmetryLabelLeftOffset = 152;
			//const int symmetryLabelBottomOffset = 63;
			symmetryLabelLeftOffset = (int)Math.Floor (175 * GameSettings.UI_SCALE);
			symmetryLabelBottomOffset = (int)Math.Floor (50 * GameSettings.UI_SCALE);


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
//				Log.Info ("ShowSnapLabels disabling sprites, GameSettings.VAB_USE_ANGLE_SNAP: " + GameSettings.VAB_USE_ANGLE_SNAP.ToString());

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
