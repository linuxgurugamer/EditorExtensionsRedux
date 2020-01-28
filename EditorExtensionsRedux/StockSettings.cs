
using System.Collections;
using System.Reflection;

namespace EditorExtensionsRedux
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class EEX : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Editor Extensions"; } }
        public override string DisplaySection { get { return "Editor Extensions"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }



        [GameParameters.CustomFloatParameterUI("Height of bottom of vessel in VAB", minValue = 0, maxValue = 20f, stepCount = 101, displayFormat = "F4",
            toolTip = "In the VAB, height bottom of vessel is moved to when using the 'b' function to adjust the position")]
        public float vabHeight = 5f;

        [GameParameters.CustomFloatParameterUI("Height of bottom of vessel in SPH", minValue = 0, maxValue = 20, stepCount = 101, displayFormat = "F4",
            toolTip = "In the VAB, height bottom of vessel is moved to when using the 'b' function to adjust the position")]
        public float sphHeight = 5f;

  
        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
   
            return true;
        }

        bool unread = false;
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
