using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws an RGBA color picker control and generates
/// a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws an RGBA color picker control and generates a sequence of notifications whenever the current value changes.")]
public class ColorPicker4Builder : ColorControlBuilderBase<Vector4>
{
    /// <summary>
    /// Gets or sets the initial color value.
    /// </summary>
    [TypeConverter(typeof(NumericRecordConverter))]
    [Description("The initial color value.")]
    public Vector4 InitialValue { get; set; } = Vector4.UnitW;

    /// <inheritdoc/>
    protected override IObservable<Vector4> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<Vector4>(observer =>
        {
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.ColorPicker4)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.ColorPicker4(label, ref value, Flags))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
