using System;
using System.Collections.Generic;

namespace BeamCalc
{
    static class StringLib
    {
        public static string TakeArg(this List<string> args)
        {
            string result = args[0];
            args.RemoveAt(0);

            return result;
        }

        public static List<string> Parse(this string command)       // Don't spend time reading the code
        {
            List<string> result = new List<string>();

            const char escapeChar = '\\';
            const char spacedArgBracket = '"';
            const char spaceArgSeparator = ' ';

            string buffer = "";
            bool isEscSeq = false;
            bool insideSpacedArgBrackets = false;
            char lastChar = (char)0;

            foreach (char c in command)
            {
                lastChar = c;
                if (!isEscSeq)
                {
                    switch (c)
                    {
                        case escapeChar:
                            isEscSeq = true;
                            break;


                        case spacedArgBracket:
                            insideSpacedArgBrackets = !insideSpacedArgBrackets;
                            if (!insideSpacedArgBrackets)
                            {
                                result.Add(buffer);
                                buffer = "";
                            }
                            break;


                        case spaceArgSeparator:
                            if (!insideSpacedArgBrackets)
                            {
                                if (buffer != "")
                                {
                                    result.Add(buffer);
                                    buffer = "";
                                }
                            }
                            else
                            {
                                buffer += c;
                            }
                            break;


                        default:
                            buffer += c;
                            break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case escapeChar:
                            buffer += c;
                            isEscSeq = false;
                            break;

                        case spacedArgBracket:
                            buffer += c;
                            isEscSeq = false;
                            break;

                        default:
                            throw new FormatException($"Escape seq '{escapeChar}{c}' is not supported");
                    }
                }
            }

            if (lastChar != '"' && lastChar != ' ' && lastChar != 0) result.Add(buffer);


            return result;
        }

        public static void AddNote(this List<string> reports, string message)
        {
            reports.Add("NOT: " + message);
        }

        public static void AddWarning(this List<string> reports, string message)
        {
            reports.Add("WRN: " + message);
        }

        public static void AddError(this List<string> reports, string message)
        {
            reports.Add("ERR: " + message);
        }
    }
}
