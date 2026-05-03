using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws an integer drag control and generates
/// a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws an integer drag control and generates a sequence of notifications whenever the current value changes.")]
public class DragIntBuilder : DragIntBase<int>
{
    /// <summary>
    /// Gets or sets the initial value of the drag control.
    /// </summary>
    [Description("The initial value of the drag control.")]
    public int InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<int> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<int>(observer =>
        {
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.DragInt)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.DragInt(label, ref value, Speed, Min, Max, Format, Flags))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
