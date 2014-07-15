using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Lin.Core.Markup
{
    public class RoutedCommand : MarkupExtension
    {
        public string Command { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Lin.Core.Commands.RoutedCommands.Commands[Command];
        }
    }
}
