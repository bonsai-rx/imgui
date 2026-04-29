using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;

namespace Bonsai.ImPlot;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that plots a collection of bars on a 2D plot and generates
/// a sequence of notifications whenever the plot is displayed.
/// </summary>
[Description("Plots a collection of bars on a 2D plot and generates a sequence of notifications whenever the plot is displayed.")]
public class PlotBars : PlotCombinator
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
    /// Plots a collection of bars on a 2D plot using the plot point getter object in the observable sequence,
    /// and emits notifications whenever the plot is displayed.
    /// </summary>
    /// <inheritdoc/>
    public unsafe IObservable<TSource> Process<TSource>(IObservable<TSource> source) where TSource : IPlotPointGetter
    {
        return Process(source, (label, value) =>
        {
            ImPlot.PlotBarsG(label, value.Getter, null, value.Count, BarSize, Flags);
        });
    }
}
