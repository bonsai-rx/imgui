using Hexa.NET.ImPlot;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Represents a single named getter function used to sample plot point values.
/// </summary>
/// <param name="Name">The label used to name the sampled attribute in plots.</param>
/// <param name="Getter">The delegate used to sample plot point values.</param>
public record struct NamedPlotPointGetter(string Name, ImPlotPointGetter Getter)
{
}
