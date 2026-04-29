using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;

namespace Bonsai.ImGui.Visualizers;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that draws a stairstep plot for each named series in the sequence
/// and emits notifications whenever the plot is displayed.
/// </summary>
[Description("Draws a stairstep plot for each named series in the sequence and emits notifications whenever the plot is displayed.")]
public class PlotStairsSeriesBuilder : PlotSeriesBuilder
{
    /// <summary>
    /// Gets or sets stairstep plot display styling and configuration flags.
    /// </summary>
    [Description("Stairstep plot display styling and configuration flags.")]
    public ImPlotStairsFlags Flags { get; set; }

    /// <summary>
    /// Draws a stairstep plot on the current canvas for each named series in the observable sequence,
    /// and emits notifications whenever the plot is displayed.
    /// </summary>
    /// <inheritdoc/>
    public unsafe override IObservable<TSource> Generate<TSource>(IObservable<TSource> source)
    {
        return Generate(source, (label, getter, count) =>
        {
            ImPlot.PlotStairsG(label, getter, null, count, Flags);
        });
    }
}
