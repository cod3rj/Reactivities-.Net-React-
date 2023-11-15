namespace Application.Core
{
    // Represents parameters for pagination
    public class PagingParams
    {
        // Maximum allowed page size
        private const int MaxPageSize = 50;

        // Current page number, default is 1
        public int PageNumber { get; set; } = 1;

        // Private field to hold the page size, with a default value of 10
        private int _pageSize = 10;

        // Public property to access the page size
        // If the page size is greater than the maximum allowed, set it to the maximum allowed page size
        public int PageSize
        {
            get => _pageSize; // Get the page size
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; // If the page size is greater than the maximum page size, set it to the maximum page size
        }
    }
}
