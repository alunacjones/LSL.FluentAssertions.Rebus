using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using LSL.FluentAssertions.Rebus;
using Rebus.TestHelpers;
using Rebus.TestHelpers.Events;

namespace LSL.FluentAssertions.Rebus
{
    /// <summary>
    /// RebusFakeBusAssertions
    /// </summary>
    public class RebusFakeBusAssertions<TEvent> : GenericCollectionAssertions<IEnumerable<TEvent>, TEvent, RebusFakeBusAssertions<TEvent>>
        where TEvent : FakeBusEvent
    {
        internal RebusFakeBusAssertions(IEnumerable<TEvent> instance) : base(instance) { }

        /// <inheritdoc/>
        protected override string Identifier => nameof(RebusFakeBusAssertions<TEvent>);

        /// <summary>
        /// Asserts the number of messages on the FakeBus instance
        /// </summary>
        /// <param name="expectedMessageCount">Expected Number of messages on the bus</param>
        /// <param name="because"></param>
        /// <param name="becauseArgs"></param>
        /// <returns></returns>
        public AndConstraint<RebusFakeBusAssertions<TEvent>> HaveMessageCount(int expectedMessageCount, string because = "", params object[] becauseArgs) 
        {
            var messageCount = Subject.Count();

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
        public AndConstraint<RebusFakeBusAssertions<TEvent>> BeEmpty(string because = "", params object[] becauseArgs)
        {
            var messageCount = Subject.Count();

            return Fluently(() => Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(messageCount == 0)
                .FailWith("Expected bus to contain no messages{reason} but found {0}", messageCount)
            );            
        }

        /// <summary>
        /// Returns the collection assertion object for MessageSent<typeparamref name="TMessage"/> events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<RebusFakeBusAssertions<MessageSent<TMessage>>> HaveSentMessages<TMessage>() =>
            ReturnForTransportMessages<MessageSent<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for MessagePublishedToTopic&lt;<typeparamref name="TMessage"/>&gt; events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<RebusFakeBusAssertions<MessagePublishedToTopic<TMessage>>> HavePublishedToTopicMessages<TMessage>() =>
            ReturnForTransportMessages<MessagePublishedToTopic<TMessage>>();

        /// <summary>
        /// Returns the collection assertion object for MessageDeferred<typeparamref name="TMessage"/> events on the bus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public AndConstraint<RebusFakeBusAssertions<MessageDeferred<TMessage>>> HaveDeferredMessages<TMessage>() =>
            ReturnForTransportMessages<MessageDeferred<TMessage>>();

        public AndConstraint<RebusFakeBusAssertions<MessageDeferredToDestination<TMessage>>> HaveDeferredToDesinationMessages<TMessage>() =>
            ReturnForTransportMessages<MessageDeferredToDestination<TMessage>>();

        public AndConstraint<RebusFakeBusAssertions<MessageDeferredToSelf<TMessage>>> HaveDeferredToSelfMessages<TMessage>() =>
            ReturnForTransportMessages<MessageDeferredToSelf<TMessage>>();

        public AndConstraint<RebusFakeBusAssertions<MessagePublished<TMessage>>> HavePublishedMessages<TMessage>() =>
            ReturnForTransportMessages<MessagePublished<TMessage>>();

        public AndConstraint<RebusFakeBusAssertions<MessageSentToDestination<TMessage>>> HaveSentToDestinationMessages<TMessage>() =>
            ReturnForTransportMessages<MessageSentToDestination<TMessage>>();

        public AndConstraint<RebusFakeBusAssertions<MessageSentToSelf<TMessage>>> HaveSentToSelfMessages<TMessage>() =>
            ReturnForTransportMessages<MessageSentToSelf<TMessage>>();

        public AndConstraint<RebusFakeBusAssertions<MessageSentWithRoutingSlip<TMessage>>> HaveSentWithRoutingSlipMessages<TMessage>() =>
            ReturnForTransportMessages<MessageSentWithRoutingSlip<TMessage>>();

        private AndConstraint<RebusFakeBusAssertions<TTransportMessage>> ReturnForTransportMessages<TTransportMessage>()
            where TTransportMessage : FakeBusEvent
        {
            var messages = Subject.OfType<TTransportMessage>();

            Execute.Assertion
                .BecauseOf("")
                .ForCondition(messages.Any())
                .FailWith("The bus contains no messages of type {0}", GetFriendlyTypeName(typeof(TTransportMessage)));

            return new AndConstraint<RebusFakeBusAssertions<TTransportMessage>>(new RebusFakeBusAssertions<TTransportMessage>(
                messages
            ));
        }

        private AndConstraint<RebusFakeBusAssertions<TEvent>> Fluently(Action assertions)
        {
            assertions();
            return new AndConstraint<RebusFakeBusAssertions<TEvent>>(this);
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