
namespace TM.Common.Models.Interfaces
{
    public interface IModifiableByUser : IModifiable
    {
        public string ModifiedBy { get; set; }
    }
}
