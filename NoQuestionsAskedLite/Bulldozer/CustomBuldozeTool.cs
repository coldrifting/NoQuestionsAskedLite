using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace NoQuestionsAskedLite
{
    public class CustomBuldozeTool : DefaultTool
    {
        public IEnumerator TryDeleteBuilding(ushort building)
        {
            // The original TryDeleteBuilding invocation has been redirected to this method using the Detour library. This method should return
            // an IEnumerator implementation for doing the actual work.
            return new TryDeleteBuildingEnumerator(building);
        }
    }
}
