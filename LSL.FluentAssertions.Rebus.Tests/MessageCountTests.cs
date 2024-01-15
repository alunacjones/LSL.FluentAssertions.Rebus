using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Rebus.TestHelpers;
using RR = Rebus.Routing;

namespace LSL.FluentAssertions.Rebus.Tests
{
    public class MessageCountTests
    {
        [Test]
        public void BeEmpty_GivenAnEmptyBusItShouldNotFail()
        {
            new FakeBus().Should().BeEmpty();
        }

        [Test]
        public void BeEmpty_GivenANoneEmptyBusItShouldFail()
        {
            var bus = new FakeBus();
            bus.Send(new object());
            new Action(() => bus.Should().BeEmpty())
                .Should()
                .Throw<AssertionException>()
                .WithMessage("Expected bus to contain no messages but found 1");
        }

        [Test]
        public void BeEmpty_GivenANoneEmptyBusItShouldFailWithTheGivenReason()
        {
            var bus = new FakeBus();
            bus.Send(new object());

            new Action(() => bus.Should().BeEmpty("it should {0}", "test-var"))
                .Should()
                .Throw<AssertionException>()
                .WithMessage("Expected bus to contain no messages because it should test-var but found 1");
        }

        [Test]
        public void WithMessageCount_GivenABusWithTwoMessagesAndAnAssertionForTwoMessages_ItShouldPass()
        {
            var bus = new FakeBus();
            bus.Send(new object());
            bus.Publish(new object());
            bus.Should().HaveMessageCount(2);
        }

        [Test]
        public void WithMessageCount_GivenABusWithOneMessageAndAnAssertionForTwoMessages_ItShouldFail()
        {
            var bus = new FakeBus();
            bus.Send(new object());
            new Action(() => bus.Should().HaveMessageCount(2))
                .Should()
                .Throw<AssertionException>()
                .WithMessage("Expected bus to contain 2 messages but found 1");
        }

        [Test]
        public void WithMessageCount_GivenABusWithOneMessageAndAnAssertionForTwoMessages_ItShouldFailWithTheGivenReason()
        {
            var bus = new FakeBus();
            bus.Send(new object());
            new Action(() => bus.Should().HaveMessageCount(2, "it should {0}", "test-var"))
                .Should()
                .Throw<AssertionException>()
                .WithMessage("Expected bus to contain 2 messages because it should test-var but found 1");
        }

        [Test]
        public void HaveSentMessages_GIvenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.Send(new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HaveSentMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HaveSentMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.Send(new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HaveSentMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void HavePublishedMessages_GivenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.Publish(new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HavePublishedMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.EventMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HavePublishedMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.Publish(new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HavePublishedMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.EventMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void HavePublishedToTopicMessages_GivenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.Advanced.Topics.Publish("topic", new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HavePublishedToTopicMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.EventMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HavePublishedToTopicMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.Advanced.Topics.Publish("topic", new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HavePublishedToTopicMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.EventMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void HaveSentToSelfMessages_GivenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.SendLocal(new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HaveSentToSelfMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HaveSentToSelfMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.SendLocal(new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HaveSentToSelfMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void HaveSentWithRoutingSlipMessages_GivenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.Advanced.Routing.SendRoutingSlip(new RR.Itinerary(), new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HaveSentWithRoutingSlipMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HaveSentWithRoutingSlipMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.Advanced.Routing.SendRoutingSlip(new RR.Itinerary(), new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HaveSentWithRoutingSlipMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void HaveDeferredMessages_GivenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.Defer(TimeSpan.MaxValue, new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HaveDeferredMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HaveDeferredMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.Defer(TimeSpan.MaxValue, new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HaveDeferredMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void HaveDeferredToSelfMessages_GivenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.DeferLocal(TimeSpan.MaxValue, new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HaveDeferredToSelfMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HaveDeferredToSelfMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.DeferLocal(TimeSpan.MaxValue, new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HaveDeferredToSelfMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void HaveDeferredToDesinationMessages_GivenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.Advanced.Routing.Defer("queue", TimeSpan.MaxValue, new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HaveDeferredToDesinationMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HaveDeferredToDesinationMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.Advanced.Routing.Defer("queue", TimeSpan.MaxValue, new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HaveDeferredToDesinationMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void HaveSentToDestinationMessages_GivenABusWithASentMessage_ItShouldPassWIthACorrectAssertion()
        {
            var bus = new FakeBus();
            bus.Advanced.Routing.Send("a-queue", new KeyValuePair<string, string>("key", "value"), new Dictionary<string, string> { ["header"] = "header-value" });

            bus.Should().HaveSentToDestinationMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                });
        }

        [Test]
        public void HaveSentToDestinationMessages_GivenABusWithASentMessage_ItShouldFailWithIncorrectAssertions()
        {
            var bus = new FakeBus();
            bus.Advanced.Routing.Send("a-queue", new KeyValuePair<string, string>("key1", "value"), new Dictionary<string, string> { ["header1"] = "header-value" });

            new Action(() => bus.Should().HaveSentToDestinationMessages<KeyValuePair<string, string>>().And
                .SatisfyRespectively(m =>
                {
                    m.OptionalHeaders.Should().BeEquivalentTo(new Dictionary<string, string> { ["header"] = "header-value" });
                    m.CommandMessage.Should().BeEquivalentTo(new KeyValuePair<string, string>("key", "value"));
                })
            )
            .Should()
            .Throw<AssertionException>()
            .WithMessage(@"Expected bus to satisfy all inspectors, but some inspectors are not satisfied:*");
        }

        [Test]
        public void Blah()
        {
            var bus = new FakeBus();
            bus.Send("a message");
            bus.Should().HaveMessageCount(1)
                .And
                .HaveSentMessages<string>()
                .And
                .SatisfyRespectively(m => m.CommandMessage.Should().Be("a message"));
        }

        [Test]
        public void asd()
        {
            var bus = new FakeBus();
            bus.Send("a message");
            bus.Publish("an event");

            bus.Should().HaveMessageCount(2);
            bus.Should().HaveSentMessages<string>()
                .And.SatisfyRespectively(m => m.CommandMessage.Should().Be("a message"));

            bus.Should().HavePublishedMessages<string>()
                .And.SatisfyRespectively(m => m.EventMessage.Should().Be("an event"));                
        }
    }
}