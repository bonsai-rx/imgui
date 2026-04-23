using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that appends into the next column of the current table and
/// generates a sequence of notifications whenever the column is visible.
/// </summary>
/// <remarks>
/// If currently on the last column, this operator will append into the first column
/// of the next row.
/// </remarks>
[Description("Appends into the next column of the current table and generates a sequence of notifications whenever the column is visible.")]
public class TableNextColumnBuilder : ControlBuilder
{
    /// <inheritdoc/>
    protected override IObservable<TSource> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<TSource>(observer =>
        {
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    if (ImGui.TableNextColumn())
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
