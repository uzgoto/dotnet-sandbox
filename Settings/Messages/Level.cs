using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.Settings.Messages
{
    public enum Level
    {
        Info,
        Warn,
        Error,
    }

    public static class LevelUtilities
    {
        public static Level GetLevel(string code)
        {
            switch (code.Substring(0, 1))
            {
                case "E":
                    return Level.Error;
                case "W":
                    return Level.Warn;
                case "I":
                    return Level.Info;
                default:
                    throw new ArgumentException($"{code} hss undefined prefix.", nameof(code));
            }
        }
    }

    public static class LevelExtensions
    { 
        public static (MessageBoxButtons Buttons, MessageBoxIcon Icon) GetMessageBoxComponent(this Level level)
        {
            switch (level)
            {
                case Level.Info:
                    return (MessageBoxButtons.OK, MessageBoxIcon.Information);
                case Level.Warn:
                    return (MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                case Level.Error:
                    return (MessageBoxButtons.OK, MessageBoxIcon.Error);
                default:
                    throw new ArgumentException($"{level} is not defined.", nameof(level));
            }
        }
    }
}
