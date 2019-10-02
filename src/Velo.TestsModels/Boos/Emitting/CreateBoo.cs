using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class CreateBoo : ICommand
    {
        public int Id { get; set; }

        public int Int { get; set; }

        public bool Bool { get; set; }

        public bool StopPropagation { get; set; }
    }
}