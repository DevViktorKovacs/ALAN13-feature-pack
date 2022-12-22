using Godot;
using System;
using System.Diagnostics;


namespace ALAN13featurepack.Utility
{
    public static class DebugHelper
    {

        public static void Print(object message)
        {
            PrettyPrint(GetTimeStamp() + message, ConsoleColor.White);
        }

        public static void NewLine()
        {
            Console.WriteLine(string.Empty);
        }

        public static void PrettyPrint(string message, ConsoleColor consoleColor)
        {
            var defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = consoleColor;


            Console.Write(System.Environment.NewLine + message);
            Console.ForegroundColor = defaultColor;
        }

        public static void PrettyPrintVerbose(object message, ConsoleColor consoleColor = ConsoleColor.White)
        {
            if (InputProcessor.IsInDebugMode && InputProcessor.VerboseLogging)
            {
                PrettyPrint(message, consoleColor);
            }
        }

        public static void UpdatePrint(string message, ConsoleColor color = ConsoleColor.DarkGray)
        {
            var defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = color;

            Console.Write($"\r                                                                                         ");
            Console.Write($"\r{message}");
            Console.ForegroundColor = defaultColor;
        }

        public static void PrettyPrint(object message, ConsoleColor consoleColor)
        {
            PrettyPrint(message.ToString(), consoleColor);
        }

        public static void PrintStackTrace()
        {

            StackTrace stackTrace = new StackTrace();

            StackFrame[] stackFrames = stackTrace.GetFrames();

            foreach (StackFrame stackFrame in stackFrames)
            {
                GD.Print(stackFrame.ToString());
            }
        }

        public static void LogStep()
        {
            StackTrace stackTrace = new StackTrace();

            StackFrame[] stackFrames = stackTrace.GetFrames();

            var lastCall = stackFrames[1].GetMethod();

            PrettyPrint($"{GetTimeStamp()}{lastCall.DeclaringType}.{lastCall.Name} has been called. (line:{stackFrames[1].GetFileName()})", ConsoleColor.DarkGray);

            Print(System.Environment.StackTrace);
        }

        public static void PrintError(string message)
        {
            PrettyPrint(message, ConsoleColor.Red);
        }

        public static void PrintTree(Node node)
        {
            var parent = node.GetParent();

            var ancestorString = parent == null ? "" : parent.GetType().ToString() + ".";


            foreach (var child in node.GetChildren())
            {
                var item = (child as Node);
                if (item != null) PrintTree(item);
            }

        }

        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString($"yyyy/MM/dd hh:mm:ss.fff tt ");
        }
    }
}
