using System.ComponentModel;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Represents an operator that creates a circular buffer object which can be used for
/// drawing one or multiple series, overwriting the oldest samples in place.
/// </summary>
[Description("Creates a circular buffer object which can be used for drawing one or multiple series, overwriting the oldest samples in place.")]
public class CircularBufferBuilder : RollingBufferBuilder
{
    private protected override RollingBuffer<TSource> CreateBuffer<TSource>(int capacity)
    {
        return new CircularBuffer<TSource>(capacity);
    }

    private protected override RollingPlotPointSeries<TSource> CreateSeries<TSource>(
        RollingBuffer<TSource> buffer,
        NamedPlotPointGetter[] getters)
    {
        return new CircularPlotPointSeries<TSource>((CircularBuffer<TSource>)buffer, getters);
    }
}
