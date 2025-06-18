using KSP.Localization;
using System;
using UnityEngine;
using System.Collections.Generic;
using ClickThroughFix;

namespace EditorExtensionsRedux
{
	public class PartInfoWindow : MonoBehaviour
	{
		//public static SettingsWindow Instance { get; private set; }
		//public bool Visible { get; set; }

		public delegate void WindowDisabledEventHandler ();

		public event WindowDisabledEventHandler WindowDisabled;

		protected virtual void OnWindowDisabled ()
		{
			if (WindowDisabled != null)
				WindowDisabled ();
		}

		string _windowTitle = Localizer.Format("#LOC_EEX_65");

		Rect _windowRect = new Rect () {
			xMin = Screen.width - 350,
			xMax = Screen.width - 50,
			yMin = 100,
			yMax = 100 //0 height, GUILayout resizes it
		};

		//ctor
		public PartInfoWindow ()
		{

		}
		void Awake ()
		{
			//start disabled
			this.enabled = false;
        }
		#if false

		void Update ()
		{
		}

		void OnEnable ()
		{
		}
		#endif

		void CloseWindow ()
		{
			this.enabled = false;
			OnWindowDisabled ();
		}

		void OnDisable ()
		{
		}


		void OnGUI ()
		{
			if (Event.current.type == EventType.Layout) {
				//_windowRect.yMax = _windowRect.yMin;
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

		const string vectFormat = "F3";
		//GUILayoutOption[] settingsLabelLayout = new GUILayoutOption[] { GUILayout.MinWidth (150) };
		private int _toolbarInt = 0;
		private string[] _toolbarStrings = { "Part", "strut", "srfAttach", "attachNodes", "camera" };

		void WindowContent (int windowID)
		{
			_toolbarInt = GUILayout.Toolbar (_toolbarInt, _toolbarStrings);

			GUILayout.BeginVertical (Localizer.Format("#LOC_EEX_66"));

//			int activeGizmos = -1;
//			try{
			//bad cpu impact
//				activeGizmos = HighLogic.FindObjectsOfType<EditorGizmos.GizmoOffset>().Length + HighLogic.FindObjectsOfType<EditorGizmos.GizmoRotate>().Length;
//			} catch(Exception){
//			}
//			GUILayout.Label ("Gizmos: " + activeGizmos.ToString());

			//Get selected part, mouseover part if none is active
			Part sp = EditorLogic.SelectedPart;
			if (sp == null)
				sp = Utility.GetPartUnderCursor ();

			if (sp != null) {

				GUILayout.BeginHorizontal ();
				GUILayout.Label (Localizer.Format("#LOC_EEX_58"));
				GUILayout.Label (sp ? sp.name : Localizer.Format("#LOC_EEX_59"));
				GUILayout.EndHorizontal ();

				GUILayout.Label (Localizer.Format("#LOC_EEX_67") + sp.GetType ().ToString ());

				if (_toolbarInt == 0) {

					PartInfoLabels (sp);

					if (sp.srfAttachNode != null) {
						GUILayout.Label ("srfAttachNode.position: " + sp.srfAttachNode.position.ToString (vectFormat));
						GUILayout.BeginVertical (Localizer.Format("#LOC_EEX_66"));
						GUILayout.Label (Localizer.Format("#LOC_EEX_68"));
						if (sp.srfAttachNode.attachedPart != null) {
							PartInfoLabels (sp.srfAttachNode.attachedPart);
						}
						GUILayout.EndVertical ();
					}
				}

				if (_toolbarInt == 1) {
					if (sp.GetType () == typeof(CompoundPart)) {
						CompoundPartInfo ((CompoundPart)sp);
					}
				}

				if (_toolbarInt == 2) {
					SrfAttaches (sp);
				}

				if (_toolbarInt == 3) {
					AttachNodeLabels (sp);
				}




			} else {
				GUILayout.Label (Localizer.Format("#LOC_EEX_69"));
			}

			if (_toolbarInt == 4) {
				CameraInfoLabels (EditorLogic.fetch.editorCamera);
			}

			GUILayout.EndVertical ();//end main content

			if (GUILayout.Button (Localizer.Format("#LOC_EEX_51"))) {
				this.enabled = false;
			}

			GUI.DragWindow ();
		}

		void SrfAttaches (Part part)
		{
			List<Part> children = part.children;

			if (children == null) {
				GUILayout.Label (Localizer.Format("#LOC_EEX_70"));
				return;
			}

			for (int i = 0; i < children.Count; i++) {
				GUILayout.Label ("child srfAttachNode #" + i.ToString ());

				GUILayout.Label ("position " + children [i].srfAttachNode.position.ToString (vectFormat));

				if (children [i].srfAttachNode.attachedPart != null)
					GUILayout.Label ("attached part " + children [i].srfAttachNode.attachedPart.name);

				GUILayout.Label ("offset " + children [i].srfAttachNode.offset.ToString (vectFormat));
				GUILayout.Label ("orientation " + children [i].srfAttachNode.orientation.ToString (vectFormat));
				GUILayout.Label ("nodeType " + children [i].srfAttachNode.nodeType.ToString ());
				if (children [i].srfAttachNode.nodeTransform != null) {
					GUILayout.Label ("nodeTransform.position " + children [i].srfAttachNode.nodeTransform.position.ToString (vectFormat));
					GUILayout.Label ("nodeTransform.up " + children [i].srfAttachNode.nodeTransform.up.ToString (vectFormat));
				} else {
					GUILayout.Label ("nodeTransform null");
				}
			}
		}

		void AttachNodeLabels (Part part)
		{
			List<AttachNode> nodes = part.attachNodes;

			if (nodes == null) {
				GUILayout.Label ("nodes null");
				return;
			}

			for (int i = 0; i < nodes.Count; i++) {
				GUILayout.Label (Localizer.Format("#LOC_EEX_71") + i.ToString ());

				GUILayout.Label (Localizer.Format("#LOC_EEX_72") + nodes [i].position.ToString (vectFormat));

				if (nodes [i].attachedPart != null)
					GUILayout.Label (Localizer.Format("#LOC_EEX_73") + nodes [i].attachedPart.name);

				GUILayout.Label (Localizer.Format("#LOC_EEX_74") + nodes [i].offset.ToString (vectFormat));

				GUILayout.Label (Localizer.Format("#LOC_EEX_75") + nodes [i].orientation.ToString (vectFormat));
			}
		}

		void PartInfoLabels (Part part)
		{

			//GUILayout.Label ("allowSrfAttach " + (EditorLogic.SelectedPart.attachRules.allowSrfAttach ? "enabled" : "disabled"));
			GUILayout.Label ("srfAttach: " + (part.attachRules.srfAttach ? "enabled" : "disabled"));
			//GUILayout.Label ("allowCollision " + (EditorLogic.SelectedPart.attachRules.allowCollision ? "enabled" : "disabled"));
			//GUILayout.Label ("allowStack " + (EditorLogic.SelectedPart.attachRules.allowStack ? "enabled" : "disabled"));
			//GUILayout.Label ("allowDock " + (EditorLogic.SelectedPart.attachRules.allowDock ? "enabled" : "disabled"));
			//GUILayout.Label ("allowRotate " + (EditorLogic.SelectedPart.attachRules.allowRotate ? "enabled" : "disabled"));
			//GUILayout.Label ("stack " + (EditorLogic.SelectedPart.attachRules.stack ? "enabled" : "disabled"));

			//foreach (var child in part.children) {
			//	GUILayout.Label ("child: " + child.name);
			//}

			if (part.collider != null) {
				GUILayout.Label (Localizer.Format("#LOC_EEX_76"));
			}

//			if (part.gameObject != null && part.gameObject.collider != null) {
//				GUILayout.Label ("part.gameObject.collider not null");
//			}

			GUILayout.Label ("isAttached " + part.isAttached.ToString ());
			GUILayout.Label ("attRotation: " + part.attRotation.ToString (vectFormat));
			GUILayout.Label ("attRotation0: " + part.attRotation0.ToString (vectFormat));
			GUILayout.Label ("attPos: " + part.attPos.ToString (vectFormat));
			GUILayout.Label ("attPos0: " + part.attPos0.ToString (vectFormat));

			GUILayout.Label ("localPosition " + part.transform.localPosition.ToString (vectFormat));
			GUILayout.Label (Localizer.Format("#LOC_EEX_72") + part.transform.position.ToString (vectFormat));
			GUILayout.Label ("localRotation " + part.transform.localRotation.ToString (vectFormat));
			GUILayout.Label ("rotation " + part.transform.rotation.ToString (vectFormat));


			GUILayout.Label ("localScale " + part.transform.localScale.ToString (vectFormat));
			GUILayout.Label ("lossyScale " + part.transform.lossyScale.ToString (vectFormat));
			GUILayout.Label (Localizer.Format("#LOC_EEX_77") + part.transform.right.ToString (vectFormat));
			GUILayout.Label (Localizer.Format("#LOC_EEX_78") + part.transform.up.ToString (vectFormat));

			GUILayout.Label (Localizer.Format("#LOC_EEX_79") + part.GetPartRendererBound ().extents.ToString (vectFormat));
			GUILayout.Label (Localizer.Format("#LOC_EEX_80") + part.GetPartRendererBound ().size.ToString (vectFormat));

			try {				
				//GUILayout.Label ("GetPartRendererBound() extents " + part.GetPartRendererBound().extents.ToString(vectFormat));
			} catch (Exception) {
				//GUILayout.Label ("bounds.extents error");
			}

			GUILayout.Label (Localizer.Format("#LOC_EEX_81") + part.orgPos.ToString (vectFormat));
		}

		void CompoundPartInfo (CompoundPart part)
		{
			AddLabel(Localizer.Format("#LOC_EEX_82"), part.name);
			AddLabel(Localizer.Format("#LOC_EEX_83"), part.direction.ToString(vectFormat));
			AddLabel(Localizer.Format("#LOC_EEX_84"), part.transform.position.ToString(vectFormat));
			AddLabel("localPosition", part.transform.localPosition.ToString(vectFormat));
			AddLabel ("maxLength", part.maxLength.ToString ("F3"));
			AddLabel ("attachState", part.attachState.ToString ());

			if (part.target != null) {
				AddLabel(Localizer.Format("#LOC_EEX_85"), part.target.name);
				AddLabel(Localizer.Format("#LOC_EEX_86"), part.targetPosition.ToString(vectFormat));
				AddLabel("targetRotation", part.targetRotation.ToString(vectFormat));
			}
		}

		void CameraInfoLabels(Camera cam)
		{
			GUILayout.Label (Localizer.Format("#LOC_EEX_87") + cam.name);
			GUILayout.Label (Localizer.Format("#LOC_EEX_88") + cam.transform.position.ToString(vectFormat));
			GUILayout.Label (Localizer.Format("#LOC_EEX_89") + cam.aspect.ToString("F3"));
			GUILayout.Label (Localizer.Format("#LOC_EEX_90") + cam.orthographic.ToString());
			GUILayout.Label ("rotation: " + cam.transform.rotation.ToString());
			AddLabel ("projectionMatrix", cam.projectionMatrix.ToString ());
			AddLabel ("cameraToWorldMatrix", cam.cameraToWorldMatrix.ToString ());
			AddLabel ("worldToCameraMatrix", cam.worldToCameraMatrix.ToString ());

			AddLabel ("editor transform", EditorLogic.fetch.transform.ToString ());
			AddLabel ("editor position", EditorLogic.fetch.transform.position.ToString ());

			//VABCamera VABcam = Camera.main.GetComponent<VABCamera> ();
			AddLabel("dragPlaneCenter", EditorLogic.fetch.dragPlaneCenter.ToString("F3"));
			EditorCameraOffset offset = cam.GetComponent<EditorCameraOffset> ();

			AddLabel ("offset SidebarPixelWidth", offset.SidebarPixelWidth.ToString ());

			UnityEngine.Component[] comps = cam.GetComponents<UnityEngine.Component>();

			AddLabel ("selPartGrabOffset", EditorLogic.fetch.selPartGrabOffset.ToString (vectFormat));

			foreach (UnityEngine.Object c in comps) {
				AddLabel (Localizer.Format("#LOC_EEX_91"), c.GetType ().ToString ());
			}
		}

		void AddLabel(string label, string value)
		{
			GUILayout.Label (string.Format("{0}" + ":" + " {1}", label, value));
		}

	}
}

