using Hexa.NET.ImPlot;
using Hexa.NET.Utilities.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Provides an abstract base class for operators that allow plotting a sequence of named series
/// on the current canvas.
/// </summary>
public abstract class PlotSeriesBuilder : ControlBuilderBase
{
    /// <summary>
    /// Builds the expression tree for configuring and rendering the series plot.
    /// </summary>
    /// <inheritdoc/>
    public override Expression Build(IEnumerable<Expression> arguments)
    {
        var source = arguments.First();
        var parameterType = source.Type.GetGenericArguments()[0];
        return Expression.Call(Expression.Constant(this), nameof(Generate), new[] { parameterType }, source);
    }

    internal unsafe delegate void SeriesPlotter(byte* data, ImPlotPointGetter getter, int count);

    internal unsafe IObservable<TSource> Generate<TSource>(IObservable<TSource> source, SeriesPlotter plot) where TSource : IPlotPointGetterSeries
    {
        return Observable.Create<TSource>(observer =>
        {
            var label = Name ?? string.Empty;
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    if (Visible)
                    {
                        var series = value.Series;
                        var labelCapacity = label.Length + 256;
                        var labelBuf = stackalloc byte[labelCapacity];
                        var builder = new StrBuilder(labelBuf, labelCapacity);
                        builder.Append(label);
                        var prefixIndex = builder.Index;

                        for (int i = 0; i < series.Length; i++)
                        {
                            builder.Index = prefixIndex;
                            builder.Append(series[i].Name);
                            builder.End();
                            plot(builder, series[i].Getter, value.Count);
                        }

                        observer.OnNext(value);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }

    /// <summary>
    /// When overridden in a derived class, plots a sequence of named series on the current canvas.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">The sequence of named series to plot on the current canvas.</param>
    /// <returns>The sequence of named series displayed on the current canvas.</returns>
    public abstract IObservable<TSource> Generate<TSource>(IObservable<TSource> source) where TSource : IPlotPointGetterSeries;
}
