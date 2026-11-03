namespace TM.Common.Models.Interfaces
{
    public interface IBaseEntity<TId> where TId : struct
    {
        public TId Id { get; }
    }
}
