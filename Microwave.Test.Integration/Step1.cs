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

namespace Microwave.Test.Unit
{
    class Step1
    {
        private StringWriter str;
        private IOutput output;
        private PowerTube powertube;
        private Display display;
        private Light light;
        [SetUp]
        public void Setup()
        {
            // setting up our own out to write to instead of stdout
            // this lets us easily check what this out has received
            str = new StringWriter();
            Console.SetOut(str);
            output = new Output();
            powertube = new PowerTube(output);
            display = new Display(output);
            light = new Light(output);
        }
        /// just a lil helper function
        public void OutputLineCorrect(String a)
        {
            Assert.That(str.ToString().Contains(a));
        }

        [Test]
        public void Display_ShowTime_CorrectOutput()
        {
            display.ShowTime(10, 15);
            OutputLineCorrect("10:15");
        }
        [Test]
        public void Display_Clear_CorrectOutput()
        {
            display.Clear();
            OutputLineCorrect("cleared");
        }
        [Test]
        public void Display_ShowPower_CorrectOutput()
        {
            display.ShowPower(150);
            OutputLineCorrect("150 W");
        }
        // TODO: Should we have more calls to light and PowerTube? Seems unnecessary since 
        // we're just interested in whether Output works with it
        [Test]
        public void Light_TurnOn_WasOff_CorrectOutput()
        {
            light.TurnOn();
            OutputLineCorrect("on");
        }
        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        public void PowerTube_TurnOn_WasOffCorrectPower_CorrectOutput(int power)
        {
            powertube.TurnOn(power);
            OutputLineCorrect($"{power}");
        }
    }

}