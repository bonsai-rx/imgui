using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;

namespace Bonsai.ImPlot;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that plots a single line on a 2D plot and generates
/// a sequence of notifications whenever the plot is displayed.
/// </summary>
[Description("Plots a single line on a 2D plot and generates a sequence of notifications whenever the plot is displayed.")]
public class PlotLine : PlotCombinator
{
    /// <summary>
    /// Gets or sets line display styling and configuration flags.
    /// </summary>
    [Description("Line display styling and configuration flags.")]
    public ImPlotLineFlags Flags { get; set; }

    /// <summary>
    /// Plots a single line on a 2D plot using the plot point getter object in the observable sequence,
    /// and emits notifications whenever the plot is displayed.
    /// </summary>
    /// <inheritdoc/>
    public unsafe IObservable<TSource> Process<TSource>(IObservable<TSource> source) where TSource : IPlotPointGetter
    {
        return Process(source, (label, value) =>
        {
            ImPlot.PlotLineG(label, value.Getter, null, value.Count, Flags);
        });
    }
}
