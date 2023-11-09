import {User, UserFormValues} from "../models/user";
import {makeAutoObservable, runInAction} from "mobx";
import agent from "../api/agent";
import {store} from "./store";
import {router} from "../router/Routes";

export default class UserStore {
    user: User | null = null;

    constructor() {
        makeAutoObservable(this);
    }

    get isLoggedIn () {
        return !!this.user;
    }

    login = async (creds: UserFormValues) => {
        const user = await agent.Account.login(creds); // This will call the login method in the agent.ts file and pass the creds
        store.commonStore.setToken(user.token); // This will set the token in the local storage
        runInAction(() => this.user = user); // This will set the user in the state
        router.navigate('/activities'); // This will redirect the user to the activities page
        store.modalStore.closeModal(); // This will close the modal after the user has logged in
    }

    register = async (creds: UserFormValues) => {
        const user = await agent.Account.register(creds); // This will call the register method in the agent.ts file and pass the creds
        store.commonStore.setToken(user.token); // this will set the token in the local storage
        runInAction(() => this.user = user); // This will set the user in the state
        router.navigate('/activities'); // This will redirect the user to the activities page after registering
        store.modalStore.closeModal(); // This will close the modal after the user has registered
    }

    logout = async () => {
        store.commonStore.setToken(null); // This will remove the token from the local storage
        this.user = null; // This will remove the token from the state
        router.navigate('/'); // This will redirect the user to the home page
    }

    getUser = async () => {
        try {
            const user = await agent.Account.current(); // This will call the current method in the agent.ts file
            runInAction(() => this.user = user); // This will set the user in the state
        } catch (error) {
            console.log(error);
        }
    }
}