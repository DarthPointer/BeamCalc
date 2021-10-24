using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeamCalc.Project;

namespace BeamCalc.Operation
{
    abstract class AbstractElementOperation<TElementHolder, TElement> : AbstractOperation where TElementHolder : SavableProjectElement, new()
    {
        protected const string create = "Create";
        protected const string change = "Change";
        protected const string delete = "Delete";

        protected abstract string UserFreindlyElementName { get; }

        protected Dictionary<string, Action<TElementHolder, List<string>>> modes;
        protected Dictionary<string, Action<TElementHolder, string, List<string>>> changers;

        protected abstract bool TryGetElementHolder(out TElementHolder result);


        public AbstractElementOperation()
        {
            modes = new Dictionary<string, Action<TElementHolder, List<string>>>()
            {
                { create, Create },
                { change, Change },
                { delete, Delete }
            };
        }


        protected abstract Dictionary<string, TElement> GetElementsDictFromHolder(TElementHolder holder);

        protected abstract void Create(TElementHolder holder, List<string> args);
        protected abstract void Delete(TElementHolder holder, List<string> args);

        void Change(TElementHolder holder, List<string> args)
        {
            if (!MandatoryArgumentPresense(args, $"{UserFreindlyElementName} to change")) return;

            string existingElementName = args.TakeArg();

            if (GetElementsDictFromHolder(holder).ContainsKey(existingElementName))
            {
                if (!MandatoryArgumentPresense(args, "parameter to change")) return;
                string parameterToChange = args.TakeArg();

                if (changers.ContainsKey(parameterToChange))
                {
                    changers[parameterToChange](holder, existingElementName, args);
                    return;
                }
                else
                {
                    Program.AddError($"Unknown parameter to change specified: {parameterToChange}. Need {GenerateChangeParameterPatternText()}.");
                    return;
                }
            }
            else
            {
                Program.AddError($"\"{existingElementName}\" does not exist. Make sure you typed {UserFreindlyElementName} name correctly and this {UserFreindlyElementName} exists.");
                return;
            }
        }


        protected string GetUserFriendlyElementHolderNane()
        {
            return new TElementHolder().UserFriendlyName;
        }


        public override bool Execute(List<string> args)
        {
            args.TakeArg();

            if (TryGetElementHolder(out TElementHolder holder))
            {
                if (!MandatoryArgumentPresense(args, $"mode ({create}|{change}|{delete})")) return true;
                string mode = args.TakeArg();

                if (modes.ContainsKey(mode))
                {
                    modes[mode](holder, args);
                }
                else
                {
                    Program.AddError($"Unknown mode \"{mode}\".");
                    return true;
                }
            }
            else
            {
                Program.AddError($"No {GetUserFriendlyElementHolderNane()} opened.");
                return true;
            }

            return true;
        }

        string GenerateChangeParameterPatternText()
        {
            return string.Join('|', changers.Keys);
        }
    }
}
