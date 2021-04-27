using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace NoQuestionsAskedLite
{
    public class ModLoader : LoadingExtensionBase
    {
        private LoadMode _mode;
        private RedirectCallsState redirectState;

        public override void OnLevelLoaded(LoadMode mode)
        {
            _mode = mode;
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame) return;

            // Is this necessary?
            //base.OnLevelLoaded(mode);
            //if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
            //{
                redirectState = RedirectionHelper.RedirectCalls(
                    typeof(BulldozeTool).GetMethod("TryDeleteBuilding", BindingFlags.Instance | BindingFlags.NonPublic),
                    typeof(CustomBuldozeTool).GetMethod("TryDeleteBuilding", BindingFlags.Instance | BindingFlags.Public));
                Debug.Log("Buildoze tool has been detoured");
            //}
        }

        public override void OnLevelUnloading()
        {
            if (_mode != LoadMode.NewGame && _mode != LoadMode.LoadGame) return;

            // Is this necessary?
            //base.OnLevelUnloading();
            RedirectionHelper.RevertRedirect(redirectState);
        }
    }
}
