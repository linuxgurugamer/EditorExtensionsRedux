using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorGizmos;
using UnityEngine;
using System.Reflection;

#if true
namespace EditorExtensionsRedux.NoOffsetBehaviour {
	/**
	 * Removes limits from offset gizmo. Limits are enforced in EditorLogic::onOffsetGizmoUpdate. This recreates the gizmo after the 
	 * st_offset_tweak state is entered and reimplements the offset logic without limits.
	 */
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class FreeOffsetBehaviour : MonoBehaviour {
		//private Log log;
#if false
		const int ST_OFFSET_TWEAK = 73;
		const int SYMUPDATEATTACHNODE = 108;
		const int GIZMOOFFSET = 66;

		const int UPDATESYMMETRY = 64;
		const int ONOFFSETGIZMOUPDATED = 35;
#endif
#if false
		const int ST_OFFSET_TWEAK = 76;
		const int SYMUPDATEATTACHNODE = 111;
		const int GIZMOOFFSET = 69;

		const int UPDATESYMMETRY = 62;
		const int ONOFFSETGIZMOUPDATED = 35;
#endif
		private delegate void CleanupFn();
		private CleanupFn OnCleanup;

		private GizmoOffset gizmo;

		public void Start() {
			if (!EditorExtensions.validVersion)
				return;
			//log = new Log(this.GetType().Name);
			Log.Debug("Start");

//			var st_offset_tweak = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_offset_tweak");
			var st_offset_tweak = (KFSMState)Refl.GetValue(EditorLogic.fetch,EditorExtensions.c.ST_OFFSET_TWEAK);

			KFSMStateChange hookOffsetUpdateFn = (from) => {
				var p = EditorLogic.SelectedPart;
				var parent = p.parent;
				var symCount = p.symmetryCounterparts.Count;
				//var attachNode = Refl.GetValue(EditorLogic.fetch, "\u001B\u0002");
				//var attachNode = Refl.GetValue(EditorLogic.fetch, "symUpdateAttachNode");
				var symUpdateAttachNode = Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.SYMUPDATEATTACHNODE);

				gizmo = GizmoOffset.Attach(EditorLogic.SelectedPart.transform, 
					new Callback<Vector3>((offset) => {
						p.transform.position = gizmo.transform.position;
						p.attPos = p.transform.localPosition - p.attPos0;

						Log.Info("symCount: " + symCount.ToString());
						if(symCount != 0) {
//							Refl.Invoke(EditorLogic.fetch, "UpdateSymmetry", p, symCount, parent, symUpdateAttachNode);
							Refl.Invoke(EditorLogic.fetch, EditorExtensions.c.UPDATESYMMETRY, p, symCount, parent, symUpdateAttachNode);
						}

						GameEvents.onEditorPartEvent.Fire(ConstructionEventType.PartOffsetting, EditorLogic.SelectedPart);
				}), new Callback<Vector3>((offset) => {
						//Refl.Invoke(EditorLogic.fetch, "onOffsetGizmoUpdated", offset);
						Refl.Invoke(EditorLogic.fetch, EditorExtensions.c.ONOFFSETGIZMOUPDATED, offset);
				}), EditorLogic.fetch.editorCamera);

				//((GizmoOffset)Refl.GetValue(EditorLogic.fetch, "\u0012")).Detach();
				//Refl.SetValue(EditorLogic.fetch, "\u0012", gizmo);
				//((GizmoOffset)Refl.GetValue(EditorLogic.fetch, "gizmoOffset")).Detach();
				((GizmoOffset)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.GIZMOOFFSET)).Detach();
				//Refl.SetValue(EditorLogic.fetch, "gizmoOffset", gizmo);
				Refl.SetValue(EditorLogic.fetch, EditorExtensions.c.GIZMOOFFSET, gizmo);
			};
			st_offset_tweak.OnEnter += hookOffsetUpdateFn;
			OnCleanup += () => {
				st_offset_tweak.OnEnter -= hookOffsetUpdateFn;
			};
			Log.Debug("Installed.");
		}

		public void OnDestroy() {
			Log.Debug("OnDestroy");
			if(OnCleanup != null) OnCleanup();
			Log.Debug("Cleanup complete.");
		}
	}
}
#endif