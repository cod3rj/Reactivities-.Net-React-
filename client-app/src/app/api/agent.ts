import axios, {AxiosError, AxiosResponse} from "axios";
import {Activity, ActivityFormValues} from "../models/activity";
import {toast} from "react-toastify";
import {router} from "../router/Routes";
import {store} from "../stores/store";
import {User, UserFormValues} from "../models/user";

// This function creates a delay using a Promise and setTimeout
const sleep = (delay: number) => {
    return new Promise((resolve) => {
        setTimeout(resolve, delay);
    });
}

// This will define the base URL for our API
axios.defaults.baseURL = 'http://localhost:5000/api';

// This function extracts the response data
const responseBody = <T>(response: AxiosResponse<T>) => response.data;

//
axios.interceptors.request.use(config => {
    const token = store.commonStore.token;
    if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;
    return config;
})

// This intercepts the response and adds a delay before resolving it
axios.interceptors.response.use(async response => {
    await sleep(1000);
    return response;
}, (error: AxiosError) => {
    // Handle various error cases and their associated responses
    const { data, status, config } = error.response as AxiosResponse;
    switch (status) {
        case 400:
            // Handle 400 status errors
            if (config.method === 'get' && data.errors.hasOwnProperty('id')) {
                router.navigate('/not-found');
            }
            if (data.errors) {
                // Handle errors in data response
                const modalStateErrors = [];
                for (const key in data.errors) {
                    if (data.errors[key]) {
                        modalStateErrors.push(data.errors[key]);
                    }
                }
                throw modalStateErrors.flat();
            } else {
                toast.error(data);
            }
            break;
        case 401:
            // Handle 401 status errors
            toast.error('unauthorized')
            break;
        case 403:
            // Handle 403 status errors
            toast.error('forbidden')
            break;
        case 404:
            // Handle 404 status errors
            router.navigate('/not-found');
            break;
        case 500:
            // Handle 500 status errors
            store.commonStore.setServerError(data);
            router.navigate('/server-error');
            break;
    }
    return Promise.reject(error);
})

// This is our request object that will contain all of our HTTP methods e.g get, post, put, delete
const request = {
    get: <T>(url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
}

// This will be the object that we will use to make requests to our API/Activities Controller
const Activities = {
    list: () => request.get<Activity[]>('/activities'),
    details: (id: string) => request.get<Activity>(`/activities/${id}`),
    create: (activity: ActivityFormValues) => request.post('/activities', activity),
    update: (activity: ActivityFormValues) => request.put(`/activities/${activity.id}`, activity),
    delete: (id: string) => request.del<void>(`/activities/${id}`),
    attend: (id: string) => request.post<void>(`/activities/${id}/attend`, {})
}

// This will be the object that we will use to make requests to our API/Account Controller
const Account = {
    current: () => request.get<User>('/account'),
    login: (user: UserFormValues) => request.post<User>('/account/login', user),
    register: (user: UserFormValues) => request.post<User>('/account/register', user),
}

const agent = {
    Activities,
    Account
}

export default agent;