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
	[KSPAddon(KSPAddon.Startup.EditorAny, true)]
	class GizmoEvents : MonoBehaviour
	{
		public static readonly EventData<EditorGizmos.GizmoRotate> onRotateGizmoSpawned = new EventData<EditorGizmos.GizmoRotate>("onRotateGizmoSpawned");
		public static readonly EventData<EditorGizmos.GizmoOffset> onOffsetGizmoSpawned = new EventData<EditorGizmos.GizmoOffset>("onOffsetGizmoSpawned");
		public static bool rotateGizmoActive = false;
		public static bool offsetGizmoActive = false;
		public static EditorGizmos.GizmoOffset[] gizmosOffset = null;
		public static EditorGizmos.GizmoOffsetHandle gizmoOffsetHandle = null;
		public static EditorGizmos.GizmoRotate[] gizmosRotate = null;
		public static EditorGizmos.GizmoRotateHandle gizmoRotateHandle = null;

		class GizmoCreationListener : MonoBehaviour
		{
			private void Start()
			// I use Start instead of Awake because whatever setup the editor does to the gizmo won't be done yet
			{
				EditorGizmos.GizmoRotate rotate = null;
				EditorGizmos.GizmoOffset offset = null;

				if (gameObject.GetComponentCached(ref rotate) != null)
				{
					onRotateGizmoSpawned.Fire(rotate);
				}
				else if (gameObject.GetComponentCached(ref offset) != null)
				{
					onOffsetGizmoSpawned.Fire(offset);
				}
				else Log.Debug("Didn't find a gizmo on this GameObject -- something has broken");

				// could destroy this MB now, unless you wanted to use OnDestroy to sent an event
			}

			private void OnDestroy()
			{
				// could also send an event on despawn here
			}
		}


		private void Awake()
		{
			AddListenerToGizmo("RotateGizmo");
			AddListenerToGizmo("OffsetGizmo");

			Destroy(gameObject);
		}


		private static void AddListenerToGizmo(string prefabName)
		{
			var prefab = AssetBase.GetPrefab(prefabName);

			if (prefab == null)
			{
				Log.Error("Couldn't find gizmo '" + prefabName + "'");
				return;
			}

			prefab.AddOrGetComponent<GizmoCreationListener>();

			#if DEBUG
			Log.Info("Added listener to " + prefabName);
			#endif
		}



		private void RotateGizmoSpawned(EditorGizmos.GizmoRotate data)
		{
			Log.Info("Rotate gizmo was spawned 1");
		}


		private void OffsetGizmoSpawned(EditorGizmos.GizmoOffset data)
		{
			Log.Info("Offset gizmo was spawned 1");
		}



	}

	#if true
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	class TestGizmoEvents : MonoBehaviour
	{
		private void Start()
		{
			GizmoEvents.onRotateGizmoSpawned.Add(RotateGizmoSpawned);
			GizmoEvents.onOffsetGizmoSpawned.Add(OffsetGizmoSpawned);
		}


		private void OnDestroy()
		{
			GizmoEvents.onRotateGizmoSpawned.Remove(RotateGizmoSpawned);
			GizmoEvents.onOffsetGizmoSpawned.Remove(OffsetGizmoSpawned);
		}


		private void RotateGizmoSpawned(EditorGizmos.GizmoRotate data)
		{
			GizmoEvents.rotateGizmoActive = true;
			GizmoEvents.offsetGizmoActive = false;
			GizmoEvents.gizmosRotate = HighLogic.FindObjectsOfType<EditorGizmos.GizmoRotate> ();
			GizmoEvents.gizmoRotateHandle = HighLogic.FindObjectOfType<EditorGizmos.GizmoRotateHandle> ();
			Log.Info("Rotate gizmo was spawned 2");
		}


		private void OffsetGizmoSpawned(EditorGizmos.GizmoOffset data)
		{
			GizmoEvents.rotateGizmoActive = false;
			GizmoEvents.offsetGizmoActive = true;
			GizmoEvents.gizmosOffset = HighLogic.FindObjectsOfType<EditorGizmos.GizmoOffset> ();
			GizmoEvents.gizmoOffsetHandle = HighLogic.FindObjectOfType<EditorGizmos.GizmoOffsetHandle> ();

            if (EditorLogic.SelectedPart != null)
            {
                
                Space sp = GizmoEvents.gizmosOffset[0].CoordSpace;
                Log.Info("gizmoOffset == null, EditorLogic.SelectedPart: " + EditorLogic.SelectedPart.partInfo.title);
                Log.Info("coordSpace: " + sp.ToString());

                if (GizmoEvents.gizmosOffset[0].CoordSpace == Space.Self)
                {
                    GizmoEvents.gizmosOffset[0].transform.rotation = EditorLogic.SelectedPart.transform.rotation;
                }
                else
                {
                    GizmoEvents.gizmosOffset[0].transform.rotation = Quaternion.identity;
                }

            }

            Log.Info("Offset gizmo was spawned 2");
		}
	}
	#endif
}

