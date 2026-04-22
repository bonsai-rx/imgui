using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that appends into the next column of the current table.
/// </summary>
/// <remarks>
/// If currently on the last column, this operator will append into the first column
/// of the next row.
/// </remarks>
[Description("Interfaces with a button control and generates a sequence of notifications whenever the button is clicked.")]
public class TableNextColumnBuilder : ControlBuilderBase<Unit>
{
    /// <inheritdoc/>
    protected override IObservable<Unit> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<Unit>(observer =>
        {
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (ImGui.TableNextColumn())
                        observer.OnNext(Unit.Default);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
