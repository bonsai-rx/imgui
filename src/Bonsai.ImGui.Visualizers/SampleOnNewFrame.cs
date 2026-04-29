using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Represents an operator that samples notifications from the sequence
/// whenever there is a new render frame event from the specified subject.
/// </summary>
[WorkflowElementCategory(ElementCategory.Combinator)]
[WorkflowElementIcon("Bonsai:Expressions.SubscribeSubject")]
[Description("Samples notifications from the sequence whenever there is a new render frame event from the specified subject.")]
public class SampleOnNewFrame : SubscribeSubject
{
    static readonly Range<int> argumentRange = Range.Create(lowerBound: 1, upperBound: 1);

    /// <inheritdoc/>
    public override Range<int> ArgumentRange => argumentRange;

    /// <inheritdoc/>
    public override Expression Build(IEnumerable<Expression> arguments)
    {
        var source = arguments.First();
        var sampler = base.Build(Enumerable.Empty<Expression>());
        if (sampler.Type == typeof(void)) return source;

        var sourceType = source.Type.GetGenericArguments()[0];
        var samplerType = sampler.Type.GetGenericArguments()[0];
        return Expression.Call(typeof(SampleOnNewFrame), nameof(SampleSafe), new[] { sourceType, samplerType }, source, sampler);
    }

    static IObservable<TSource> Process<TSource>(ISubject<TSource> subject)
    {
        return subject;
    }

    static IObservable<TSource> SampleSafe<TSource, TSample>(IObservable<TSource> source, IObservable<TSample> sampler)
    {
        return source.Sample(sampler).TakeUntil(sampler.IgnoreElements().LastOrDefaultAsync());
    }
}
