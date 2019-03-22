namespace CapiControls.Models
{
    public class BaseEntity<T> where T : class
    {
        T Id { get; set; }
    }
}
