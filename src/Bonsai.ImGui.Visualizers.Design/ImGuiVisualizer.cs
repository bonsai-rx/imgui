using Bonsai.ImGui.Design;
using System.Collections.Generic;
using System.Linq;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Displays the contents of a graphical user interface constructed using Dear ImGui.
/// </summary>
public class ImGuiVisualizer : ImGuiMashupVisualizer
{
    /// <inheritdoc/>
    protected override IEnumerable<IExtensionFactory> GetExtensions() => Enumerable.Empty<IExtensionFactory>();
}
