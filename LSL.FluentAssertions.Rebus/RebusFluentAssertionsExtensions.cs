using Rebus.TestHelpers;

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
        public static RebusFakeBusAssertions Should(this FakeBus instance) => new RebusFakeBusAssertions(instance);
    }
}