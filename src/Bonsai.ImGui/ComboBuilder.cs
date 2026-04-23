using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a dropdown combo box and generates
/// a sequence of notifications whenever the selected item changes.
/// </summary>
[Description("Draws a dropdown combo box and generates a sequence of notifications whenever the selected item changes.")]
public class ComboBuilder : ControlBuilder<string>
{
    /// <summary>
    /// Gets or sets the available items to display in the combo box.
    /// </summary>
    [Description("The available items to display in the combo box.")]
    public string[] Items { get; set; }

    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var currentItem = 0;
            var items = Items ?? [];
            var label = $"##{Name ?? nameof(ImGui.Combo)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.Combo(label, ref currentItem, items, items.Length))
                        observer.OnNext(items[currentItem]);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
