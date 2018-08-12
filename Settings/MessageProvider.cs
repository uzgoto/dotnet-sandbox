using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uzgoto.DotNetSnipet.Settings.Messages;

namespace Uzgoto.DotNetSnipet.Settings
{
    class MessageProvider
    {
        public static IEnumerable<Message> Load()
        {
            var settings = ConfigurationManager.GetSection("applicationSettings") as ApplicationSettings;
            return settings?.Messages?.OfType<Message>() ?? Enumerable.Empty<Message>();
        }
    }

}