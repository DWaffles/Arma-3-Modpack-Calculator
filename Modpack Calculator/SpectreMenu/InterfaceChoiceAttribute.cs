namespace ModpackCalculator.SpectreMenu
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class InterfaceChoiceAttribute : Attribute
    {
        public string ChoiceGroup { get; }
        public string ChoiceName { get; }
        public InterfaceChoiceAttribute(string choiceGroup, string choiceName)
        {
            ChoiceGroup = choiceGroup;
            ChoiceName = choiceName;
        }
    }
}
