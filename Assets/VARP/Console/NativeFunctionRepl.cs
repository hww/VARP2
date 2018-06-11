﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VARP;
using Console = VARP.Console;
using System.IO;

public class NativeFunctionRepl : MonoBehaviour
{
    private static NativeFunctionRepl instance;

    public static NativeFunctionRepl Instance
    {
        get { return (instance!=null) ? instance : (instance = UiSingletone.FindSingletoneOfType<NativeFunctionRepl>()); }
    }


    private Mode replMode;

    private Buffer replBuffer;
    private KeyMap keyMap;
    private string prompt = "> ";

    private void OnEnable()
    {
        keyMap = new KeyMap();
        replMode = new Mode("repl", keyMap: keyMap);

        replBuffer = new Buffer("repl");
        replBuffer.EnabeMajorMode(replMode);
        replBuffer.EnabeMinorMode(ReadLine.Instance);
        replBuffer.Enable();

        ReadLine.Instance.AutoCompletionListener += AutoCompletionHandler;
        // define some functions
        NativeFunction.Define("help", Help, "Display short help for commands.");
        NativeFunction.Define("man", Man, "Display manual for the command. It works only if 'man' contains manual for given command.");
        ReadLine.Instance.Read(prompt, Evaluate);

        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string  stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                Console.ResetColor();
                Console.WriteLine("ERRO " + logString);
                break;
            case LogType.Assert:
                Console.SetColor(Color.red);
                Console.WriteLine("ASRT " + logString);
                break;
            case LogType.Warning:
                Console.WriteLine("WARN " + logString);
                break;
            case LogType.Log:
                Console.SetColor(Color.yellow);
                Console.WriteLine("INFO " + logString);
                break;
            case LogType.Exception:
                Console.SetColor(Color.red);
                Console.WriteLine("ERRO " + logString);
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
        Console.ResetColor();
    }


    private string[] AutoCompletionHandler(string text, int caretPosition)
    {
        var start = text.Substring(0, caretPosition);
        var list = new List<string>();
        foreach (var nf in NativeFunction.AllFunctions)
        {
            if (nf.Key.StartsWith(start))
                list.Add(nf.Key);
        }
        return list.ToArray();
    }

    public object Evaluate(string text)
    {
        object result = null;
        var args = text.Split(' ');
        var cmd = args[0];
        var fun = NativeFunction.TryLockup(cmd);
        if (fun != null && fun.function != null)
        {
            result = fun.Call(args);
        }
        else
        {
            Console.WriteLine(string.Format("The command '{0}' is not exists", text));
        }
        ReadLine.Instance.Read(prompt, Evaluate);
        return result;
    }

    public void OnEnterTab(string text)
    {

    }

    private object Help([NotNull] params object[] args)
    {
        if (args == null) throw new ArgumentNullException("args");
        if (args.Length == 1)
        {
            Console.WriteLine("These shell commands are defined internally.");
            Console.WriteLine("Type `help' to see this list.");
            Console.WriteLine("Type `help name' to find out more about the function `name'.");
            Console.WriteLine("Type `help shell' to find out more about the shell in general.");
            Console.WriteLine("Optionaly: Type `man name' or `man shell' to see extended documentation.");
            Console.WriteLine();

            // Generic help
            var funcs = NativeFunction.GetNames();
            var lines = TerminalTableBuilder.BuildTable(funcs, Console.BufferWidth, 2);
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
        else if (args.Length >= 2)
        {
            var arg = args[1] as string;
            if (arg == null) throw new NullReferenceException("arg");

            // Help for exact function
            if (string.Equals(arg, "shell"))
            {
                Console.WriteLine("Short shell information.");
                Console.WriteLine("Use 'left' and right' arrow to change caret position.");
                Console.WriteLine("Use 'up' and 'down' arrow to navigate in the hostory.");
                Console.WriteLine("Use 'tab' to complete current word. Or press 'tab tab' to see the list of commands which starts with curent word");
                Console.WriteLine("Use 'enter' to execute whole expression.");
            }
            else
            {
                var fun = NativeFunction.TryLockup(arg);
                if (fun != null)
                {
                    if (fun.help != null)
                        Console.WriteLine(fun.help);
                    else
                        Console.WriteLine(string.Format("Command '{0}' does not have help", fun.name));
                }
                else
                {
                    Console.WriteLine(string.Format("Command '{0}' does not exists", arg));  
                }

            }
        }
        return null;
    }

    private object Man([NotNull] params object[] args)
    {
        if (args == null) throw new ArgumentNullException("args");
        Console.WriteLine("Optionaly: Type `info name' or `info shell' to see extended documentation.");
        if (args.Length >= 2)
        {
            var arg = args[1] as string;
            if (arg == null) throw new NullReferenceException("arg");

            var fun = NativeFunction.TryLockup(arg);
            if (fun != null)
            {
                var filename = Path.Combine(Application.streamingAssetsPath, "info");
                filename = Path.Combine(filename, arg);
                if (File.Exists(filename))
                    CatFileToConsole(filename);
                else
                    Console.WriteLine(string.Format("Command '{0}' does not have manual", arg));
            }
            else
            {
                Console.WriteLine(string.Format("Command '{0}' does not exists", fun.name));
            }
        }
        return null;
    }


    private bool CatFileToConsole(string fileName)
    {
        try
        {
            var theReader = new StreamReader(fileName, Encoding.Default);
            using (theReader)
            {
                var line = theReader.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    line = theReader.ReadLine();
                }
                theReader.Close();
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("{0}\n", e.Message);
            return false;
        }
    }
}

