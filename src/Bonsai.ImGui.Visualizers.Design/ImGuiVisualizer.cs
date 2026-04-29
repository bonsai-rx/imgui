using Bonsai;
using Bonsai.Design;
using Bonsai.ImGui.Design;
using Bonsai.ImGui.Visualizers;
using System.Collections.Generic;
using System.Linq;

[assembly: TypeVisualizer(typeof(ImGuiVisualizer), Target = typeof(MashupSource<ImGuiMashupVisualizer, ImGuiVisualizer>))]

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Provides backend infrastructure for displaying the contents of a graphical
/// user interface constructed using Dear ImGui.
/// </summary>
public class ImGuiVisualizer : ImGuiMashupVisualizer
{
    /// <inheritdoc/>
    protected override IEnumerable<IExtensionFactory> GetExtensions() => Enumerable.Empty<IExtensionFactory>();
}
