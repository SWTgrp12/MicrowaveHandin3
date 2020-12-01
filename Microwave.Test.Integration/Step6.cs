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
    class Step6
    {
        private Light light;
        //private Output output;
        private IOutput output; // substitute used due to making things easier with Nsubstitute functions, it has already been tested in the first place
        private Display display;
        private UserInterface uut;
        private Timer timer;
        private PowerTube powertube;
        private CookController _ckctrl;
        private Button _startCancelButton;
        private Button _powerButton;
        private Button _timeButton;
        private Door _door;

        [SetUp]
        public void Setup()
        {
            _door = new Door();
            // output = new Output();
            output = Substitute.For<IOutput>(); // substitute used due to making things easier with Nsubstitute functions, it has already been tested in the first place
            _startCancelButton = new Button();
            _powerButton = new Button();
            _timeButton = new Button();
            timer = new Timer();
            powertube = new PowerTube(output);
            display = new Display(output);
            _ckctrl = new CookController(timer, display, powertube);
            light = new Light(output);
            uut = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, display, light, _ckctrl);
            _ckctrl.UI = uut;
        }


        [Test]
        public void Ready_DoorOpen_LightOn()
        {
            _door.Open();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Light is turned on")));
        }

        [Test]
        public void Ready_DoorOpen_DoorClose_LightOff()
        {
            
            _door.Open();
            _door.Close();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Light is turned off")));
        }

        [TestCase(1, 50)]
        [TestCase(2, 100)]
        [TestCase(14, 700)]
        public void Ready_DoorOpenClose_Ready_Setpower(int press,int power)
        {
            _door.Open();
            _door.Close();
            for (int i = 0; i < press; i++)
            { 
                _powerButton.Press();
            }
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains($"Display shows: {power} W")));
        }

        [Test]
        public void Ready_resetpower_to_50_after_700()
        {
            for (int i = 1; i <= 15; i++)
            {
                _powerButton.Press();
            }
            output.Received(2).OutputLine(Arg.Is<string>(str => str.Contains("Display shows: 50 W")));
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            _powerButton.Press();

            _startCancelButton.Press();

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));

        }

        [TestCase("Display cleared")]
        [TestCase("Light is turned on")]
        public void SetPower_DoorOpened_DisplayCleared_lightisturnedon(string a) // checks if all messages sent based on actions to display
        {
            _powerButton.Press();

            _door.Open();

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(a)));
        }

        [TestCase(1,1,0)]
        [TestCase(2, 2, 0)]
        [TestCase(3, 3, 0)]
        [TestCase(4, 4, 0)]
        [TestCase(5, 5, 0)]
        public void SetPower_TimeButtonPress_view_display_time(int press, int min, int sec)
        {
            _powerButton.Press();

            for (int i = 0; i < press; i++)
            {
                _timeButton.Press();

            }
           
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains($"Display shows: {min:D2}:{sec:D2}")));
        }


        [TestCase("Display shows: 50 W")]
        public void SetTime_StartButton_CookerIsCalled_50W(string a)
        {
            _powerButton.Press();

            _timeButton.Press();

            _startCancelButton.Press();

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(a)));

        }

        [Test]
        public void SetTime_StartButton_CookerIsCalled_1min()
        {
            _powerButton.Press();

            _timeButton.Press();

            _startCancelButton.Press();
            
            Assert.That(timer.TimeRemaining,Is.EqualTo(60));
        }

        [Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            _powerButton.Press();

            _timeButton.Press();
            
            _door.Open();

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
        }

        [Test]
        public void SetTime_DoorOpened_LightOn()
        {
            _powerButton.Press();

            _timeButton.Press();

            _door.Open();

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Light is turned on")));
        }

        
        [Test]
        public void SetTime_StartButton_LightIsCalled()
        {
            _powerButton.Press();
            
            _timeButton.Press();
            
            _startCancelButton.Press();

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Light is turned on")));
        }

        [Test]
        public void Cooking_CookingIsDone_LightOff()
        {
            _powerButton.Press();

            _timeButton.Press();

            _startCancelButton.Press();

              while (timer.TimeRemaining != 0)
                {
                // wait til cooking is finished
                }

            
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Light is turned off")));
        }

        [Test]
        public void Cooking_CookingIsDone_ClearDisplay()
        {
            _powerButton.Press();

            _timeButton.Press();

            _startCancelButton.Press();

            while (timer.TimeRemaining != 0)
            {
                // wait til cooking is finished
            }

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
        }

        

    }
}