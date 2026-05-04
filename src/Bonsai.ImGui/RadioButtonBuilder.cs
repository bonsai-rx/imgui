using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a radio button control and generates
/// a sequence of notifications whenever the button is clicked.
/// </summary>
[Description("Draws a radio button control and generates a sequence of notifications whenever the button is clicked.")]
public class RadioButtonBuilder : TextControlBuilder<int>
{
    /// <summary>
    /// Gets or sets the value that identifies this button within its radio button group.
    /// </summary>
    /// <remarks>
    /// This value is emitted when the button is clicked.
    /// </remarks>
    [Description("The value that identifies this button within its radio button group.")]
    public int ButtonValue { get; set; }

    /// <summary>
    /// Gets or sets the value of the current group selection.
    /// </summary>
    /// <remarks>
    /// The radio button is highlighted when this value equals
    /// <see cref="ButtonValue"/>. Radio buttons form a group when the
    /// value of this property is kept in sync across multiple instances.
    /// </remarks>
    [Description("The value of the current group selection.")]
    public int SelectedValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<int> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<int>(observer =>
        {
            var label = $"{Text}##{Name ?? nameof(ImGui.RadioButton)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    var selected = SelectedValue;
                    if (Visible && ImGui.RadioButton(label, ref selected, ButtonValue))
                    {
                        SelectedValue = selected;
                        observer.OnNext(selected);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
