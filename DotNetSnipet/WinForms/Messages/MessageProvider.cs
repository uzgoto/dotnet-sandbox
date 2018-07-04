using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uzgoto.DotNetSnipet.WinForms.Messages
{
    class MessageProvider
    {
        public static IEnumerable<Message> Load()
        {
            var settings = ConfigurationManager.GetSection("applicationSettings") as ApplicationSettings;
            return settings?.Messages?.OfType<Message>() ?? Enumerable.Empty<Message>();
        }
    }

    public class ApplicationSettings : ConfigurationSection
    {
        [ConfigurationProperty("messages")]
        [ConfigurationCollection(typeof(MessageCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public MessageCollection Messages => this["messages"] as MessageCollection;
    }

    public class MessageCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Message();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as Message).Code;
        }
    }
}