using Bonsai.Expressions;
using Hexa.NET.ImPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Represents an operator that creates a rolling buffer object which can be used for
/// drawing one or multiple series.
/// </summary>
[Description("Creates a rolling buffer object which can be used for drawing one or multiple series.")]
public class RollingBufferBuilder : SingleArgumentExpressionBuilder
{
    /// <summary>
    /// Gets or sets the capacity of the rolling buffer.
    /// </summary>
    [Description("The capacity of the rolling buffer.")]
    public int Capacity { get; set; }

    /// <summary>
    /// Gets or sets the name of the property that will be used to fill the X-axis data for all series.
    /// </summary>
    [Editor("Bonsai.Design.MemberSelectorEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
    [Description("The name of the property that will be used to fill the X-axis data for all series.")]
    public string IndexSelector { get; set; }

    /// <summary>
    /// Gets or sets the names of the properties that will be used to populate individual series data.
    /// </summary>
    [Editor("Bonsai.ImGui.Visualizers.DataMemberSelectorEditor, Bonsai.ImGui.Visualizers.Design", DesignTypes.UITypeEditor)]
    [Description("The names of the properties that will be used to populate individual series data.")]
    public string ValueSelector { get; set; }

    static LambdaExpression CreateValueGetter(ParameterExpression getterParameter, Type delegateType, string memberPath)
    {
        var member = ExpressionHelper.MemberAccess(getterParameter, memberPath);
        var getterBody = member.Type != typeof(double) ? Expression.Convert(member, typeof(double)) : member;
        return Expression.Lambda(delegateType, getterBody, memberPath, new[] { getterParameter });
    }

    static IEnumerable<LambdaExpression> CreateMemberGetters(ParameterExpression getterParameter, Type delegateType, string selector)
    {
        if (string.IsNullOrEmpty(selector))
        {
            yield return CreateValueGetter(getterParameter, delegateType, string.Empty);
            yield break;
        }

        foreach (var memberName in ExpressionHelper.SelectMemberNames(selector))
            yield return CreateValueGetter(getterParameter, delegateType, memberName);
    }

    static NewArrayExpression CreateMemberGetterArray(Type namedGetterType, ParameterExpression getterParameter, Type delegateType, string selector)
    {
        var memberGetters = CreateMemberGetters(getterParameter, delegateType, selector);
        var constructorInfo = namedGetterType.GetConstructor(namedGetterType.GetGenericArguments());
        return Expression.NewArrayInit(namedGetterType, memberGetters.Select(getter =>
            Expression.New(constructorInfo, Expression.Constant(getter.Name), getter)));
    }

    /// <inheritdoc/>
    public override Expression Build(IEnumerable<Expression> arguments)
    {
        var source = arguments.First();
        var methodName = nameof(Process);
        var parameterType = source.Type.GetGenericArguments()[0];
        if (ExpressionHelper.IsEnumerableType(parameterType))
        {
            parameterType = parameterType.GetGenericArguments()[0];
            methodName = nameof(ProcessCollection);
        }

        var getterType = typeof(ValueGetter<>).MakeGenericType(parameterType);
        var namedGetterType = typeof(KeyValuePair<,>).MakeGenericType(typeof(string), getterType);
        var getterParameter = Expression.Parameter(parameterType.MakeByRefType());
        var combinator = Expression.Constant(this);

        var indexSelector = IndexSelector;
        if (!string.IsNullOrEmpty(indexSelector))
        {
            var getterX = CreateValueGetter(getterParameter, getterType, indexSelector);
            var getterY = CreateMemberGetterArray(namedGetterType, getterParameter, getterType, ValueSelector);
            return Expression.Call(combinator, methodName, new[] { parameterType }, source, getterX, getterY);
        }
        else
        {
            var getterY = CreateMemberGetterArray(namedGetterType, getterParameter, getterType, ValueSelector);
            return Expression.Call(combinator, methodName, new[] { parameterType }, source, getterY);
        }
    }

    static unsafe NamedPlotPointGetter CreatePlotPointGetter<TSource>(
        RollingBuffer<TSource> buffer,
        string name,
        ValueGetter<TSource> getY)
    {
        return new(name, (_, idx, pointPtr) =>
        {
            var point = (ImPlotPoint*)pointPtr;
            point->X = idx;
            point->Y = getY(ref buffer[idx]);
            return pointPtr;
        });
    }

    static unsafe NamedPlotPointGetter CreatePlotPointGetter<TSource>(
        RollingBuffer<TSource> buffer,
        string name,
        ValueGetter<TSource> getX,
        ValueGetter<TSource> getY)
    {
        return new(name, (_, idx, pointPtr) =>
        {
            var point = (ImPlotPoint*)pointPtr;
            point->X = getX(ref buffer[idx]);
            point->Y = getY(ref buffer[idx]);
            return pointPtr;
        });
    }

    unsafe IObservable<RollingPlotPointSeries<TSource>> Process<TSource>(IObservable<TSource> source, KeyValuePair<string, ValueGetter<TSource>>[] valueGetters)
    {
        return Observable.Defer(() =>
        {
            var buffer = new RollingBuffer<TSource>(Capacity);
            var getters = Array.ConvertAll(valueGetters, getter => CreatePlotPointGetter(buffer, getter.Key, getter.Value));
            var series = new RollingPlotPointSeries<TSource>(buffer, getters);
            return source.Select(value =>
            {
                buffer.Push(value);
                return series;
            });
        });
    }

    unsafe IObservable<RollingPlotPointSeries<TSource>> ProcessCollection<TSource>(IObservable<IEnumerable<TSource>> source, KeyValuePair<string, ValueGetter<TSource>>[] valueGetters)
    {
        return Observable.Defer(() =>
        {
            var buffer = new RollingBuffer<TSource>(Capacity);
            var getters = Array.ConvertAll(valueGetters, getter => CreatePlotPointGetter(buffer, getter.Key, getter.Value));
            var series = new RollingPlotPointSeries<TSource>(buffer, getters);
            return source.Select(value =>
            {
                buffer.Push(value);
                return series;
            });
        });
    }

    unsafe IObservable<RollingPlotPointSeries<TSource>> Process<TSource>(IObservable<TSource> source, ValueGetter<TSource> indexGetter, KeyValuePair<string, ValueGetter<TSource>>[] valueGetters)
    {
        return Observable.Defer(() =>
        {
            var buffer = new RollingBuffer<TSource>(Capacity);
            var getters = Array.ConvertAll(valueGetters, getter => CreatePlotPointGetter(buffer, getter.Key, indexGetter, getter.Value));
            var series = new RollingPlotPointSeries<TSource>(buffer, getters);
            return source.Select(value =>
            {
                buffer.Push(value);
                return series;
            });
        });
    }

    unsafe IObservable<RollingPlotPointSeries<TSource>> ProcessCollection<TSource>(IObservable<IEnumerable<TSource>> source, ValueGetter<TSource> indexGetter, KeyValuePair<string, ValueGetter<TSource>>[] valueGetters)
    {
        return Observable.Defer(() =>
        {
            var buffer = new RollingBuffer<TSource>(Capacity);
            var getters = Array.ConvertAll(valueGetters, getter => CreatePlotPointGetter(buffer, getter.Key, indexGetter, getter.Value));
            var series = new RollingPlotPointSeries<TSource>(buffer, getters);
            return source.Select(value =>
            {
                buffer.Push(value);
                return series;
            });
        });
    }

    delegate double ValueGetter<TSource>(ref TSource source);
}
