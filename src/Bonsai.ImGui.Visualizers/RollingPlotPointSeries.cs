using Bonsai.ImPlot;
using Hexa.NET.ImPlot;
using System;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Provides an adapter over a rolling buffer used for providing data for
/// interactive visualizations.
/// </summary>
/// <typeparam name="TSource">The type of the elements stored in the rolling buffer.</typeparam>
public class RollingPlotPointSeries<TSource> : IPlotPointGetter, IPlotPointGetterSeries
{
    private readonly RollingBuffer<TSource> storage;
    private readonly NamedPlotPointGetter[] series;

    internal RollingPlotPointSeries(RollingBuffer<TSource> buffer, NamedPlotPointGetter[] getters)
    {
        storage = buffer;
        series = getters;
    }

    /// <inheritdoc/>
    public int Count => storage.Count;

    /// <inheritdoc/>
    public ReadOnlySpan<NamedPlotPointGetter> Series => series;

    ImPlotPointGetter IPlotPointGetter.Getter => series[0].Getter;
}
