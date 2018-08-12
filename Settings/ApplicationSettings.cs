using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uzgoto.DotNetSnipet.Settings.Messages;

namespace Uzgoto.DotNetSnipet.Settings
{
    public class ApplicationSettings : ConfigurationSection
    {
        [ConfigurationProperty("messages")]
        [ConfigurationCollection(typeof(MessageCollection))]
        public MessageCollection Messages => this["messages"] as MessageCollection;
    }
}
