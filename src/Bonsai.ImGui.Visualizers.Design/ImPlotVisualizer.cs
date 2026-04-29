using Bonsai;
using Bonsai.Design;
using Bonsai.ImGui.Design;
using Bonsai.ImGui.Visualizers;
using Bonsai.ImPlot;
using System.Collections.Generic;

[assembly: TypeVisualizer(typeof(ImPlotVisualizer), Target = typeof(MashupSource<ImGuiMashupVisualizer, ImPlotVisualizer>))]

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Provides backend infrastructure for displaying the contents of a graphical
/// user interface using ImPlot.
/// </summary>
public class ImPlotVisualizer : ImGuiMashupVisualizer
{
    /// <inheritdoc/>
    protected override IEnumerable<IExtensionFactory> GetExtensions()
    {
        yield return ImPlotExtension.Factory;
    }
}
