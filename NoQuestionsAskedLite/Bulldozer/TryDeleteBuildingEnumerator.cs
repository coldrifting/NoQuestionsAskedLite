using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NoQuestionsAskedLite
{
    public class TryDeleteBuildingEnumerator : IEnumerator, IDisposable, IEnumerator<object>
    {
        internal object current;

        private BulldozeToolExtender _bulldozeTool;
        private ushort _buildingId;

        private bool _nextResult;

        public TryDeleteBuildingEnumerator(ushort buildingId)
        {
            _bulldozeTool = new BulldozeToolExtender();
            _buildingId = buildingId;
            _nextResult = true;
        }

        object IEnumerator<object>.Current
        {
            get { return this.current; }
        }

        object IEnumerator.Current
        {
            get { return this.current; }
        }

        public bool MoveNext()
        {
            if (_nextResult)
            {
                _nextResult = false;
                current = 0;
                DoDelete();

                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        private void DoDelete()
        {
            // Check if confirmation is required. This is only the case for non-automatic building types and if
            // configured in the options of this mod.
            if (RequireConfirmation())
                DeleteWithConfirmation();
            else
                _bulldozeTool.DeleteBuildingImpl(_buildingId);
        }

        private bool RequireConfirmation()
        {
            var info = Singleton<BuildingManager>.instance.m_buildings.m_buffer[_buildingId].Info;
            if (info.m_placementStyle == ItemClass.Placement.Automatic) return false;

            if (Event.current == null) return false;

            bool confirmRequired = false;

            return confirmRequired;
        }

        private void DeleteWithConfirmation()
        {
            ConfirmPanel.ShowModal(LocaleID.CONFIRM_BUILDINGDELETE, delegate (UIComponent comp, int ret)
            {
                if (ret == 1)
                    // Invoke the default DeleteBuilding method here instead of the tweaked DeleteBuildingImpl, as the custom behavior is
                    // not required here
                    _bulldozeTool.DeleteBuilding(_buildingId);
            });
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _nextResult = false;
                if (_bulldozeTool != null)
                    _bulldozeTool = null;
            }
        }
    }
}
