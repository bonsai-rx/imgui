using Bonsai.ImGui;
using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImPlot;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that begins drawing a plot and provides
/// a sequence of notifications for drawing plot contents.
/// </summary>
[Description("Begins drawing a plot and provides a sequence of notifications for drawing plot contents.")]
public class PlotBuilder : ControlBuilder<string>
{
    /// <summary>
    /// Gets or sets the size of the plot. Negative values will stretch the corresponding dimension to fill available space.
    /// </summary>
    [TypeConverter(typeof(NumericRecordConverter))]
    [Description("The size of the plot. Negative values will stretch the corresponding dimension to fill available space.")]
    public Vector2 Size { get; set; }

    /// <summary>
    /// Gets or sets plot configuration and styling flags.
    /// </summary>
    [Description("Plot configuration and styling flags.")]
    public ImPlotFlags Flags { get; set; }

    /// <summary>
    /// Gets or sets styling and configuration flags for the X-axis.
    /// </summary>
    [Description("Styling and configuration flags for the X-axis.")]
    public ImPlotAxisFlags FlagsX { get; set; }

    /// <summary>
    /// Gets or sets styling and configuration flags for the Y-axis.
    /// </summary>
    [Description("Styling and configuration flags for the Y-axis.")]
    public ImPlotAxisFlags FlagsY { get; set; }

    /// <summary>
    /// Gets or sets the label to display on the X-axis.
    /// </summary>
    [Description("The label to display on the X-axis.")]
    public string LabelX { get; set; }

    /// <summary>
    /// Gets or sets the label to display on the Y-axis.
    /// </summary>
    [Description("The label to display on the Y-axis.")]
    public string LabelY { get; set; }

    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var label = $"##{Name ?? "Plot"}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImPlot.BeginPlot(label, Size, Flags))
                    {
                        ImPlot.SetupAxes(LabelX ?? string.Empty, LabelY ?? string.Empty, FlagsX, FlagsY);
                        observer.OnNext(label);
                        ImPlot.EndPlot();
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
