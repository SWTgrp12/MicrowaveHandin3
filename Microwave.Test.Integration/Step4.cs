using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    class Step4
    {
        private IOutput output;
        private PowerTube powerTube;
        private Display display;
        private Timer timer;
        private CookController controller;
        private IUserInterface ui;
        [SetUp]
        public void Setup()
        {
            ui = Substitute.For<IUserInterface>();
            //output = new Output();
            output = Substitute.For<IOutput>();
            timer = new Timer();
            display = new Display(output);
            powerTube = new PowerTube(output);

            controller = new CookController(timer, display, powerTube, ui);
        }
        [Test]
        public void StartCooking_ValidParameters_TimerStarted()
        {
            controller.StartCooking(50, 60);
            Assert.That(timer.TimeRemaining, Is.EqualTo(60));
        }
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        public void StartCooking_ValidParameters_TimerElapsed(int time)
        {
            controller.StartCooking(50, time);
            System.Threading.Thread.Sleep(time * 1000 + 100);

            ui.Received().CookingIsDone();
        }
        [Test]
        public void Cooking_TimerTick_DisplayCalled_ShowsCorrectValue()
        {
            controller.StartCooking(50,  4);
            // wait n seconds
            System.Threading.Thread.Sleep(2 * 1000 + 100 );
            // check that the CookController has printed the right time
            // corresponding to number of seconds that has passed

            // TODO: This doesn't work, seems like it doesn't change from stdout correctly? 

            //StringWriter str = new StringWriter();
            //Console.SetOut(str);
            //Assert.That(str.ToString().Contains("00:02"));

            // Is it fine to just substitute Output?
            output.Received().OutputLine("Display shows: 00:02");
        }
    }

}