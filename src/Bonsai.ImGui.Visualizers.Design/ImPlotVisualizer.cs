using Bonsai.ImGui.Design;
using Bonsai.ImPlot;
using System.Collections.Generic;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Displays the contents of a graphical user interface constructed using Dear ImGui and ImPlot.
/// </summary>
public class ImPlotVisualizer : ImGuiMashupVisualizer
{
    /// <inheritdoc/>
    protected override IEnumerable<IExtensionFactory> GetExtensions()
    {
        yield return ImPlotExtension.Factory;
    }
}
