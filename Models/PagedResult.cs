using System;
using System.Collections.Generic;

namespace LearnHubFO.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public int FirstItemIndex => (PageIndex - 1) * PageSize + 1;
        public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalItems);
    }
}
