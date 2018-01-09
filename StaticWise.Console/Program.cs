﻿using StaticWise.Compiler;
using StaticWise.Entities;
using StaticWise.Compiler.Utilities.Logger;
using System;
using System.Collections.Generic;
using StaticWise.Common.Files;
using StaticWise.Common.Deserialize;
using StaticWise.Common.Queries;
using System.Diagnostics;
using StaticWise.Common.Urls;

namespace StaticWise
{
    class Program
    {
        #region Properties

        // Initialize a message to introduce the application to the user
        static string _welcome = @" _____ _        _   _      _    _ _
/  ___| |      | | (_)    | |  | (_)
\ `--.| |_ __ _| |_ _  ___| |  | |_ ___  ___ 
 `--. \ __/ _` | __| |/ __| |/\| | / __|/ _ \
/\__/ / || (_| | |_| | (__\  /\  / \__ \  __/
\____/ \__\__,_|\__|_|\___|\/  \/|_|___/\___|

Welcome to StaticWise. An open source static blog generator built using .NET and C#.
For more information, please visit this project on GitHub at https://github.com/stevenmclintock/staticwise.";

        // Initialize the required manager objects
        static IFileManager _fileManager = new FileManager();
        static IDeserializeManager _deserializeManager = new DeserializeManager(_fileManager);
        static IQueryManager _queryManager = new QueryManager(_deserializeManager, _fileManager);
        static IUrlManager _urlManager = new UrlManager();

        // Initialize a logger object
        static ILogger _log = new Logger();

        #endregion

        #region Methods

        static void Main(string[] args)
        {
            // Output a welcome message to the console window
            Console.WriteLine(_welcome);

            // Initialize an empty configuration object
            Config config = new Config();

            do
            {
                // Prompt the user to enter the path to the JSON configuration file
                Console.WriteLine("\nPlease enter the path of your JSON configuration file:");
                string pathToConfig = Console.ReadLine();

                // Deserialize the JSON file to a configuration object
                if (!string.IsNullOrEmpty(pathToConfig))
                    config = _deserializeManager.DeserializeConfig(pathToConfig);

                // Determine if the configuration object is validate or not
                if (!string.IsNullOrWhiteSpace(config.FilePath))
                    Console.WriteLine($"\nThe JSON configuration file for \"{config.Title}\" is ready to use.");
                else
                    Console.WriteLine("\nUnable to open the JSON configuration file. Please try again.");
            }
            while (string.IsNullOrWhiteSpace(config.FilePath));

            // Prompt the user to build the blog using the configuration object
            Console.WriteLine($"Press the ENTER key to build the static blog.");

            // Exit the console application if the ENTER key was not read
            if (!Console.ReadKey().Key.Equals(ConsoleKey.Enter)) Environment.Exit(0);

            // Start a timer
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                // Initialize the compile object and build the blog using the configuration object
                Compile compile = new Compile(config, _log, _fileManager, _queryManager, _urlManager);
                compile.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

            // Stop the timer
            sw.Stop();

            // Get the log entries generated by the compile object
            List<LogEntry> logEntries = _log.GetEntries();

            // Output the log entries to the console window
            Console.WriteLine("\n=============\nLog Entries\n=============\n");
            if (logEntries != null) logEntries.ForEach(x => Console.WriteLine($"{x.Status.ToString()}: {x.Message}"));

            // Output the time elapsed to the console window
            Console.WriteLine($"Time elapsed: {sw.Elapsed}\n\nPress any key to continue...");
            Console.Read();
        }

        #endregion
    }
}