using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;

/// <summary>
/// The author is Robert Lute
/// getrobertajob@gmail.com
/// 719-310-0055
/// 
/// This is a program to gather stats on the CPU load and Memory useage and relay info to the user. 
/// 
/// It will get current CPU load, 
///   notify the user of the current CPU load via text to speech and displayed in the console, 
///   and then output a different message if the CPU load is above a threashold. 
///   
/// It will get current memory usage, 
///   notify the user of the current memory usuage via text to speech and displayed in the console, 
///   and then output a different message if the memory usuage is above a threashold. 
///   
/// This was to test geting computer stats and building functions to use the speech synthesis of windows. 
/// </summary>
namespace Voice_CPU_Memory_Monitor
{
    class Program
    {
        private static SpeechSynthesizer sysnth = new SpeechSynthesizer();

        //
        // WHERE ALL THE MAGIC HAPPENS!
        //
        static void Main(string[] args)
        {
            // List of message that will be selected at random when the CPU is hammered!
            List<string> cpuMaxedoutMessages = new List<string>();
            cpuMaxedoutMessages.Add("WARNING: Holy crap your CPU is about to catch fire!");
            cpuMaxedoutMessages.Add("WARNING: oh my god you should not run your CPU that hard");
            cpuMaxedoutMessages.Add("WARNING: Stop downloading porn, it's maxing me out");
            cpuMaxedoutMessages.Add("WARNING: Your CPU is officially chasing squirrels");
            cpuMaxedoutMessages.Add("WARNING: RED ALERT! RED ALERT! RED ALERT! RED ALERT!");

            // The dice for list of messages
            Random rand = new Random();



            // This will greet the user in a the default voice
            sysnth.Speak("Welcome to Jarvis version one point Oh!");

            #region My Performance Counters
            // This will pull the current CPU load in percentage
            PerformanceCounter perfCPUCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCPUCount.NextValue();

            // This will pull the current available memory in Megabytes
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();

            // This will pull the system uptime (in seconds)
            PerformanceCounter perfUptimeCount = new PerformanceCounter("System", "System Up Time");
            perfUptimeCount.NextValue();
            #endregion

            TimeSpan uptimeSpan = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
            string systemUptimeMessage = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds",
                (int)uptimeSpan.TotalDays,
                (int)uptimeSpan.Hours,
                (int)uptimeSpan.Minutes,
                (int)uptimeSpan.Seconds
                );

            // Tell the user what the current system uptime is
            //sysnth.Speak(systemUptimeMessage);
            JerrySpeak(systemUptimeMessage, VoiceGender.Male, 2);

            int speechSpeed = 1;
            int rate = 1;
            bool isChromeOpenedAlready = false;

            //Infinite While Loop
            while (true)
            {
                // Get the current performance counter values
                int currentCpuPercentage = (int)perfCPUCount.NextValue();
                int currentAvailableMemory = (int)perfMemCount.NextValue();

                // Every 1 second print the CPU load in percentage to the screen
                Console.WriteLine("CPU Load        : {0}%", currentCpuPercentage);
                Console.WriteLine("Available Memory: {0}MB", currentAvailableMemory);



                // Only tell us when the CPU is above 80% usage
                #region Logic
                if (currentCpuPercentage > 15)
                {
                    // This is designed to prevent the speech from getting too fast
                    if( currentCpuPercentage == 100)
                    {
                        if(speechSpeed < 5 )
                        {
                            speechSpeed++;
                        }
                        string cpuLoadVocalMessage = cpuMaxedoutMessages[rand.Next(5)];

                        if(isChromeOpenedAlready == false)
                        {
                            OpenWebsite("https://youtu.be/dQw4w9WgXcQ");
                            isChromeOpenedAlready = true;
                        }
                        JerrySpeak(cpuLoadVocalMessage, VoiceGender.Male, speechSpeed);
                    }
                    else
                    {
                        string cpuLoadVocalMessage = String.Format("The current CPU load is {0} percent", currentCpuPercentage);
                        JerrySpeak(cpuLoadVocalMessage, VoiceGender.Female, rate);
                    }
                }

                // Only tell us when memory is below one gigabyte
                if (currentAvailableMemory < 12500)
                {
                    string memAvailableVocalMessage = String.Format("You currently have {0} megabytes of memory available", currentAvailableMemory);
                    JerrySpeak(memAvailableVocalMessage, VoiceGender.Male, rate);
                }
                #endregion

                Thread.Sleep(1000);
            }// end of loop
        }

        /// <summary>
        ///  Speaks with a selected voice
        /// </summary>
        /// <param name="message"></param>
        /// <param name=""></param>
        public static void JerrySpeak(string message, VoiceGender voiceGender)
        {
            sysnth.SelectVoiceByHints(voiceGender);
            sysnth.Speak(message);
        }

        /// <summary>
        /// Speaks with a selected voice at a selected speed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        /// <param name="rate"></param>
        public static void JerrySpeak(string message, VoiceGender voiceGender, int rate)
        {
            sysnth.Rate = rate;
            JerrySpeak(message, voiceGender);
        }

        public static void OpenWebsite(string URL)
        {
            // Open a website
            Process p1 = new Process();
            p1.StartInfo.FileName = "chrome.exe";
            p1.StartInfo.Arguments = URL;
            p1.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p1.Start();
        }
    }
}
