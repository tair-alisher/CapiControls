using System.Collections.Generic;

namespace CapiControls.ViewModels
{
    public class IndexViewModel<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
