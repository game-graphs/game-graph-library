using System;
using System.Collections.Generic;
using GDT.Algorithm;

namespace GDT.Algorithm.WFC
{
    public interface IWfcModule<TModule, TPosition>
                    where TModule: class, IWfcModule<TModule, TPosition>
    {
        public bool CanModuleBePlaced(WfcCell<TModule, TPosition> currentCell, WfcSpace<TModule, TPosition> wfcSpace);
    }
}