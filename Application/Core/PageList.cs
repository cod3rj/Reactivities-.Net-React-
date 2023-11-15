using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
    // Represents a paginated list of items, inheriting from the List<T> class
    public class PageList<T> : List<T>
    {
        // Constructor for PageList
        // Initializes a new instance of the PageList class with the provided items, count, page number, and page size.
        public PageList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber; // Set the current page number
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); // Calculate the total number of pages using Math.Ceiling to round off the value
            PageSize = pageSize; // Set the page size
            TotalCount = count; // Set the total count of items
            AddRange(items); // Add the items to the list
        }

        // Properties to store pagination information
        // Gets or sets the current page number.
        public int CurrentPage { get; set; }
        // Gets or sets the total number of pages.
        public int TotalPages { get; set; }
        // Gets or sets the number of items on each page.
        public int PageSize { get; set; }
        // Gets or sets the total number of items.
        public int TotalCount { get; set; }

        // Method to create a paginated list asynchronously
        // Creates a new instance of the PageList class asynchronously based on the provided IQueryable source, page number, and page size.
        public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync(); // Get the total count of items in the source
            // Retrieve the items for the specified page
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Create and return a new PageList
            return new PageList<T>(items, count, pageNumber, pageSize);
        }
    }
}
