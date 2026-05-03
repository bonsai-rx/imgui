using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws an RGB color picker control and generates
/// a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws an RGB color picker control and generates a sequence of notifications whenever the current value changes.")]
public class ColorPicker3Builder : ColorControlBuilderBase<Vector3>
{
    /// <summary>
    /// Gets or sets the initial color value.
    /// </summary>
    [TypeConverter(typeof(NumericRecordConverter))]
    [Description("The initial color value.")]
    public Vector3 InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<Vector3> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<Vector3>(observer =>
        {
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.ColorPicker3)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.ColorPicker3(label, ref value, Flags))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
