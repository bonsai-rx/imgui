using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;

namespace Bonsai.ImGui.Visualizers;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that draws a digital plot for each named series in the sequence
/// and emits notifications whenever the plot is displayed.
/// </summary>
[Description("Draws a digital plot for each named series in the sequence and emits notifications whenever the plot is displayed.")]
public class PlotDigitalSeriesBuilder : PlotSeriesBuilder
{
    /// <summary>
    /// Gets or sets digital plot display styling and configuration flags.
    /// </summary>
    [Description("Digital plot display styling and configuration flags.")]
    public ImPlotDigitalFlags Flags { get; set; }

    /// <summary>
    /// Draws a digital plot on the current canvas for each named series in the observable sequence,
    /// and emits notifications whenever the plot is displayed.
    /// </summary>
    /// <inheritdoc/>
    public unsafe override IObservable<TSource> Generate<TSource>(IObservable<TSource> source)
    {
        return Generate(source, (label, getter, count) =>
        {
            ImPlot.PlotDigitalG(label, getter, null, count, Flags);
        });
    }
}
