#if true
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

using  EditorExtensionsRedux;

namespace EditorExtensionsRedux.SelectRoot2 {
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class SelectRoot2Behaviour : MonoBehaviour {
	//	private Log log;
#if false
		const int SELECTEDPART = 13;
		const int ST_ROOT_SELECT = 77;
		const int ST_ROOT_UNSELECTED = 76;
		const int MODEMSG = 60;
		const int ST_IDLE = 70;
		const int ST_PLACE = 71;
		const int ONMOUSEISOVER = 250;
		const int GET_STATEEVENTS = 0;
#endif
#if false
		//const int SELECTEDPART = 13;
		const int ST_ROOT_SELECT = 80;
		const int ST_ROOT_UNSELECTED = 79;
		const int MODEMSG = 63;
		const int ST_IDLE = 73;
		const int ST_PLACE = 74;
		const int ONMOUSEISOVER = 250;
		const int GET_STATEEVENTS = 0;
#endif
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
			if (!EditorExtensions.validVersion)
				return;
			//log = new Log(this.GetType().Name);
			Log.Info("Start");

			// Oh god, so much dirty reflection. Please don't sue me, Squad :(
			//KerbalFSM editorFSM = (KerbalFSM)Refl.GetValue(EditorLogic.fetch, "\u0001");


			// Skip first click in root selection mode:
			KFSMEvent skipFirstClickEvent = new KFSMEvent("SelectRoot2_skipFirstClickEvent");
			skipFirstClickEvent.OnCheckCondition = (state) => {
				// Um about that always true condition... There is a funny sound glitch
				// if the root part doesn't have any ROOTABLE children so check for that.
				// Thanks Kerbas_ad_astra, http://forum.kerbalspaceprogram.com/threads/43208?p=1948755#post1948755
				return EditorReRootUtil.GetRootCandidates(EditorLogic.RootPart.GetComponentsInChildren<Part>()).Any();
			};


			skipFirstClickEvent.OnEvent = () => {
				//Refl.SetValue(EditorLogic.fetch, "selectedPart", EditorLogic.RootPart); // SelectedPart
				Refl.SetValue(EditorLogic.fetch, EditorExtensions.c.SELECTEDPART, EditorLogic.RootPart); // SelectedPart
			};
			//KFSMState st_root_select = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_root_select");
			KFSMState st_root_select = (KFSMState)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.ST_ROOT_SELECT);
			skipFirstClickEvent.GoToStateOnEvent = st_root_select;

			//KFSMState st_root_unselected = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_root_unselected");
			KFSMState st_root_unselected = (KFSMState)Refl.GetValue (EditorLogic.fetch, EditorExtensions.c.ST_ROOT_UNSELECTED);
			InjectEvent (st_root_unselected, skipFirstClickEvent);

			// Fix ability to select if already hovering:
			KFSMStateChange fixAlreadyHoveringPartFn = (from) => {
				Part partUnderCursor = GetPartUnderCursor();
				var selectors = EditorLogic.SortedShipList;
			
				//EditorLogic.fetch.Lock (true, true, true, "SelectRoot2");

				var selectorUnderCursor = selectors.Find(x => (Part)x == partUnderCursor);
				if(selectorUnderCursor) 
				{
//						Refl.Invoke(selectorUnderCursor, "OnMouseIsOver");
					Refl.Invoke(selectorUnderCursor, EditorExtensions.c.ONMOUSEISOVER);
				}
			};
			st_root_select.OnEnter += fixAlreadyHoveringPartFn;
			OnCleanup += () => {
				Log.Info("SelectRoot OnCleanup 1");
				st_root_select.OnEnter -= fixAlreadyHoveringPartFn;
			};

			// Provide a more meaningful message after our changes:
			KFSMStateChange postNewMessageFn = (from) => {
				//var template = (ScreenMessage)Refl.GetValue(EditorLogic.fetch, "modeMsg");
				var template = (ScreenMessage)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.MODEMSG);
				ScreenMessages.PostScreenMessage("Select a new root part", template);
			};

			st_root_select.OnEnter += postNewMessageFn;
			OnCleanup += () => {
				Log.Info("SelectRoot OnCleanup 2");
				st_root_select.OnEnter -= postNewMessageFn;
			};

			// Drop the new root part after selection:

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
				//var template = (ScreenMessage)Refl.GetValue(EditorLogic.fetch, "modeMsg");
				var template = (ScreenMessage)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.MODEMSG);
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
			//KFSMState st_idle = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_idle");
			KFSMState st_idle = (KFSMState)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.ST_IDLE);
			dropNewRootPartEvent.GoToStateOnEvent = st_idle;

			//KFSMState st_place = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_place");
			KFSMState st_place = (KFSMState)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.ST_PLACE);
			InjectEvent(st_place, dropNewRootPartEvent);
				

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
//				((List<KFSMEvent>)Refl.GetValue(state, "stateEvents")).Remove(injectedEvent);
				Log.Info("SelectRoot OnCleanup 3");
//				List<KFSMState> kfsmstatelist = (List<KFSMState>)Refl.GetValue(state, GET_STATEEVENTS);
#if false
				foreach(var kfsmstate in kfsmstatelist)
				{
					if (kfsmstate == state)
					{
						state.
						kfsmstatelist.Remove(injectedEvent);
						Log.Info("Removed event " + injectedEvent.name + " from state " +  state.name);
						return;
					}
				}
#endif


			};
			Log.Info("Injected event " + injectedEvent.name + " into state " + state.name);
		}

		public void OnDestroy() {
			if (!EditorExtensions.validVersion)
				return;
			Log.Info("SelectRoot OnDestroy");
			if(OnCleanup != null) OnCleanup();
			Log.Info("Cleanup complete.");
		}
	}

}
#endif