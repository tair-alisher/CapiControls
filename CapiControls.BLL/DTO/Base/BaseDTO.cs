namespace CapiControls.BLL.DTO.Base
{
    public class BaseDTO
    {
    }

    public class BaseDTO<T> : BaseDTO
    {
        public T Id { get; set; }
    }
}
