import {Photo, Profile} from "../models/profile";
import {makeAutoObservable, runInAction} from "mobx";
import agent from "../api/agent";
import {store} from "./store";

export default class ProfileStore {
    profile: Profile | null = null;
    loadingProfile = false;
    uploading = false;
    loading = false;

    constructor() {
        makeAutoObservable(this);
    }

    //
    get isCurrentUser() {
        if (store.userStore.user && this.profile) {
            return store.userStore.user.userName === this.profile.username;
        }
        return false;
    }

    //
    loadProfile = async (username: string) => {
        this.loadingProfile = true;
        try {
            const profile = await agent.Profiles.get(username);
            runInAction(() => {
                this.profile = profile;
                this.loadingProfile = false;
            })
        }catch (error) {
            console.log(error);
            runInAction(() => this.loadingProfile = false);
        }
    }

    //
    uploadPhoto = async (file: Blob) => {
        this.uploading = true;
        try {
            const response = await agent.Profiles.uploadPhoto(file); // This will return a Photo object
            const photo = response.data; // This will be the Photo object
            runInAction(() => {
                if (this.profile) {
                    this.profile.photos?.push(photo);
                    if (photo.isMain && store.userStore.user) {
                        store.userStore.setImage(photo.url);
                        this.profile.image = photo.url;
                    }
                }
                this.uploading = false;
            })
        }catch (error) {
            console.log(error);
            runInAction(() => this.uploading = false);
        }
    }

    //
    setMainPhoto = async (photo: Photo) => {
        this.loading = true;
        try {
            await agent.Profiles.setMainPhoto(photo.id); // This will set the photo as the main photo in the API
            store.userStore.setImage(photo.url); // This will set the photo as the main photo in the state
            runInAction(() => {
                // If the profile exists, set the photo as the main photo in the state
                if (this.profile && this.profile.photos) {
                    this.profile.photos.find(p => p.isMain)!.isMain = false; // This will set the current main photo to false
                    this.profile.photos.find(p => p.id === photo.id)!.isMain = true; // This will set the new main photo to true
                    this.profile.image = photo.url; // This will set the new main photo as the profile image
                    this.loading = false; // This will set the loading state to false
                }
            })
        }catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    //
    deletePhoto = async (photo: Photo) => {
        this.loading = true;
        try {
            await agent.Profiles.deletePhoto(photo.id); // This will delete the photo from the API
            runInAction(() => {
                // If the profile exists, delete the photo from the state
                if(this.profile && this.profile.photos) {
                    this.profile.photos = this.profile.photos.filter(p => p.id !== photo.id); // This will filter out the photo that was deleted
                    this.loading = false; // This will set the loading state to false
                }
            })
        }catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    //
    updateProfile = async (profile: Partial<Profile>) => {
        this.loading = true;
        try {
            await agent.Profiles.updateProfile(profile); // This will update the profile in the API
            runInAction(() => {
                // If the profile display name is not equal to the display name in the state, update the display name in the state
                if (profile.displayName !== store.userStore.user?.displayName) {
                    store.userStore.setDisplayName(profile.displayName!); // This will update the display name in the state
                }
                this.profile = {...this.profile, ...profile as Profile}; // This will update the profile in the state
                this.loading = false; // This will set the loading state to false
            })
        }catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }
}