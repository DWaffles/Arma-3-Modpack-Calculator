using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModpackCalculator.SpectreMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class InterfaceOptionAttribute : Attribute
    {
        public string ChoiceGroup { get; }
        public string ChoiceName { get; }
        public InterfaceOptionAttribute(string choiceGroup, string command)
        {
            ChoiceGroup = choiceGroup;
            ChoiceName = command;
        }
    }
}
