namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Provides an adapter over a circular buffer used for providing data for
/// interactive visualizations.
/// </summary>
/// <typeparam name="TSource">The type of the elements stored in the circular buffer.</typeparam>
public class CircularPlotPointSeries<TSource> : RollingPlotPointSeries<TSource>
{
    internal CircularPlotPointSeries(CircularBuffer<TSource> buffer, NamedPlotPointGetter[] getters)
        : base(buffer, getters)
    {
    }

    /// <summary>
    /// Gets the index of the end of the buffer.
    /// </summary>
    public int End => ((CircularBuffer<TSource>)storage).End;
}
