using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;

namespace Bonsai.ImGui.Visualizers;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that plots a labeled line for each named series in the sequence
/// and emits notifications whenever the plot is displayed.
/// </summary>
[Description("Plots a labeled line for each named series in the sequence and emits notifications whenever the plot is displayed.")]
public class PlotLineSeriesBuilder : PlotSeriesBuilder
{
    /// <summary>
    /// Gets or sets line display styling and configuration flags.
    /// </summary>
    [Description("Line display styling and configuration flags.")]
    public ImPlotLineFlags Flags { get; set; }

    /// <summary>
    /// Plots a labeled line on the current canvas for each named series in the observable sequence,
    /// and emits notifications whenever the plot is displayed.
    /// </summary>
    /// <inheritdoc/>
    public unsafe override IObservable<TSource> Generate<TSource>(IObservable<TSource> source)
    {
        return Generate(source, (label, getter, count) =>
        {
            ImPlot.PlotLineG(label, getter, null, count, Flags);
        });
    }
}
