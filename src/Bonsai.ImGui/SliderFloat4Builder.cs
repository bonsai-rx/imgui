using OpenTK;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;
using SNVector4 = System.Numerics.Vector4;

/// <summary>
/// Represents an operator that draws a group of four floating-point sliders and
/// generates a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws a group of four floating-point sliders and generates a sequence of notifications whenever the current value changes.")]
public class SliderFloat4Builder : SliderFloatBase<Vector4>
{
    /// <summary>
    /// Gets or sets the initial value of the slider.
    /// </summary>
    [Description("The initial value of the slider.")]
    [TypeConverter(typeof(NumericRecordConverter))]
    public Vector4 InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<Vector4> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<Vector4>(observer =>
        {
            var min = Min;
            var max = Max;
            var initialValue = InitialValue;
            var value = new SNVector4(initialValue.X, initialValue.Y, initialValue.Z, initialValue.W);
            var label = $"##{Name ?? nameof(ImGui.SliderFloat4)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.SliderFloat4(label, ref value, min, max))
                        observer.OnNext(new(value.X, value.Y, value.Z, value.W));
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
