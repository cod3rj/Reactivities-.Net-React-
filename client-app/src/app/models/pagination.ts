// Interface representing pagination information
export interface Pagination {
    currentPage: number;   // Current page number
    itemsPerPage: number;  // Number of items to display per page
    totalItems: number;    // Total number of items in the dataset
    totalPages: number;    // Total number of pages based on itemsPerPage and totalItems
}

// Generic class representing a paginated result with data and pagination information
export class PaginatedResult<T> {
    data: T;               // The actual data for the current page
    pagination: Pagination; // Pagination information for the data

    // Constructor to initialize data and pagination
    constructor(data: T, pagination: Pagination) {
        this.data = data;
        this.pagination = pagination;
    }
}

//
export class PagingParams {
    pageNumber;
    pageSize;

    constructor(pageNumber = 1, pageSize = 10) {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
    }
}