using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace z3nApp
{
    public enum LogType
    {
        Info,
        Warning,
        Error
    }

    public enum LogColor
    {
        Default,
        Gray,
        Yellow,
        Orange,
        Red,
        Pink,
        Violet,
        Blue,
        LightBlue,
        Turquoise,
        Green
    }

    public class Logger
    {
        private readonly bool _logShow;
        private readonly string _emoji;

        public Logger(bool log = false, string classEmoji = null)
        {
            _logShow = log;
            _emoji = classEmoji;
        }

        public void Send(string toLog, [CallerMemberName] string callerName = "", bool show = false, bool thr0w = false, int cut = 0, bool wrap = true)
        {
            if (!show && !_logShow) return;
            string header = string.Empty;
            string body = toLog;

            if (wrap)
            {
                var stackFrame = new StackFrame(1);
                header = LogHeader(stackFrame, callerName);
                body = LogBody(toLog, cut);
            }
            string toSend = header + body;
            (LogType type, LogColor color) = LogColour(header, toLog);
            Execute(toSend, type, color, thr0w);
        }

        private string LogHeader(StackFrame stackFrame, string callerName)
        {
            string formated = string.Empty;

            try
            {
                Process currentProcess = Process.GetCurrentProcess();
                long processMemory = currentProcess.WorkingSet64 / 1024 / 1024;
                long managedMemory = GC.GetTotalMemory(true) / 1024 / 1024;
                formated += $" 🧠  [{processMemory}Mb | {managedMemory}Mb]";
            }
            catch { }

            try
            {
                var callingMethod = stackFrame.GetMethod();
                if (callingMethod == null || callingMethod.DeclaringType == null)
                    formated += $" 🔳  [Unknown]";
                else
                    formated += $" 🔲  [{callingMethod.DeclaringType.Name}.{callerName}]";
            }
            catch { }

            return formated ?? string.Empty;
        }

        private string LogBody(string toLog, int cut)
        {
            if (!string.IsNullOrEmpty(toLog))
            {
                if (cut != 0)
                {
                    int lineCount = toLog.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Length;
                    if (lineCount > cut) toLog = toLog.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
                }
                if (!string.IsNullOrEmpty(_emoji))
                {
                    toLog = $"[ {_emoji} ] {toLog}";
                }
                return $"\n          {toLog.Trim()}";
            }
            return string.Empty;
        }

        private (LogType, LogColor) LogColour(string header, string toLog)
        {
            LogType type = LogType.Info;
            LogColor color = LogColor.Default;

            var colorMap = new Dictionary<string, LogColor>
            {
                { "`.", LogColor.Default },
                { "`w", LogColor.Gray },
                { "`y", LogColor.Yellow },
                { "`o", LogColor.Orange },
                { "`r", LogColor.Red },
                { "`p", LogColor.Pink },
                { "`v", LogColor.Violet },
                { "`b", LogColor.Blue },
                { "`lb", LogColor.LightBlue },
                { "`t", LogColor.Turquoise },
                { "`g", LogColor.Green },
                { "!W", LogColor.Orange },
                { "!E", LogColor.Orange },
                { "relax", LogColor.LightBlue },
                { "Err", LogColor.Orange },
            };

            string combined = (header ?? "") + (toLog ?? "");
            foreach (var pair in colorMap)
            {
                if (combined.Contains(pair.Key))
                {
                    color = pair.Value;
                    break;
                }
            }

            if (combined.Contains("!W")) type = LogType.Warning;
            if (combined.Contains("!E")) type = LogType.Error;

            return (type, color);
        }

        private void Execute(string toSend, LogType type, LogColor color, bool thr0w)
        {
            string colorPrefix = MapLogColorToPrefix(color);
            Debug.WriteLine($"🔳{DateTime.Now:MM-dd HH:mm:ss}] {colorPrefix}{type}: {toSend}");

            if (thr0w) throw new Exception(toSend);
        }

        private string MapLogColorToPrefix(LogColor color)
        {
            return color switch
            {
                LogColor.Gray => "[Gray] ",
                LogColor.Yellow => "[Yellow] ",
                LogColor.Orange => "[Orange] ",
                LogColor.Red => "[Red] ",
                LogColor.Pink => "[Pink] ",
                LogColor.Violet => "[Violet] ",
                LogColor.Blue => "[Blue] ",
                LogColor.LightBlue => "[LightBlue] ",
                LogColor.Turquoise => "[Turquoise] ",
                LogColor.Green => "[Green] ",
                _ => ""
            };
        }
    }
}