using Rebus.TestHelpers;
using Rebus.TestHelpers.Events;

namespace LSL.FluentAssertions.Rebus
{
    /// <summary>
    /// RebusFluentAssertionsExtensions
    /// </summary>
    public static class RebusFluentAssertionsExtensions
    {
        /// <summary>
        /// Provides assertions for Rebus FakeBus
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static RebusFakeBusAssertions<FakeBusEvent> Should(this FakeBus instance) => new RebusFakeBusAssertions<FakeBusEvent>(instance.Events);
    }
}