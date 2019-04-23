using System.Collections.Generic;

namespace CapiControls.Web.ViewModels
{
    public class ListViewModel<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
