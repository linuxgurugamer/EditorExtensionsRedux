using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace SelectRoot2 {
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class SelectRoot2Behaviour : MonoBehaviour {
		private Log log;

		private delegate void CleanupFn();
		private CleanupFn OnCleanup;

		/**
		 * The stock root selection has two states: 
		 *  - st_root_unselected: Active after switching to root mode. Waits for mouse up, picks part and sets SelectedPart.
		 *  - st_root: The state after the first click.
		 *  
		 * Skip straight to st_root state by adding an event to st_root_unselected with 
		 * always true condition that sets up SelectedPart and transitions to st_root.
		 * 
		 * Needs to run after EditorLogic#Start() so the states are initialized.
		 */
		public void Start() {
			log = new Log(this.GetType().Name);
			log.Debug("Start");

			// Oh god, so much dirty reflection. Please don't sue me, Squad :(
			KerbalFSM editorFSM = (KerbalFSM)Refl.GetValue(EditorLogic.fetch, "\u0001");

			{
				// Skip first click in root selection mode:
				KFSMEvent skipFirstClickEvent = new KFSMEvent("SelectRoot2_skipFirstClickEvent");
				skipFirstClickEvent.OnCheckCondition = (state) => {
					// Um about that always true condition... There is a funny sound glitch
					// if the root part doesn't have any ROOTABLE children so check for that.
					// Thanks Kerbas_ad_astra, http://forum.kerbalspaceprogram.com/threads/43208?p=1948755#post1948755
					return EditorReRootUtil.GetRootCandidates(EditorLogic.RootPart.GetComponentsInChildren<Part>()).Any();
				};
				skipFirstClickEvent.OnEvent = () => {
					Refl.SetValue(EditorLogic.fetch, "\u0003", EditorLogic.RootPart); // SelectedPart
				};
				KFSMState st_root = (KFSMState)Refl.GetValue(EditorLogic.fetch, "\u001c");
				skipFirstClickEvent.GoToStateOnEvent = st_root;

				KFSMState st_root_unselected = (KFSMState)Refl.GetValue(EditorLogic.fetch, "\u001b");
				InjectEvent(st_root_unselected, skipFirstClickEvent);

				// Fix ability to select if already hovering:
				KFSMStateChange fixAlreadyHoveringPartFn = (from) => {
					Part partUnderCursor = GetPartUnderCursor();
					var selectors = (List<PartSelector>)Refl.GetValue(EditorLogic.fetch, "\u0020\u0002");
					var selectorUnderCursor = selectors.Find(x => (Part)Refl.GetValue(x, "\u0001") == partUnderCursor);
					if(selectorUnderCursor) {
						Refl.Invoke(selectorUnderCursor, "OnMouseEnter");
					}
				};
				st_root.OnEnter += fixAlreadyHoveringPartFn;
				OnCleanup += () => {
					st_root.OnEnter -= fixAlreadyHoveringPartFn;
				};

				// Provide a more meaningful message after our changes:
				KFSMStateChange postNewMessageFn = (from) => {
					var template = (ScreenMessage)Refl.GetValue(EditorLogic.fetch, "\u000e");
					ScreenMessages.PostScreenMessage("Select a new root part", template, true);
				};
				st_root.OnEnter += postNewMessageFn;
				OnCleanup += () => {
					st_root.OnEnter -= postNewMessageFn;
				};
			}

			// Drop the new root part after selection:
			{
				KFSMEvent dropNewRootPartEvent = new KFSMEvent("SelectRoot2_dropNewRootPartEvent");
				dropNewRootPartEvent.OnCheckCondition = (state) => {
					return editorFSM.lastEventName.Equals("on_rootSelect");
				};
				dropNewRootPartEvent.OnEvent = () => {
					// Normally the backup is triggered in on_partDropped#OnCheckCondition()
					// But we skip that, so do backup here instead.
					EditorLogic.fetch.SetBackup();
					// Normally triggered in on_partDropped#OnEvent().
					// TODO: Do we actually need this?
					GameEvents.onEditorPartEvent.Fire(ConstructionEventType.PartDropped, EditorLogic.SelectedPart);

					var template = (ScreenMessage)Refl.GetValue(EditorLogic.fetch, "\u000e");
					ScreenMessages.PostScreenMessage(String.Empty, template, true);

				};
				KFSMState st_idle = (KFSMState)Refl.GetValue(EditorLogic.fetch, "\u0015");
				dropNewRootPartEvent.GoToStateOnEvent = st_idle;

				KFSMState st_place = (KFSMState)Refl.GetValue(EditorLogic.fetch, "\u0016");
				InjectEvent(st_place, dropNewRootPartEvent);
			}

			log.Debug("Setup complete..");
		}

		private Part GetPartUnderCursor() {
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)) {
				var part = (Part)hit.transform.gameObject.GetComponent(typeof(Part));
				if(part) return part;
			}
			return null;
		}

		private void InjectEvent(KFSMState state, KFSMEvent injectedEvent) {
			state.AddEvent(injectedEvent);
			OnCleanup += () => {
				((List<KFSMEvent>)Refl.GetValue(state, "stateEvents")).Remove(injectedEvent);
				log.Debug("Removed event {0} from state {0}", injectedEvent.name, state.name);
			};
			log.Debug("Injected event {0} into state {0}", injectedEvent.name, state.name);
		}

		public void OnDestroy() {
			log.Debug("OnDestroy");
			if(OnCleanup != null) OnCleanup();
			log.Debug("Cleanup complete.");
		}
	}

	#region Utils

	public class Log {
		private static readonly string ns = typeof(Log).Namespace;
		private readonly string id = String.Format("{0:X8}", Guid.NewGuid().GetHashCode());
		private readonly string name;

		public Log(string name) {
			this.name = name;
		}

		private void Print(string level, string message, params object[] values) {
			MonoBehaviour.print("[" + name + ":" + level + ":" + id + "]  " + String.Format(message, values));
		}

		public void Debug(string message, params object[] values) {
			Print("DEBUG", message, values);
		}

		public void Warn(string message, params object[] values) {
			Print("WARN", message, values);
		}
	}

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

	#endregion
}
