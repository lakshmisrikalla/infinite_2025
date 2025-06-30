using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_4
{
        public class MobilePhone
        {
            public delegate void RingEventHandler();


            public event RingEventHandler OnRing;

            public void ReceiveCall()
            {
                Console.WriteLine("Incoming call...");
                OnRing?.Invoke();
            }
        }


        public class RingtonePlayer
        {
            public void OnRing()
            {
                Console.WriteLine("Playing ringtone...");
            }
        }

        public class ScreenDisplay
        {
            public void OnRing()
            {
                Console.WriteLine("Displaying caller information...");
            }
        }

        public class VibrationMotor
        {
            public void OnRing()
            {
                Console.WriteLine("Phone is vibrating...");
            }
        }
        class Mobile
        {
            static void Main()
            {
                MobilePhone phone = new MobilePhone();


                RingtonePlayer ringtone = new RingtonePlayer();
                ScreenDisplay screen = new ScreenDisplay();
                VibrationMotor motor = new VibrationMotor();


                phone.OnRing += ringtone.OnRing;
                phone.OnRing += screen.OnRing;
                phone.OnRing += motor.OnRing;

                phone.ReceiveCall();
                Console.ReadLine();
            }
        }
    }
