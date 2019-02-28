using System;
using FakeItEasy;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using SharpTox.Core;
using SharpTox.Core.Interfaces;

namespace SharpTox.UnitTests
{
    [TestFixture(TestOf = typeof(ToxLoop))]
    public class ToxLoop_Test
    {
        [Test]
        public void Start_NullToxIterate_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ToxLoop.Start(null));
        }

        [Test]
        public void Start_ToxIterate_CallIterate()
        {
            var scheduler = new TestScheduler();
            var iterate = A.Fake<IToxIterate>();

            A.CallTo(() => iterate.Iterate())
                .Returns(TimeSpan.FromTicks(1));

            using(ToxLoop.Start(iterate, scheduler))
            {
                scheduler.AdvanceBy(2);
            }

            A.CallTo(() => iterate.Iterate())
                .MustHaveHappened(2, Times.OrMore);
        }
    }
}
