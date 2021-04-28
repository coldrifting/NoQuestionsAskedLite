using ICities;
using CitiesHarmony.API;

namespace NoQuestionsAskedLite
{
    public class NoQuestionsAskedLiteInfo : IUserMod
    {
        public string Name => "No Questions Asked Lite";
        public string Description => "Removes the bulldozer confirmation prompt";

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() => NoQuestionsAskedLite.PatchAll());
        }

        public void OnDisabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled) NoQuestionsAskedLite.UnpatchAll();
        }
    }
}
