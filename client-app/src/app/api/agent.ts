import axios, {AxiosError, AxiosResponse} from "axios";
import {Activity, ActivityFormValues} from "../models/activity";
import {toast} from "react-toastify";
import {router} from "../router/Routes";
import {store} from "../stores/store";
import {User, UserFormValues} from "../models/user";
import {Profile, UserActivity} from "../models/profile";
import {PaginatedResult} from "../models/pagination";

// Function to create a delay using a Promise and setTimeout
const sleep = (delay: number) => {
    return new Promise((resolve) => {
        // Use setTimeout to simulate a delay before resolving the Promise.
        setTimeout(resolve, delay);
    });
}

// Set the base URL for our API
axios.defaults.baseURL = 'http://localhost:5000/api';

// Function to extract the response data from an AxiosResponse
const responseBody = <T>(response: AxiosResponse<T>) => response.data;

// Axios request interceptor to add authorization header to outgoing requests
axios.interceptors.request.use(config => {
    // Retrieve the authentication token from the common store
    const token = store.commonStore.token;

    // If a token is available and the request has headers, add the Authorization header.
    if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;

    // Return the modified request configuration.
    return config;
})

// This intercepts the response and introduces a simulated delay to emulate real-world network conditions.
axios.interceptors.response.use(async response => {
    // Simulate a delay of 1 second to provide a more realistic user experience.
    await sleep(1000);

    // Check if the response headers contain information about pagination.
    const pagination = response.headers['pagination'];

    // If pagination information is present, structure the response data into a PaginatedResult.
    if (pagination) {
        response.data = new PaginatedResult(response.data, JSON.parse(pagination));
        // Return the modified response as an AxiosResponse of PaginatedResult.
        return response as AxiosResponse<PaginatedResult<any>>;
    }

    // If there's no pagination information, return the response as is.
    return response;
}, (error: AxiosError) => {
    // Destructure relevant information from the error response.
    const { data, status, config } = error.response as AxiosResponse;

    // Switch statement to handle different HTTP status codes.
    switch (status) {
        case 400:
            // Handle 400 status errors
            // If it's a 'get' request with a missing id, navigate to the 'not-found' page.
            if (config.method === 'get' && data.errors.hasOwnProperty('id')) {
                router.navigate('/not-found');
            }
            // If there are errors in the data response, extract and throw them.
            if (data.errors) {
                const modalStateErrors = [];
                for (const key in data.errors) {
                    if (data.errors[key]) {
                        modalStateErrors.push(data.errors[key]);
                    }
                }
                throw modalStateErrors.flat();
            } else {
                // Otherwise, display a generic error message.
                toast.error(data);
            }
            break;
        case 401:
            // Handle 401 status errors
            // Display an 'unauthorized' error message.
            toast.error('unauthorized')
            break;
        case 403:
            // Handle 403 status errors
            // Display a 'forbidden' error message.
            toast.error('forbidden')
            break;
        case 404:
            // Handle 404 status errors
            // Navigate to the 'not-found' page.
            router.navigate('/not-found');
            break;
        case 500:
            // Handle 500 status errors
            // Set the server error in the common store and navigate to the 'server-error' page.
            store.commonStore.setServerError(data);
            router.navigate('/server-error');
            break;
    }

    // Reject the promise with the error to propagate it to the calling code.
    return Promise.reject(error);
})

// Request object containing all HTTP methods (get, post, put, delete)
const request = {
    // HTTP GET method
    get: <T>(url: string) => axios.get<T>(url).then(responseBody),
    // HTTP POST method
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    // HTTP PUT method
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    // HTTP DELETE method
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

// Object for making requests to the Activities Controller in the API
const Activities = {
    // Get a paginated list of activities
    list: (params: URLSearchParams) => {
        // Make an HTTP GET request using Axios
        return axios.get<PaginatedResult<Activity[]>>('/activities', {
            params, // Include URL parameters in the request
        })
            // Handle the response using the responseBody function
            .then(responseBody);
    },
    // Get details of a specific activity by ID
    details: (id: string) => request.get<Activity>(`/activities/${id}`),
    // Create a new activity
    create: (activity: ActivityFormValues) => request.post('/activities', activity),
    // Update an existing activity
    update: (activity: ActivityFormValues) => request.put(`/activities/${activity.id}`, activity),
    // Delete an activity by ID
    delete: (id: string) => request.del<void>(`/activities/${id}`),
    // Attend an activity by ID
    attend: (id: string) => request.post<void>(`/activities/${id}/attend`, {}),
}

// Object for making requests to the Account Controller in the API
const Account = {
    // Get the current user's account details
    current: () => request.get<User>('/account'),
    // Log in with user credentials
    login: (user: UserFormValues) => request.post<User>('/account/login', user),
    // Register a new user account
    register: (user: UserFormValues) => request.post<User>('/account/register', user),
}

// Object for making requests to the Profiles Controller in the API
const Profiles = {
    // Get profile details by username
    get: (username: string) => request.get<Profile>(`/profiles/${username}`),
    // Upload a photo to the user's profile
    uploadPhoto: (file: Blob) => {
        let formData = new FormData(); // FormData is a built-in JS class for handling form data
        formData.append('File', file); // Append the file to the form data
        return axios.post('photos', formData, {
            headers: {'Content-Type': 'multipart/form-data'} // Set the content type for file upload
        })
    },
    // Set a photo as the main photo
    setMainPhoto: (id: string) => request.post(`/photos/${id}/setMain`, {}),
    // Delete a photo by ID
    deletePhoto: (id: string) => request.del(`/photos/${id}`),
    // Update the user's profile details
    updateProfile: (profile: Partial<Profile>) => request.put(`/profiles`, profile),
    // Update the following status for a user
    updateFollowing: (username: string) => request.post(`/follow/${username}`, {}),
    // Get a list of profiles that a user is following or being followed by
    listFollowings: (username: string, predicate: string) => request.get<Profile[]>(`/follow/${username}?predicate=${predicate}`),
    // Get a list of activities that a user is attending
    listActivities: (username: string, predicate: string) => request.get<UserActivity[]>(`/profiles/${username}/activities?predicate=${predicate}`),
}

const agent = {
    Activities,
    Account,
    Profiles
}

export default agent;