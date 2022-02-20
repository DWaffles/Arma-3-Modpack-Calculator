namespace ModpackCalculator.SpectreMenu
{
    internal static class ChoiceGroups
    {
        public const string SelectGroup = "Select";
        public const string CalculateGroup = "Calculate";
        public const string View = "View";
        public const string ExportGroup = "Export";
        public const string MenuGroup = "Menu";
        public static int GetGroupPriority(string group)
        {
            return group switch
            {
                SelectGroup => 1,
                CalculateGroup => 2,
                View => 3,
                ExportGroup => 4,
                MenuGroup => 5,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
