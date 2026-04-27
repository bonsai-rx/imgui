using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using System.Xml.Serialization;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws an image and generates
/// a sequence of notifications whenever the image is visible.
/// </summary>
[ResetCombinator]
[Description("Draws an image and generates a sequence of notifications whenever the image is visible.")]
public class ImageBuilder : ControlBuilder
{
    /// <summary>
    /// Gets or sets the reference to the texture where the image is stored.
    /// </summary>
    [XmlIgnore]
    [Description("The reference to the texture where the image is stored.")]
    public ImTextureRef Image { get; set; }

    /// <summary>
    /// Gets or sets the size of the image.
    /// </summary>
    [TypeConverter(typeof(NumericRecordConverter))]
    [Description("The size of the image.")]
    public Vector2 ImageSize { get; set; }

    /// <inheritdoc/>
    protected unsafe override IObservable<TSource> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<TSource>(observer =>
        {
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    if (Visible)
                    {
                        ImGui.Image(Image, ImageSize);
                        observer.OnNext(value);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
