using System.Collections.Generic;
using GDT.Algorithm;

namespace GDT.Algorithm.WFC
{
    public class WfcCell<TModule, TPosition>
                        where TModule: class, IWfcModule<TModule, TPosition>
    {
        public List<TModule> AvailableModules { get; }
        public TPosition Position { get; }
        public bool WasSelected { get; set; } = false;

        public WfcCell(TPosition position, List<TModule> allModules)
        {
            Position = position;
            AvailableModules = new List<TModule>(allModules);
        }

        /// <summary>
        /// Removes all invalid modules based on the neighbors
        /// </summary>
        /// <param name="wfcSpace">The space this cell is in</param>
        /// <returns>true if any modules were removed</returns>
        public bool RemoveInvalidModules(WfcSpace<TModule, TPosition> wfcSpace)
        {
            if (WasSelected) return false;
                
            var numberOfRemovedModules = AvailableModules.RemoveAll(module => !module.CanModuleBePlaced(this, wfcSpace));
            return numberOfRemovedModules > 0;
        }
    }
}