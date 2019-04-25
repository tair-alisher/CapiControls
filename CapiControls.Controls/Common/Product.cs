namespace CapiControls.Controls.Common
{
    public class Product
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string[] Units { get; set; }

        public Product(string code, string name, string[] units)
        {
            Code = code;
            Name = name;
            Units = units;
        }
    }
}
