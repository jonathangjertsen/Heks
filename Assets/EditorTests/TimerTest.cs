using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TimerTest
    {
        private int timerTop = 10;

        [Test]
        public void StartStartsTimer()
        {
            var timer = new Timer(timerTop, null);
            Assert.AreEqual(timer.Value, timerTop);
            Assert.False(timer.Running);
            timer.Start();
            Assert.AreEqual(timer.Value, timerTop);
            Assert.True(timer.Running);
        }

        [Test]
        public void StopStopsTimer()
        {
            var timer = new Timer(timerTop, null);
            Assert.False(timer.Running);
            timer.Start();
            timer.Stop();
            Assert.False(timer.Running);
        }

        [Test]
        public void CallsTimeoutAndStopsItselfAtCorrectTime()
        {
            bool timeoutCalled = false;
            int timerTop = 10;
            var timer = new Timer(timerTop, () => timeoutCalled = true);
            timer.Start();
            for (int i = 0; i < timerTop; i++)
            {
                Assert.False(timeoutCalled);
                timer.Tick();
            }
            Assert.True(timeoutCalled);
            Assert.False(timer.Running);
        }

        [Test]
        public void CanUpdateSetTimeoutCallback()
        {
            bool timeoutCalledOriginal = false;
            bool timeoutCalledNew = false;
            var timer = new Timer(timerTop, () => timeoutCalledOriginal = true);
            timer.Start();
            timer.Tick();
            timer.SetTimeoutCallback(() => timeoutCalledNew = true);
            timer.TickThrough();
            Assert.True(timeoutCalledNew);
            Assert.False(timeoutCalledOriginal);
        }

        [Test]
        public void StopResetsTimerValue()
        {
            var timer = new Timer(timerTop, null);
            Assert.False(timer.Running);
            timer.Start();
            timer.Tick();
            Assert.AreEqual(timer.Value, timerTop - 1);
            timer.Stop();
            Assert.AreEqual(timer.Value, timerTop);
            Assert.False(timer.Running);
            timer.Tick();
            Assert.False(timer.Running);
            Assert.AreEqual(timer.Value, timerTop);
        }

        [Test]
        public void PauseStopsTimerButDoesNotResetTimerValue()
        {
            var timer = new Timer(timerTop, null);
            Assert.False(timer.Running);
            timer.Start();
            timer.Tick();
            Assert.AreEqual(timer.Value, timerTop - 1);
            timer.Pause();
            Assert.False(timer.Running);
            Assert.AreEqual(timer.Value, timerTop - 1);
            timer.Tick();
            Assert.False(timer.Running);
            Assert.AreEqual(timer.Value, timerTop - 1);
        }
    }

    public class TimerCollectionTests
    {
        private int timerTop = 10;

        [Test]
        public void CanCreateCollection()
        {
            new TimerCollection();
        }

        [Test]
        public void ReadTimerFails()
        {
            var timers = new TimerCollection();
            try
            {
                timers.Running("none");
            }
            catch (TimerNotFoundException exc)
            {
                CheckExceptionMessage(exc, "none", "");
            }
        }

        [Test]
        public void CanReadFromAddedTimer()
        {
            var timers = new TimerCollection();
            timers.Add("a timer", 10, null);

            Assert.False(timers.Running("a timer"));
        }

        [Test]
        public void ExceptionMessageHasAvailableTimers()
        {
            var timers = new TimerCollection();
            timers.Add("a timer", 10, null);
            timers.Add("another timer", 10, null);

            Assert.False(timers.Running("a timer"));

            try
            {
                timers.Running("none");
            }
            catch (TimerNotFoundException exc)
            {
                CheckExceptionMessage(exc, "none", "'a timer', 'another timer'");
            }
        }

        [Test]
        public void StartAndStopOccursOnNextTickAll()
        {
            int timerTop = 50;
            bool timerBTimedOut = false;

            var timers = new TimerCollection();
            timers.Add("A", timerTop, () => timers.Stop("B"));
            timers.Add("B", timerTop, () => timerBTimedOut = true);
            timers.Add("C", timerTop, () => timers.Stop("B"));

            timers.Start("A");
            timers.Start("B");
            timers.Start("C");

            Assert.False(timers.Running("A"));
            Assert.False(timers.Running("B"));
            Assert.False(timers.Running("C"));

            timers.PropagateStartAndStop();

            Assert.True(timers.Running("A"));
            Assert.True(timers.Running("B"));

            for (int i = 0; i < timerTop; i++)
            {
                Assert.True(timers.Running("A"));
                Assert.True(timers.Running("B"));
                Assert.True(timers.Running("C"));
                timers.TickAll();
            }

            Assert.True(timerBTimedOut);

            Assert.False(timers.Running("A"));
            Assert.False(timers.Running("B"));
            Assert.False(timers.Running("C"));
        }

        [Test]
        public void TimerRing()
        {
            int timerTop = 10;

            int timeoutsA = 0;
            int timeoutsB = 0;
            int timeoutsC = 0;

            var timers = new TimerCollection();
            timers.Add("A", timerTop, () => { timeoutsA++; timers.Start("B"); });
            timers.Add("B", timerTop, () => { timeoutsB++; timers.Start("C"); });
            timers.Add("C", timerTop, () => { timeoutsC++; timers.Start("A"); });

            timers.Start("A");

            timers.PropagateStartAndStop();

            for (int i = 0; i < timerTop; i++)
            {
                timers.TickAll();
            }

            Assert.AreEqual(1, timeoutsA);
            Assert.AreEqual(0, timeoutsB);
            Assert.AreEqual(0, timeoutsC);

            for (int i = 0; i < timerTop; i++)
            {
                timers.TickAll();
            }

            Assert.AreEqual(1, timeoutsA);
            Assert.AreEqual(1, timeoutsB);
            Assert.AreEqual(0, timeoutsC);


            for (int i = 0; i < timerTop; i++)
            {
                timers.TickAll();
            }

            Assert.AreEqual(1, timeoutsA);
            Assert.AreEqual(1, timeoutsB);
            Assert.AreEqual(1, timeoutsC);

            for (int i = 0; i < timerTop; i++)
            {
                timers.TickAll();
            }

            Assert.AreEqual(2, timeoutsA);
            Assert.AreEqual(1, timeoutsB);
            Assert.AreEqual(1, timeoutsC);

            for (int i = 0; i < timerTop; i++)
            {
                timers.TickAll();
            }

            Assert.AreEqual(2, timeoutsA);
            Assert.AreEqual(2, timeoutsB);
            Assert.AreEqual(1, timeoutsC);
        }

        [Test]
        public void TickAllTicksAll()
        {
            int timerTopA = 10;
            int timerTopB = 15;
            int timerTopC = 20;

            bool timerATimedOut = false;
            bool timerBTimedOut = false;
            bool timerCTimedOut = false;

            var timers = new TimerCollection();
            timers.Add("A", timerTopA, () => timerATimedOut = true);
            timers.Add("B", timerTopB, () => timerBTimedOut = true);
            timers.Add("C", timerTopC, () => timerCTimedOut = true);

            timers.Start("A");
            timers.Start("B");
            timers.Start("C");

            timers.PropagateStartAndStop();

            Assert.AreEqual(timers.Value("A"), timerTopA);
            Assert.AreEqual(timers.Value("B"), timerTopB);
            Assert.AreEqual(timers.Value("C"), timerTopC);

            for (int i = 0; i < timerTopA; i++)
            {
                Assert.AreEqual(timers.Value("A"), timerTopA - i);
                Assert.AreEqual(timers.Value("B"), timerTopB - i);
                Assert.AreEqual(timers.Value("C"), timerTopC - i);
                timers.TickAll();
            }

            Assert.True(timerATimedOut);
            Assert.False(timerBTimedOut);
            Assert.False(timerCTimedOut);

            for (int i = 0; i < (timerTopB - timerTopA); i++)
            {
                Assert.AreEqual(timers.Value("A"), timerTopA);
                Assert.AreEqual(timers.Value("B"), timerTopB - timerTopA - i);
                Assert.AreEqual(timers.Value("C"), timerTopC - timerTopA - i);
                timers.TickAll();
            }

            Assert.True(timerATimedOut);
            Assert.True(timerBTimedOut);
            Assert.False(timerCTimedOut);

            for (int i = 0; i < (timerTopC - timerTopB); i++)
            {
                Assert.AreEqual(timers.Value("A"), timerTopA);
                Assert.AreEqual(timers.Value("B"), timerTopB);
                Assert.AreEqual(timers.Value("C"), timerTopC - timerTopB - i);
                timers.TickAll();
            }

            Assert.AreEqual(timers.Value("A"), timerTopA);
            Assert.AreEqual(timers.Value("B"), timerTopB);
            Assert.AreEqual(timers.Value("C"), timerTopC);

            Assert.True(timerATimedOut);
            Assert.True(timerBTimedOut);
            Assert.True(timerCTimedOut);
        }

        private void CheckExceptionMessage(TimerNotFoundException exc, string name, string available)
        {
            Assert.AreEqual(exc.Message, $"No timer named '{name}'. Available timers: {available}");
        }
    }
}
