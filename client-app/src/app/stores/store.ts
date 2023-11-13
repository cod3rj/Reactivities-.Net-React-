import ActivityStore from "./activityStore";
import {createContext, useContext} from "react";
import CommonStore from "./commonStore";
import UserStore from "./userStore";
import ModalStore from "./modalStore";
import ProfileStore from "./ProfileStore";
import CommentStore from "./commentStore";

// This interface will define the properties of our store
interface Store {
    activityStore: ActivityStore;
    commonStore: CommonStore;
    userStore: UserStore;
    modalStore: ModalStore;
    profileStore: ProfileStore;
    commentStore: CommentStore;
}

// store is an object that will hold all the stores
export const store: Store = {
    activityStore: new ActivityStore(),
    commonStore: new CommonStore(),
    userStore: new UserStore(),
    modalStore: new ModalStore(),
    profileStore: new ProfileStore(),
    commentStore: new CommentStore(),
}

// Initializes with the store object, making it available to all the components in our application
export const StoreContext = createContext(store);

// This is a custom hook that enable components to consume the context of the store by using useContext
export function useStore() {
    return useContext(StoreContext);
}