using TM.Common.Models.Interfaces;

namespace TM.Common.Models.Bases
{
    public class ActivatableEntity<TId> : BaseEntity<TId>,  IActivatableByUser where TId : struct
    {
        public string ModifiedBy { get; set; }
        public DateTime DateTimeModified { get; set; }
        public bool IsActive { get; set; }
    }
}
 