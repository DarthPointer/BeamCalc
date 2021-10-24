using BeamCalc.Project;

namespace BeamCalc.Operation
{
    abstract class AbstractProjectElementOperation<TElement> : AbstractElementOperation<ProjectData, TElement>
    {
        public AbstractProjectElementOperation() : base() { }

        protected override bool TryGetElementHolder(out ProjectData result)
        {
            return Program.TryGetActiveProject(out result);
        }
    }
}
