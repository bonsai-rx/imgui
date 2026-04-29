using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;

namespace Bonsai.ImGui.Visualizers;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that plots a collection of bars for each named series in the sequence
/// and emits notifications whenever the plot is displayed.
/// </summary>
[Description("Plots a collection of bars for each named series in the sequence and emits notifications whenever the plot is displayed.")]
public class PlotBarsSeriesBuilder : PlotSeriesBuilder
{
    /// <summary>
    /// Gets or sets the size of each bar, in plot units.
    /// </summary>
    [Description("The size of each bar, in plot units.")]
    public double BarSize { get; set; } = 0.67;

    /// <summary>
    /// Gets or sets bar display styling and configuration flags.
    /// </summary>
    [Description("Bar display styling and configuration flags.")]
    public ImPlotBarsFlags Flags { get; set; }

    /// <summary>
    /// Plots a collection of bars on the current canvas for each named series in the observable sequence,
    /// and emits notifications whenever the plot is displayed.
    /// </summary>
    /// <inheritdoc/>
    public unsafe override IObservable<TSource> Generate<TSource>(IObservable<TSource> source)
    {
        return Generate(source, (label, getter, count) =>
        {
            ImPlot.PlotBarsG(label, getter, null, count, BarSize, Flags);
        });
    }
}
