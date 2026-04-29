using Hexa.NET.ImPlot;

namespace Bonsai.ImPlot;

/// <summary>
/// Provides an interface for objects supporting sampling of plot points for visualization.
/// </summary>
public interface IPlotPointGetter
{
    /// <summary>
    /// Gets a delegate which can be used to sample plot points from the object.
    /// </summary>
    ImPlotPointGetter Getter { get; }

    /// <summary>
    /// Gets the number of points available for sampling.
    /// </summary>
    int Count { get; }
}
