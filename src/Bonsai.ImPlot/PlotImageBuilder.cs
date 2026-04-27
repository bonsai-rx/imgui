using Bonsai.ImGui;
using Hexa.NET.ImGui;
using Hexa.NET.ImPlot;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Bonsai.ImPlot;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Represents an operator that plots an axis-aligned image and generates
/// a sequence of notifications whenever the image is visible.
/// </summary>
[ResetCombinator]
[Description("Plots an axis-aligned image and generates a sequence of notifications whenever the image is visible.")]
public class PlotImageBuilder : ControlBuilder
{
    /// <summary>
    /// Gets or sets the reference to the texture where the image is stored.
    /// </summary>
    [XmlIgnore]
    [Description("The reference to the texture where the image is stored.")]
    public ImTextureRef Image { get; set; }

    /// <summary>
    /// Gets or sets the minimum bounds in plot coordinates at which to plot the image.
    /// </summary>
    [TypeConverter(typeof(NumericRecordConverter))]
    [Description("The minimum bounds in plot coordinates at which to plot the image.")]
    public ImPlotPoint BoundsMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum bounds in plot coordinates at which to plot the image.
    /// </summary>
    [TypeConverter(typeof(NumericRecordConverter))]
    [Description("The maximum bounds in plot coordinates at which to plot the image.")]
    public ImPlotPoint BoundsMax { get; set; }

    /// <inheritdoc/>
    protected unsafe override IObservable<TSource> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<TSource>(observer =>
        {
            var label = $"##{Name ?? nameof(ImPlot.PlotImage)}";
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    if (Visible)
                    {
                        ImPlot.PlotImage(label, Image, BoundsMin, BoundsMax);
                        observer.OnNext(value);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
