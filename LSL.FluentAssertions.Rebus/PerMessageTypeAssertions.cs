using System.Collections.Generic;
using FluentAssertions.Collections;

/// <summary>
/// PerMessageTypeAssertions
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public class PerMessageTypeAssertions<TEvent> : GenericCollectionAssertions<IEnumerable<TEvent>, TEvent, PerMessageTypeAssertions<TEvent>>
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="actualValue"></param>
    /// <returns></returns>
    public PerMessageTypeAssertions(IEnumerable<TEvent> actualValue) : base(actualValue)
    {
    }
}