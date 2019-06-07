namespace CapiControls.Controls.Common
{
    internal class Product
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string GskpCode { get; set; }
        public string[] Units { get; set; }

        public Product(string code, string name, string gskpCode, string[] units)
        {
            Code = code;
            Name = name;
            GskpCode = gskpCode;
            Units = units;
        }
    }
}
