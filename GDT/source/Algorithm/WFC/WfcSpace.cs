using System;
using System.Collections.Generic;
using System.Linq;
using GDT.Algorithm;

#nullable enable

namespace GDT.Algorithm.WFC
{
    public abstract class WfcSpace<TModule, TPosition>
                                    where TModule: class, IWfcModule<TModule, TPosition>
    {
        public Blackboard Blackboard { get; }

        protected WfcSpace()
        {
            Blackboard = new Blackboard();
        }
        
        protected WfcSpace(Blackboard blackboard)
        {
            Blackboard = blackboard;
        }

        protected abstract IEnumerable<WfcCell<TModule, TPosition>> GetCells();

        protected T RandomSelector<T>(List<T> choices)
        {
            Random random = new Random();
            return choices[random.Next(0, choices.Count)];
        }
        
        public abstract WfcCell<TModule, TPosition>? GetCell(TPosition position);

        /// <summary>
        /// Method that describes which cell will be collapsed next.
        /// The cell with the highest priority will be evaluated (if it has more than one option left)
        /// </summary>
        /// <param name="priorityFunc">The method that returns a priority (highest will be evaluated).</param>
        /// <param name="deciderFunc">The method that decides which element will be evaluated if several elements have the highest priority</param>
        /// <returns>The next cell which will be collapsed</returns>
        public WfcCell<TModule, TPosition> SelectCell(
            Func<WfcCell<TModule, TPosition>, int> priorityFunc, 
            Func<List<WfcCell<TModule, TPosition>>, WfcCell<TModule, TPosition>> deciderFunc)
        {
            int max = GetCells().Where(cell => cell.AvailableModules.Count > 1)
                .Max(cell => priorityFunc.Invoke(cell));
            var bestCells = GetCells().Where(cell => priorityFunc.Invoke(cell) == max).ToList();

            var selectedCell = deciderFunc(bestCells);
            selectedCell.WasSelected = true;
            return selectedCell;
        }
        
        public WfcCell<TModule, TPosition> SelectCell(Func<WfcCell<TModule, TPosition>, int> priorityFunc)
        {
            return SelectCell(priorityFunc, RandomSelector);
        }
        
        /// <summary>
        /// Default WFC cell selection by using the cell with the least remaining possibilities.
        /// </summary>
        /// <returns>One of the cells with the least remaining modules</returns>
        public WfcCell<TModule, TPosition> SelectCell()
        {
            return SelectCell(cell => Int32.MaxValue - cell.AvailableModules.Count, RandomSelector);
        }

        public abstract List<WfcCell<TModule, TPosition>> GetNeighbors(WfcCell<TModule, TPosition> cell);

        public virtual bool GenerationFinished()
        {
            foreach (var cell in GetCells())
            {
                if (cell.AvailableModules.Count > 1) return false;
            }

            return true;
        }

        public bool GenerationSuccessful()
        {
            foreach (var cell in GetCells())
            {
                if (cell.AvailableModules.Count != 1) return false;
            }

            return true;
        }
        
        public void Collapse(WfcCell<TModule, TPosition> cell, 
                             Func<WfcCell<TModule, TPosition>, TModule> moduleSelectionFunc,
                             Action<WfcSpace<TModule, TPosition>, TModule> onModuleSelected)
        {
            TModule selectedModule = moduleSelectionFunc.Invoke(cell);
            cell.AvailableModules.RemoveAll(tile => tile != selectedModule);
            onModuleSelected.Invoke(this, selectedModule);
            
            PropagateResult(cell);
        }
        
        public void Collapse(WfcCell<TModule, TPosition> cell, 
                             Action<WfcSpace<TModule, TPosition>, TModule> onModuleSelected)
        {
            Collapse(cell, wfcCell => RandomSelector(wfcCell.AvailableModules), onModuleSelected);
        }
        
        public void Collapse(WfcCell<TModule, TPosition> cell)
        {
            Collapse(cell, wfcCell => RandomSelector(wfcCell.AvailableModules), (space, module) => {});
        }
        
        protected virtual void PropagateResult(WfcCell<TModule, TPosition> cell)
        {
            Stack<WfcCell<TModule, TPosition>> updateStack = new (GetNeighbors(cell));

            while (updateStack.Any())
            {
                WfcCell<TModule, TPosition> currentCell = updateStack.Pop();
                var wereElementsRemoved = currentCell.RemoveInvalidModules(this);
                if (wereElementsRemoved)
                {
                    GetNeighbors(currentCell).ForEach(wfcCell => updateStack.Push(wfcCell));
                }
            }
        }

        public void PropagateGlobally()
        {
            Stack<WfcCell<TModule, TPosition>> updateStack = new (GetCells());
            while (updateStack.Any())
            {
                WfcCell<TModule, TPosition> currentCell = updateStack.Pop();
                var wereElementsRemoved = currentCell.RemoveInvalidModules(this);
                if (wereElementsRemoved)
                {
                    GetNeighbors(currentCell).ForEach(wfcCell => updateStack.Push(wfcCell));
                }
            }
        }
    }
}