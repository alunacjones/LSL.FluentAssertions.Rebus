using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Rebus.TestHelpers;
using Rebus.TestHelpers.Events;

namespace LSL.FluentAssertions.Rebus
{
    /// <summary>
    /// RebusFakeBusAssertions
    /// </summary>
    public class RebusFakeBusAssertions : ReferenceTypeAssertions<FakeBus, RebusFakeBusAssertions>
    {
        internal RebusFakeBusAssertions(FakeBus instance) : base(instance) { }

        /// <inheritdoc/>
        protected override string Identifier => nameof(RebusFakeBusAssertions);

        /// <summary>
        /// Asserts the number of messages on the FakeBus instance
        /// </summary>
        /// <param name="expectedMessageCount">Expected Number of messages on the bus</param>
        /// <param name="because"></param>
        /// <param name="becauseArgs"></param>
        /// <returns></returns>
        public AndConstraint<RebusFakeBusAssertions> HaveMessageCount(int expectedMessageCount, string because = "", params object[] becauseArgs) 
        {
            var messageCount = Subject.Events.Count();

            return Fluently(() => Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(messageCount == expectedMessageCount)
                .FailWith("Expected bus to contain {0} messages{reason} but found {1}", expectedMessageCount, messageCount)
            );
        }

        /// <summary>
        /// Asserts that there are no messages on the FakeBus instance
        /// </summary>
        /// <param name="because"></param>
        /// <param name="becauseArgs"></param>
        /// <returns></returns>
        public AndConstraint<RebusFakeBusAssertions> BeEmpty(string because = "", params object[] becauseArgs)
        {
            var messageCount = Subject.Events.Count();

            return Fluently(() => Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(messageCount == 0)
                .FailWith("Expected bus to contain no messages{reason} but found {0}", messageCount)
            );            
        }

        /// <summary>
        /// Returns the collection assertion object for MessageDeferred&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessageDeferred<TMessage>>> HaveDeferredMessages<TMessage>() =>
            ReturnForTransportMessages<MessageDeferred<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for HaveDeferredToDesinationMessages&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessageDeferredToDestination<TMessage>>> HaveDeferredToDesinationMessages<TMessage>() =>
            ReturnForTransportMessages<MessageDeferredToDestination<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for HaveDeferredToSelfMessages&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessageDeferredToSelf<TMessage>>> HaveDeferredToSelfMessages<TMessage>() =>
            ReturnForTransportMessages<MessageDeferredToSelf<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for HavePublishedMessages&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessagePublished<TMessage>>> HavePublishedMessages<TMessage>() =>
            ReturnForTransportMessages<MessagePublished<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for MessagePublishedToTopic&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessagePublishedToTopic<TMessage>>> HavePublishedToTopicMessages<TMessage>() =>
            ReturnForTransportMessages<MessagePublishedToTopic<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for MessageSent&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessageSent<TMessage>>> HaveSentMessages<TMessage>() =>
            ReturnForTransportMessages<MessageSent<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for HaveSentToDestinationMessages&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessageSentToDestination<TMessage>>> HaveSentToDestinationMessages<TMessage>() =>
            ReturnForTransportMessages<MessageSentToDestination<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for HaveSentToSelfMessages&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessageSentToSelf<TMessage>>> HaveSentToSelfMessages<TMessage>() =>
            ReturnForTransportMessages<MessageSentToSelf<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for HaveSentWithRoutingSlipMessages&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<PerMessageTypeAssertions<MessageSentWithRoutingSlip<TMessage>>> HaveSentWithRoutingSlipMessages<TMessage>() =>
            ReturnForTransportMessages<MessageSentWithRoutingSlip<TMessage>>();

        private AndConstraint<PerMessageTypeAssertions<TTransportMessage>> ReturnForTransportMessages<TTransportMessage>()
            where TTransportMessage : FakeBusEvent
        {
            var messages = Subject.Events.OfType<TTransportMessage>();

            Execute.Assertion
                .BecauseOf("")
                .ForCondition(messages.Any())
                .FailWith("The bus contains no messages of type {0}", GetFriendlyTypeName(typeof(TTransportMessage)));

            return new AndConstraint<PerMessageTypeAssertions<TTransportMessage>>(new PerMessageTypeAssertions<TTransportMessage>(
                messages
            ));
        }

        private AndConstraint<RebusFakeBusAssertions> Fluently(Action assertions)
        {
            assertions();
            return new AndConstraint<RebusFakeBusAssertions>(this);
        }

        private string GetFriendlyTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var cleanedUpName = new string(type.Name.TakeWhile(c => c != '`').ToArray());
                return $"{cleanedUpName}<{string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName))}>";
            }

            return type.Name;
        }
    }
}
