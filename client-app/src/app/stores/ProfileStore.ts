import {Photo, Profile} from "../models/profile";
import {makeAutoObservable, reaction, runInAction} from "mobx";
import agent from "../api/agent";
import {store} from "./store";

export default class ProfileStore {
    profile: Profile | null = null;
    loadingProfile = false;
    uploading = false;
    loading = false;
    followings: Profile[] = [];
    loadingFollowings = false;
    activeTab = 0;

    constructor() {
        makeAutoObservable(this);

        reaction(
            () => this.activeTab,
            activeTab => {
                if (activeTab === 3 || activeTab === 4) {
                    const predicate = activeTab === 3 ? 'followers' : 'following';
                    this.loadFollowings(predicate);
                }else {
                    this.followings = [];
                }
            }
        )
    }

    // This will set the active tab
    setActiveTab = (activeTab: number) => {
        this.activeTab = activeTab;
    }

    // This will return true if the current user is the user whose profile is being viewed
    get isCurrentUser() {
        if (store.userStore.user && this.profile) {
            return store.userStore.user.userName === this.profile.username;
        }
        return false;
    }

    // Load the profile
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

    // Upload a photo
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

    // Set the main photo
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

    // Delete the photo
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

    // Update the profile
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

    // Update following
    updateFollowing = async (username: string, following: boolean) => {
        this.loading = true;
        try {
            await agent.Profiles.updateFollowing(username); // This will update the following in the API
            store.activityStore.updateAttendeeFollowing(username); // This will update the following in the state
            runInAction(() => {
                // If the profile username is not equal to the username in the state
                // & the profile username is not equal to the username, update the following in the state
                if (this.profile && this.profile.username !== store.userStore.user?.userName && this.profile.username === username) {
                    following ? this.profile.followersCount++ : this.profile.followersCount--; // This will update the followers count in the state
                    this.profile.following = !this.profile.following; // This will update the following in the state
                }
                // If the profile username is equal to the username in the state, update the following in the state
                if(this.profile && this.profile.username === store.userStore.user?.userName) {
                    following ? this.profile.followingCount++ : this.profile.followingCount--; // This will update the following count in the state
                }
                this.followings.forEach(profile => {
                    // If the profile username is equal to the username, update the following in the state
                    if (profile.username === username) {
                        profile.following ? profile.followersCount-- : profile.followersCount++; // This will update the followers count in the state
                        profile.following = !profile.following; // This will update the following in the state
                    }
                })
                this.loading = false; // This will set the loading state to false
            })
        }catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    // Load followings
    loadFollowings = async (predicate: string) => {
        this.loadingFollowings = true;
        try {
            const followings = await agent.Profiles.listFollowings(this.profile!.username, predicate); // This will return a list of profiles
            runInAction(() => {
                this.followings = followings; // This will set the followings in the state
                this.loadingFollowings = false; // This will set the loading state to false
            })
        }catch (error) {
            console.log(error);
            runInAction(() => this.loadingFollowings = false);
        }
    }
}