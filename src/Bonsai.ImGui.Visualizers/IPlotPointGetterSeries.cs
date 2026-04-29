using System;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Provides an interface for objects supporting sampling multiple plot point series for visualization.
/// </summary>
public interface IPlotPointGetterSeries
{
    /// <summary>
    /// Gets a collection of named delegates which can be used to sample plot points from the object.
    /// </summary>
    ReadOnlySpan<NamedPlotPointGetter> Series { get; }

    /// <summary>
    /// Gets the number of points available for sampling.
    /// </summary>
    int Count { get; }
}
