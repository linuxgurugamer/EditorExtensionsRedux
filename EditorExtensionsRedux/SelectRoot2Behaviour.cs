#if true
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

//using EditorExtensionsRedux;

namespace EditorExtensionsRedux.SelectRoot2 {
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class SelectRoot2Behaviour : MonoBehaviour {
	//	private Log log;

		private delegate void CleanupFn();
		private CleanupFn OnCleanup;

		/**
		 * The stock root selection has two states: 
		 *  - st_root_unselected: Active after switching to root mode. Waits for mouse up, picks part and sets SelectedPart.
		 *  - st_root_select: The state after the first click.
		 *  
		 * Skip straight to st_root state by adding an event to st_root_unselected with 
		 * always true condition that sets up SelectedPart and transitions to st_root.
		 * 
		 * Needs to run after EditorLogic#Start() so the states are initialized.
		 */
		public void Start() {
			//log = new Log(this.GetType().Name);
			Log.Info("Start");

			// Oh god, so much dirty reflection. Please don't sue me, Squad :(
			//KerbalFSM editorFSM = (KerbalFSM)Refl.GetValue(EditorLogic.fetch, "\u0001");

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
					Refl.SetValue(EditorLogic.fetch, "selectedPart", EditorLogic.RootPart); // SelectedPart
				};
				KFSMState st_root_select = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_root_select");
				skipFirstClickEvent.GoToStateOnEvent = st_root_select;

				KFSMState st_root_unselected = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_root_unselected");
				InjectEvent(st_root_unselected, skipFirstClickEvent);
			
				// Fix ability to select if already hovering:
				KFSMStateChange fixAlreadyHoveringPartFn = (from) => {
					Part partUnderCursor = GetPartUnderCursor();
					var selectors = EditorLogic.SortedShipList;
				
					//EditorLogic.fetch.Lock (true, true, true, "SelectRoot2");

					var selectorUnderCursor = selectors.Find(x => (Part)x == partUnderCursor);
					if(selectorUnderCursor) 
						Refl.Invoke(selectorUnderCursor, "OnMouseEnter");
				};
				st_root_select.OnEnter += fixAlreadyHoveringPartFn;
				OnCleanup += () => {
					st_root_select.OnEnter -= fixAlreadyHoveringPartFn;
				};

				// Provide a more meaningful message after our changes:
				KFSMStateChange postNewMessageFn = (from) => {
					var template = (ScreenMessage)Refl.GetValue(EditorLogic.fetch, "modeMsg");
					ScreenMessages.PostScreenMessage("Select a new root part", template);
				};
				st_root_select.OnEnter += postNewMessageFn;
				OnCleanup += () => {
					st_root_select.OnEnter -= postNewMessageFn;
				};
			}
#if true
			
			// Drop the new root part after selection:
			{
				KFSMEvent dropNewRootPartEvent = new KFSMEvent("SelectRoot2_dropNewRootPartEvent");
				dropNewRootPartEvent.OnCheckCondition = (state) => {
					return EditorLogic.fetch.lastEventName.Equals("on_rootSelect");
				};
				dropNewRootPartEvent.OnEvent = () => {
					// Normally the backup is triggered in on_partDropped#OnCheckCondition()
					// But we skip that, so do backup here instead.

					EditorLogic.fetch.SetBackup();
					// Normally triggered in on_partDropped#OnEvent().
					GameEvents.onEditorPartEvent.Fire(ConstructionEventType.PartDropped, EditorLogic.SelectedPart);
					var template = (ScreenMessage)Refl.GetValue(EditorLogic.fetch, "modeMsg");
					//ScreenMessages.PostScreenMessage(String.Empty, template);
					if (template != null)
						ScreenMessages.PostScreenMessage("New Root selected and dropped", template);
					else
						ScreenMessages.PostScreenMessage("New Root selected and dropped");

					EditorLogic.SelectedPart.gameObject.SetLayerRecursive(0, 1 << 21);
#if false
					
					Part[] parts = EditorLogic.RootPart.GetComponentsInChildren<Part>();
					foreach (var p in parts)
					{
						Log.Info(p.ToString() + "p.enabled: " + p.enabled.ToString());

						//Log.Info(p.ToString() + "p.frozen: " + p.active .ToString());
					}
#endif
					//EditorLogic.fetch.Unlock("SelectRoot2");
				};

				// problem is with the following line
				KFSMState st_idle = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_idle");
				dropNewRootPartEvent.GoToStateOnEvent = st_idle;

				KFSMState st_place = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_place");
				InjectEvent(st_place, dropNewRootPartEvent);

			}
#endif
			Log.Info("Setup complete..");
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
				Log.Info("Removed event " + injectedEvent.name + " from state " +  state.name);
			};
			Log.Info("Injected event " + injectedEvent.name + " into state " + state.name);
		}

		public void OnDestroy() {
			Log.Info("OnDestroy");
			if(OnCleanup != null) OnCleanup();
			Log.Info("Cleanup complete.");
		}
	}

	#region Utils

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
	#endregion
}
#endif