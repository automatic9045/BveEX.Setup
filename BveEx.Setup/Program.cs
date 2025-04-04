﻿using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BveEx.Setup
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            bool isInteractive = true;

            {
                RootCommand rootCommand = new RootCommand("BveEX をインストールします。");

                Command nonInteractiveCommand = new Command("non-interactive", "非対話モードで実行します。");
                nonInteractiveCommand.AddAlias("n");

                {
                    Option<string> bve6PathOption = new Option<string>("--bve6", "BveEX を適用する BVE Trainsim 6 のパスを指定します。");
                    bve6PathOption.AddAlias("-6");

                    Option<string> bve5PathOption = new Option<string>("--bve5", "BveEX を適用する BVE Trainsim 5 のパスを指定します。");
                    bve5PathOption.AddAlias("-5");

                    Option<string> scenarioDirectoryOption = new Option<string>("--scenario", "BveEX サンプルシナリオを配置するシナリオフォルダを指定します。");
                    scenarioDirectoryOption.AddAlias("-s");

                    Option<bool> installSdkOption = new Option<bool>("--sdk", "BveEX SDK をインストールします。")
                    {
                        Arity = ArgumentArity.Zero,
                    };
                    installSdkOption.AddAlias("-d");

                    nonInteractiveCommand.AddOption(bve6PathOption);
                    nonInteractiveCommand.AddOption(bve5PathOption);
                    nonInteractiveCommand.AddOption(scenarioDirectoryOption);
                    nonInteractiveCommand.AddOption(installSdkOption);

                    Option<bool> bve6PathForLogOption = CreateFlagOption("--bve6-for-log");
                    Option<bool> bve5PathForLogOption = CreateFlagOption("--bve5-for-log");
                    Option<bool> scenarioDirectoryForLogOption = CreateFlagOption("--scenario-for-log");

                    nonInteractiveCommand.AddOption(bve6PathForLogOption);
                    nonInteractiveCommand.AddOption(bve5PathForLogOption);
                    nonInteractiveCommand.AddOption(scenarioDirectoryForLogOption);

                    nonInteractiveCommand.SetHandler((bve6Path, bve5Path, scenarioDirectory, installSdk, isBve6PathForLog, isBve5PathForLog, isScenarioDirectoryForLog) =>
                    {
                        isInteractive = false;

                        TargetPath.Bve6Path.Value = CreateTarget(bve6Path, isBve6PathForLog);
                        TargetPath.Bve5Path.Value = CreateTarget(bve5Path, isBve5PathForLog);
                        TargetPath.ScenarioDirectory.Value = CreateTarget(scenarioDirectory, isScenarioDirectoryForLog);
                        TargetPath.InstallSdk.Value = installSdk;
                    }, bve6PathOption, bve5PathOption, scenarioDirectoryOption, installSdkOption, bve6PathForLogOption, bve5PathForLogOption, scenarioDirectoryForLogOption);


                    Option<bool> CreateFlagOption(string name)
                    {
                        return new Option<bool>(name)
                        {
                            IsHidden = true,
                            Arity = ArgumentArity.Zero,
                        };
                    }

                    InstallationTarget CreateTarget(string path, bool isForLog)
                    {
                        if (path is null) return InstallationTarget.NotIdentified;
                        if (path == string.Empty) return InstallationTarget.NotIdentified;

                        InstallationTarget result = new InstallationTarget(path, isForLog);
                        return result;
                    }
                }

                rootCommand.AddCommand(nonInteractiveCommand);
                rootCommand.Invoke(args);
            }

            Models.Navigator.Initialize(isInteractive);

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
