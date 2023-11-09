import {ServerError} from "../models/serverError";
import {makeAutoObservable, reaction} from "mobx";

export default class CommonStore {
    // This is where we define the state of our application e.g token, error, etc
    error: ServerError | null = null;
    token: string | null | undefined = localStorage.getItem('jwt'); // This will get the token from the local storage
    appLoaded = false;

    // This is the constructor that will make all properties observable
    constructor() {
        makeAutoObservable(this); // This will make all properties observable

        // This will run the code inside the callback function when the token changes
        reaction(
            () => this.token,
            token => {
                if (token) {
                    localStorage.setItem('jwt', token) // This will set the token in the local storage
                } else {
                    localStorage.removeItem('jwt') // This will remove the token from the local storage
                }
            }
        )
    }

    setServerError(error: ServerError) {
        this.error = error;
    }

    setToken (token: string | null) {
        this.token = token; // This will set the token in the state
    }

    setAppLoaded = () => {
        this.appLoaded = true;
    }
}