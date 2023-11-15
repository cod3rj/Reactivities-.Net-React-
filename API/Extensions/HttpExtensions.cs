using System.Text.Json;

namespace API.Extensions
{
    // Extension methods for handling HTTP responses
    public static class HttpExtensions
    {
        /* 
            Adds pagination information to the HTTP response headers
            Parameters:
          - currentPage: The current page number in the paginated results.
          - itemsPerPage: The number of items to be displayed per page.
          - totalItems: The total number of items available.
          - totalPages: The total number of pages based on the pagination settings.
        */
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            // Create an anonymous object to represent the pagination information
            var paginationHeader = new
            {
                currentPage,
                itemsPerPage,
                totalItems,
                totalPages
            };

            // Serialize the pagination information and add it to the response headers with the key "Pagination"
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));

            // Ensure that the "Pagination" header is exposed to the client for proper CORS handling
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
