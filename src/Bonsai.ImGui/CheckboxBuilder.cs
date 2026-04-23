using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a checkbox control and generates
/// a sequence of notifications whenever the checked status changes.
/// </summary>
[Description("Draws a checkbox control and generates a sequence of notifications whenever the checked status changes.")]
public class CheckboxBuilder : TextControlBuilder<bool>
{
    /// <summary>
    /// Gets or sets the initial checked state of the checkbox.
    /// </summary>
    [Description("The initial checked state of the checkbox.")]
    public bool Checked { get; set; }

    /// <inheritdoc/>
    protected override IObservable<bool> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<bool>(observer =>
        {
            var checkedState = Checked;
            var label = $"{Text}##{Name ?? nameof(ImGui.Checkbox)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.Checkbox(label, ref checkedState))
                        observer.OnNext(checkedState);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
