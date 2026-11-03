using System.ComponentModel.DataAnnotations;
using TM.Common.Models.Interfaces;

namespace TM.Common.Models.Bases
{
    public class AuditableEntity<TId> : ActivatableEntity<TId>, ICreatableByUser where TId : struct
    {
        [Required] public string CreatedBy { get; set; }
        [Required] public DateTime DateTimeCreated { get; set; } = DateTime.UtcNow;
    }
}
