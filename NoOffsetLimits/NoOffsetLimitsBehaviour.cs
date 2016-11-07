using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorGizmos;
using UnityEngine;
using System.Reflection;
using EditorExtensionsRedux;

// DO NOT ENABLE THIS, OLD CODE
#if false
namespace EditorExtensionsRedux.NoOffsetBehaviour
{
#if false
    /**
	 * Removes limits from offset gizmo. Limits are enforced in EditorLogic::onOffsetGizmoUpdate. This recreates the gizmo after the 
	 * st_offset_tweak state is entered and reimplements the offset logic without limits.
	 */
    public class Constantsz
    {

        // Following for NoOffsetLimits
        public int ST_OFFSET_TWEAK = 73;
        public int SYMUPDATEATTACHNODE = 108;
        public int GIZMOOFFSET = 66;
        public int GIZMOROTATE = 67;

        public int UPDATESYMMETRY = 64;
        public int ONOFFSETGIZMOUPDATED = 35;



        public bool Init()
        {
            if (Versioning.version_major == 1 && Versioning.version_minor == 1 && Versioning.Revision == 0 /*&& Versioning.BuildID == 1024 */)
            {


                // NoOffsetLimits
                ST_OFFSET_TWEAK = 73;
                SYMUPDATEATTACHNODE = 108;
                GIZMOOFFSET = 66;

                UPDATESYMMETRY = 64;
                ONOFFSETGIZMOUPDATED = 35;

                return true;
            }
            if (Versioning.version_major == 1 && Versioning.version_minor == 1 && (Versioning.Revision == 1 || Versioning.Revision == 2) /*&& Versioning.BuildID == 1024 */)
            {


                // NoOffsetLimits
                ST_OFFSET_TWEAK = 76;
                SYMUPDATEATTACHNODE = 111;
                GIZMOOFFSET = 69;

                return true;
            }
            if (Versioning.version_major == 1 && Versioning.version_minor == 1 && Versioning.Revision == 3 /*&& Versioning.BuildID == 1024 */)
            {

                // NoOffsetLimits
                ST_OFFSET_TWEAK = 76;
                SYMUPDATEATTACHNODE = 111;
                GIZMOOFFSET = 69;

                UPDATESYMMETRY = 62;
                ONOFFSETGIZMOUPDATED = 35;


                return true;
            }
            if (Versioning.version_major == 1 && Versioning.version_minor == 2 && Versioning.Revision == 0)
            {

                // NoOffsetLimits
                ST_OFFSET_TWEAK = 75;
                SYMUPDATEATTACHNODE = 110;
                GIZMOOFFSET = 68;

                UPDATESYMMETRY = 61;
                ONOFFSETGIZMOUPDATED = 35;

                return true;
            }
            return false;
        }
    }
#endif

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class FreeOffsetBehaviour : MonoBehaviour
    {
        //private Log log;

        //public static Constants c = new Constants();

        public static FreeOffsetBehaviour Instance = null;

        private delegate void CleanupFn();
        private CleanupFn OnCleanup;

        private GizmoOffset gizmoOffset;
        private GizmoRotate gizmoRotate;

        public void Start()
        {
            Instance = this;

            //log = new Log(this.GetType().Name);
            Log.Debug("Start");

            //			var st_offset_tweak = (KFSMState)Refl.GetValue(EditorLogic.fetch, "st_offset_tweak");
            var st_offset_tweak = (KFSMState)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.ST_OFFSET_TWEAK);

            KFSMStateChange hookOffsetUpdateFn = (from) =>
            {

                var p = EditorLogic.SelectedPart;
                var parent = p.parent;
                var symCount = p.symmetryCounterparts.Count;
                //var attachNode = Refl.GetValue(EditorLogic.fetch, "\u001B\u0002");
                //var attachNode = Refl.GetValue(EditorLogic.fetch, "symUpdateAttachNode");
                var symUpdateAttachNode = Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.SYMUPDATEATTACHNODE);

                //public static GizmoOffset Attach(Transform host, Quaternion rotOffset, Callback<Vector3> onMove, Callback<Vector3> onMoved, Camera refCamera = null);

                // gizmoRotate = GizmoRotate.Attach(
                //    EditorLogic.SelectedPart.transform, 
                //                    this.gizmoPivot, 
                //new Callback<Quaternion>(this.onRotateGizmoUpdate), 
                //new Callback<Quaternion>(this.onRotateGizmoUpdated), 
                //this.editorCamera);

                gizmoRotate = (GizmoRotate)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.GIZMOROTATE);

                Quaternion rotOffset = p.attRotation;

                //==============================================================================
                gizmoOffset = GizmoOffset.Attach(EditorLogic.SelectedPart.transform,
                    rotOffset,
                //===
                    new Callback<Vector3>((offset) =>
                    {
                        Space coordSpace = gizmoOffset.CoordSpace;
                        if (coordSpace == Space.Self)
                            gizmoOffset.SetCoordSystem(Space.World);
                        else
                            gizmoOffset.SetCoordSystem(Space.Self);

                        p.transform.position = gizmoOffset.transform.position;
                        p.attPos = p.transform.localPosition - p.attPos0;

                        Log.Info("symCount: " + symCount.ToString());
                        if (symCount != 0)
                        {
                            //							Refl.Invoke(EditorLogic.fetch, "UpdateSymmetry", p, symCount, parent, symUpdateAttachNode);
                            Refl.Invoke(EditorLogic.fetch, EditorExtensions.c.UPDATESYMMETRY, p, symCount, parent, symUpdateAttachNode);
                        }

                        GameEvents.onEditorPartEvent.Fire(ConstructionEventType.PartOffsetting, EditorLogic.SelectedPart);
                    }),
                    //===   
                    new Callback<Vector3>((offset) =>
                    {
                        //Refl.Invoke(EditorLogic.fetch, "onOffsetGizmoUpdated", offset);
                        Refl.Invoke(EditorLogic.fetch, EditorExtensions.c.ONOFFSETGIZMOUPDATED, offset);
                    }),
                    //===
                    EditorLogic.fetch.editorCamera);

                //((GizmoOffset)Refl.GetValue(EditorLogic.fetch, "\u0012")).Detach();
                //Refl.SetValue(EditorLogic.fetch, "\u0012", gizmo);
                //((GizmoOffset)Refl.GetValue(EditorLogic.fetch, "gizmoOffset")).Detach();
                ((GizmoOffset)Refl.GetValue(EditorLogic.fetch, EditorExtensions.c.GIZMOOFFSET)).Detach();
                //Refl.SetValue(EditorLogic.fetch, "gizmoOffset", gizmo);
                Refl.SetValue(EditorLogic.fetch, EditorExtensions.c.GIZMOOFFSET, gizmoOffset);
            };
            st_offset_tweak.OnEnter += hookOffsetUpdateFn;
            OnCleanup += () =>
            {
                st_offset_tweak.OnEnter -= hookOffsetUpdateFn;
            };
            Log.Debug("Installed.");
        }

        public void OnDestroy()
        {
            Log.Debug("FreeOffsetBehaviour.OnDestroy");
            if (OnCleanup != null) OnCleanup();
            Log.Debug("Cleanup complete.");
        }
    }
}
#endif