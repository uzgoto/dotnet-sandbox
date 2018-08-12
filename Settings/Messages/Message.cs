using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uzgoto.DotNetSnipet.Settings.Messages
{
    public class Message : ConfigurationElement, IMessage
    {
        [ConfigurationProperty("code", IsRequired = true, IsKey = true, DefaultValue = "0")]
        [RegexStringValidator(@"(?:0|[EWI][0-9]{4})")]
        public string Code
        {
            get => this["code"] as string;
            set => this["code"] = value;
        }
        [ConfigurationProperty("text", IsRequired = true)]
        [StringValidator(InvalidCharacters = "\"\\")]
        public string Text
        {
            get => this["text"] as string;
            set => this["text"] = value;
        }
        [ConfigurationProperty("title", IsRequired = true)]
        [StringValidator(InvalidCharacters = "\"\\", MaxLength = 30)]
        public string Title
        {
            get => this["title"] as string;
            set => this["title"] = value;
        }
        public Level Level => LevelUtilities.GetLevel(this.Code);
        public MessageBoxButtons Buttons => this.Level.GetMessageBoxComponent().Buttons;
        public MessageBoxIcon Icon => this.Level.GetMessageBoxComponent().Icon;
    }
}
