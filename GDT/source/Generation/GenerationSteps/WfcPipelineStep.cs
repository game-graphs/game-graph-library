using System;
using System.Collections.Generic;
using GDT.Algorithm.WFC;
using GDT.Algorithm.WFC.Implementation.Graph;
using GDT.Model;
using GDT.Algorithm;

#nullable enable

namespace GDT.Generation.GenerationSteps
{
    public class WfcPipelineStep<TTileData> : IPipelineStep
                                where TTileData: struct
    {
        public string Name { get; } = "Wave Function Collapse Step";
        
        private readonly List<WfcGraphTile<TTileData>> _availableModules;
        private Blackboard _blackboard;
        private readonly bool _requiresGlobalPropagation;
        private readonly Func<WfcCell<WfcGraphTile<TTileData>, int>, int>? _priorityFunc;
        private readonly Func<WfcCell<WfcGraphTile<TTileData>, int>, WfcGraphTile<TTileData>>?  _moduleSelectionFunc;
        private readonly Action<WfcSpace<WfcGraphTile<TTileData>, int>, WfcGraphTile<TTileData>>? _onModuleSelected;
        private readonly string _wfcNeighborLayerName;

        public WfcPipelineStep(
            List<WfcGraphTile<TTileData>> availableModules, 
            Blackboard blackboard, 
            bool requiresGlobalPropagation,
            Func<WfcCell<WfcGraphTile<TTileData>,int>,int>? priorityFunc,
            Func<WfcCell<WfcGraphTile<TTileData>, int>, WfcGraphTile<TTileData>>? moduleSelectionFunc,
            Action<WfcSpace<WfcGraphTile<TTileData>, int>, WfcGraphTile<TTileData>>? onModuleSelected,
            string wfcNeighborLayerName)
        {
            _availableModules = availableModules;
            _blackboard = blackboard;
            _requiresGlobalPropagation = requiresGlobalPropagation;
            _priorityFunc = priorityFunc;
            _moduleSelectionFunc = moduleSelectionFunc;
            _onModuleSelected = onModuleSelected;
            _wfcNeighborLayerName = wfcNeighborLayerName;
        }
        
        public WfcPipelineStep(
            List<WfcGraphTile<TTileData>> availableModules, 
            Blackboard blackboard, 
            bool requiresGlobalPropagation,
            Action<WfcSpace<WfcGraphTile<TTileData>, int>, WfcGraphTile<TTileData>>? onModuleSelected,
            string wfcNeighborLayerName = "WfcNeighborLayer")
        
        :this(availableModules, blackboard, requiresGlobalPropagation, null, null, onModuleSelected, wfcNeighborLayerName)
        {}

        public Graph ExecuteStep(Graph graph)
        {
            Graph graphCopy = graph.Clone();
            Blackboard blackboardCopy = _blackboard.Copy();
            
            WfcGraphSpace<TTileData> wfcGraphSpace = new(graphCopy, _availableModules, blackboardCopy, wfcNeighborLayerName: _wfcNeighborLayerName);

            while (!wfcGraphSpace.GenerationFinished())
            {
                WfcCell<WfcGraphTile<TTileData>, int> selectedCell = SelectCell(wfcGraphSpace);
                
                Collapse(wfcGraphSpace, selectedCell);
                
                if (_requiresGlobalPropagation) wfcGraphSpace.PropagateGlobally();
            }

            if (wfcGraphSpace.GenerationSuccessful())
            {
                _blackboard = blackboardCopy;
                wfcGraphSpace.PropagateToGraphAndCleanup();
                return graphCopy;
            };
            
            return ExecuteStep(graph);
        }

        private void Collapse(WfcGraphSpace<TTileData> wfcGraphSpace, WfcCell<WfcGraphTile<TTileData>, int> selectedCell)
        {
            if (_moduleSelectionFunc != null && _onModuleSelected != null)
            {
                wfcGraphSpace.Collapse(selectedCell, _moduleSelectionFunc, _onModuleSelected);
            }
            else if(_onModuleSelected != null)
            {
                wfcGraphSpace.Collapse(selectedCell, _onModuleSelected);
            }
            else
            {
                wfcGraphSpace.Collapse(selectedCell);
            }

        }

        private WfcCell<WfcGraphTile<TTileData>, int> SelectCell(WfcGraphSpace<TTileData> wfcGraphSpace)
        {
            WfcCell<WfcGraphTile<TTileData>, int> selectedCell;
            if (_priorityFunc != null)
            {
                selectedCell = wfcGraphSpace.SelectCell(_priorityFunc);
            }
            else
            {
                selectedCell = wfcGraphSpace.SelectCell();
            }

            return selectedCell;
        }
    }
}