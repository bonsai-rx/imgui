using Bonsai.ImGui.Design;
using System.ComponentModel;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Represents an operator that configures an immediate mode visualizer
/// using Dear ImGui.
/// </summary>
[TypeVisualizer(typeof(ImGuiVisualizer))]
[Description("Configures an immediate mode visualizer using Dear ImGui.")]
public class ImGuiVisualizerBuilder : ImGuiMashupVisualizerBuilder
{
}
