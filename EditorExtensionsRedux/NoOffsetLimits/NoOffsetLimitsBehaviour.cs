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

		private delegate void CleanupFn();
		private CleanupFn OnCleanup;

		private GizmoOffset gizmo;

		public void Start() {
			//log = new Log(this.GetType().Name);
			Log.Debug("Start");

			var st_offset_tweak = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_offset_tweak");
			
			KFSMStateChange hookOffsetUpdateFn = (from) => {
				var p = EditorLogic.SelectedPart;
				var parent = p.parent;
				var symCount = p.symmetryCounterparts.Count;
				//var attachNode = Refl.GetValue(EditorLogic.fetch, "\u001B\u0002");
				var attachNode = Refl.GetValue(EditorLogic.fetch, "symUpdateAttachNode");

				gizmo = GizmoOffset.Attach(EditorLogic.SelectedPart.transform, 
					new Callback<Vector3>((offset) => {
						p.transform.position = gizmo.transform.position;
						p.attPos = p.transform.localPosition - p.attPos0;

						Log.Info("symCount: " + symCount.ToString());
						if(symCount != 0) {
							Refl.Invoke(EditorLogic.fetch, "UpdateSymmetry", p, symCount, parent, attachNode);
						}

						GameEvents.onEditorPartEvent.Fire(ConstructionEventType.PartOffsetting, EditorLogic.SelectedPart);
				}), new Callback<Vector3>((offset) => {
					Refl.Invoke(EditorLogic.fetch, "onOffsetGizmoUpdated", offset);
				}), EditorLogic.fetch.editorCamera);

				//((GizmoOffset)Refl.GetValue(EditorLogic.fetch, "\u0012")).Detach();
				//Refl.SetValue(EditorLogic.fetch, "\u0012", gizmo);
				((GizmoOffset)Refl.GetValue(EditorLogic.fetch, "gizmoOffset")).Detach();
				Refl.SetValue(EditorLogic.fetch, "gizmoOffset", gizmo);
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

#if false
	public static class Refl {
		public static FieldInfo GetField(object obj, string name) {
			var f = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if(f == null) throw new Exception("No such field: " + obj.GetType() + "#" + name);
			return f;
		}
		public static object GetValue(object obj, string name) {
			return GetField(obj, name).GetValue(obj);
		}
		public static void SetValue(object obj, string name, object value) {
			GetField(obj, name).SetValue(obj, value);
		}
		public static MethodInfo GetMethod(object obj, string name) {
			var m = obj.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if(m == null) throw new Exception("No such method: " + obj.GetType() + "#" + name);
			return m;
		}
		public static object Invoke(object obj, string name, params object[] args) {
			return GetMethod(obj, name).Invoke(obj, args);
		}
	}
#endif
}
#endif