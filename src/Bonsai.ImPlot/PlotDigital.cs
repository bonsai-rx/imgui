using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;

namespace Bonsai.ImPlot;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that draws a digital plot on the current canvas and generates
/// a sequence of notifications whenever the plot is displayed.
/// </summary>
[Description("Draws a digital plot on the current canvas and generates a sequence of notifications whenever the plot is displayed.")]
public class PlotDigital : PlotCombinator
{
    /// <summary>
    /// Gets or sets digital plot display styling and configuration flags.
    /// </summary>
    [Description("Digital plot display styling and configuration flags.")]
    public ImPlotDigitalFlags Flags { get; set; }

    /// <summary>
    /// Draws a digital plot on the current canvas using the plot point getter object in the
    /// observable sequence, and emits notifications whenever the plot is displayed.
    /// </summary>
    /// <inheritdoc/>
    public unsafe IObservable<TSource> Process<TSource>(IObservable<TSource> source) where TSource : IPlotPointGetter
    {
        return Process(source, (label, value) =>
        {
            ImPlot.PlotDigitalG(label, value.Getter, null, value.Count, Flags);
        });
    }
}
