using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that plots text on a 2D plot and generates
/// a sequence of notifications whenever the element is displayed.
/// </summary>
[Combinator]
[Description("Plots text on a 2D plot and generates a sequence of notifications whenever the element is displayed.")]
public class PlotText
{
    /// <summary>
    /// Gets or sets a value specifying whether the plot element is displayed.
    /// </summary>
    [Category(nameof(CategoryAttribute.Behavior))]
    [Description("Specifies whether the plot element is displayed.")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the text to be included in the plot.
    /// </summary>
    [Category(nameof(CategoryAttribute.Appearance))]
    [Description("The text to be included in the plot.")]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the X position in plot coordinates of the text box.
    /// </summary>
    [Description("The X position in plot coordinates of the text box.")]
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the Y position in plot coordinates of the text box.
    /// </summary>
    [Description("The Y position in plot coordinates of the text box.")]
    public double Y { get; set; }

    /// <summary>
    /// Gets or sets the size of the image.
    /// </summary>
    [TypeConverter(typeof(NumericRecordConverter))]
    [Description("The size of the image.")]
    public Vector2 PixelOffset { get; set; }

    /// <summary>
    /// Gets or sets text box display styling configuration flags.
    /// </summary>
    [Description("Text box display styling configuration flags.")]
    public ImPlotTextFlags Flags { get; set; }

    /// <summary>
    /// Plots text on a 2D plot coordinates using the specified configuration properties and emits
    /// notifications whenever the element is displayed.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
    /// <param name="source">An observable sequence of notifications used to plot the text.</param>
    /// <returns>
    /// An observable sequence of drawing notifications. Notifications may be
    /// omitted if the plot is not visible or disabled.
    /// </returns>
    public IObservable<string> Process<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    if (Visible)
                    {
                        var text = Text;
                        ImPlot.PlotText(text, X, Y, PixelOffset, Flags);
                        observer.OnNext(text);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
