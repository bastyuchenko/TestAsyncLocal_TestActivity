using Serilog;

namespace ColorExtension
{
    public static class LoggerExtensions
    {
        public const string BackgroundBlack = "\u001b[40m";
        public const string BackgroundRed = "\u001b[41m";
        public const string BackgroundGreen = "\u001b[42m";
        public const string BackgroundYellow = "\u001b[43m";
        public const string BackgroundBlue = "\u001b[44m";
        public const string BackgroundMagenta = "\u001b[45m";
        public const string BackgroundCyan = "\u001b[46m";
        public const string BackgroundWhite = "\u001b[47m";
        public const string BackgroundBrightBlack = "\u001b[40;2m";
        public const string BackgroundBrightRed = "\u001b[41;2m";
        public const string BackgroundBrightGreen = "\u001b[42;2m";
        public const string BackgroundBrightYellow = "\u001b[43;2m";
        public const string BackgroundBrightBlue = "\u001b[44;2m";
        public const string BackgroundBrightMagenta = "\u001b[45;2m";
        public const string BackgroundBrightCyan = "\u001b[46;2m";
        public const string BackgroundBrightWhite = "\u001b[47;2m";

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
        public static void BkColor(
          this ILogger logger,
          string messageTemplate,
          params object[] args)
        {
            // Get the color they chose
            string CurrentColor = (string)args[args.GetLength(0) - 1];

            // Get rid of the color parameter now as it will break the Serilog parser
            args[args.GetLength(0) - 1] = "";

            // Prepend our color code to every argument (tested with strings and numbers)
            for (int i = 0; i < args.GetLength(0); i++)
            {
                args[i] = CurrentColor + args[i];
            }

            // Find all the arguments looking for the close bracket
            List<int> indexes = messageTemplate.AllIndexesOf("}");
            int iterations = 0;
            // rebuild messageTemplate with our color-coded arguments
            // Note: we have to increase the index on each iteration based on the previous insertion of
            // a color code
            foreach (var i in indexes)
            {
                messageTemplate = messageTemplate.Insert(i + 1 + (iterations++ * CurrentColor.Length), CurrentColor);
            }

            // Prefix the entire message template with color code
            string bkg = CurrentColor + messageTemplate;

            // Log it with a context
            logger.ForContext("IsImportant", true)
              .Information(bkg, args);
        }
    }
}