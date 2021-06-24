using System;
using UnityEngine;
using KSP.UI.Screens; // has EditorPartList

namespace Rememberer
{

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class RememEditor : MonoBehaviour
    {
        private static bool sortAsc = true;
        private static int sortIndex = 1;
        private static ConfigNode nodeFile = null;
        private static bool disabled = false;

        // capture part list sort method change
        private void SortCB(int index, bool asc)
        {
            sortIndex = index;
            sortAsc = asc;
        }

        const string FILE = "GameData/EditorExtensionsRedux/PluginData/RememEditor.cfg";
        const string SORTASC_NAME = "partListSortAsc";
        const string SORTINDEX_NAME = "partListSortIndex";

        static public bool hasMod(string modIdent)
        {
            foreach (AssemblyLoader.LoadedAssembly a in AssemblyLoader.loadedAssemblies)
            {
                if (modIdent == a.name)
                    return true;
            }
            return false;
        }



        public void Start()
        {
            if (disabled)
                return;
            if (nodeFile == null)
            {
                if (hasMod("PRUNE"))
                {
                    EditorExtensionsRedux.Log.Info("Rememberer.Start, PRUNE found");

                    disabled = true;
                    return;
                }

                // Imports initial sort settings from config file into a default "root" Config Node
                nodeFile = ConfigNode.Load(KSPUtil.ApplicationRootPath + FILE);
                if (nodeFile != null)
                {
                    sortAsc = Convert.ToBoolean(nodeFile.GetValue(SORTASC_NAME));  // true: ascending, false: descending
                    sortIndex = Convert.ToInt32(nodeFile.GetValue(SORTINDEX_NAME));  // 0: mame, 1: mass, 2: cost, 3: size
                }
            }
                // set initial sort method
                EditorPartList.Instance.partListSorter.ClickButton(sortIndex);
                if (!sortAsc)
                {
                    EditorPartList.Instance.partListSorter.ClickButton(sortIndex);
                }
            //Track the user's sort changes
            EditorPartList.Instance.partListSorter.AddOnSortCallback(SortCB);
        }

        public void OnDisable()
        {
            nodeFile = new ConfigNode();

            nodeFile.SetValue(SORTASC_NAME, sortAsc.ToString(), true);
            nodeFile.SetValue(SORTINDEX_NAME, sortIndex.ToString(), true);
            nodeFile.Save(KSPUtil.ApplicationRootPath + FILE);

            //EditorPartList is already disabled when OnDisable is called so remove callback gives NRE
            //EditorPartList.Instance.partListSorter.RemoveOnSortCallback(SortCB);
        }
    }

}
