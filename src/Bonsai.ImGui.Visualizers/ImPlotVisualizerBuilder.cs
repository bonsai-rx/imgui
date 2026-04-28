using Bonsai.ImGui.Design;
using System.ComponentModel;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Represents an operator that configures an immediate mode visualizer
/// backend for ImPlot.
/// </summary>
[TypeVisualizer(typeof(ImPlotVisualizer))]
[Description("Configures an immediate mode visualizer backend for ImPlot.")]
public class ImPlotVisualizerBuilder : ImGuiMashupVisualizerBuilder
{
}
