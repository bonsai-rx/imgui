using Bonsai;
using Bonsai.Design;
using Bonsai.ImGui;
using Bonsai.ImGui.Design;
using Bonsai.ImPlot.Design;
using System.Collections.Generic;

[assembly: TypeVisualizer(typeof(ImPlotVisualizer), Target = typeof(MashupSource<ImGuiVisualizer, ImPlotVisualizer>))]

namespace Bonsai.ImPlot.Design;

/// <summary>
/// Provides backend infrastructure for displaying the contents of a graphical
/// user interface using ImPlot.
/// </summary>
public class ImPlotVisualizer : ImGuiVisualizer
{
    /// <inheritdoc/>
    protected override IEnumerable<IExtensionFactory> GetExtensions()
    {
        yield return ImPlotExtension.Factory;
    }
}
