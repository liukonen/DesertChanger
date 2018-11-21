using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesertChangerMain
{
    class Shared
    {
        public static string InputBox(string Prompt, string Title, string DefaultResponse)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(Prompt, Title, DefaultResponse);
        }

    }
}
