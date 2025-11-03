using System.Collections.Generic;

namespace BlazorApp.Models
{
    public class PagedResult<T>
    {
        public int Total { get; set; }
        public List<T> Customers { get; set; } = new();
    }
}