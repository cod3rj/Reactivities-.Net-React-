import {Photo, Profile, UserActivity} from "../models/profile";
import {makeAutoObservable, reaction, runInAction} from "mobx";
import agent from "../api/agent";
import {store} from "./store";

export default class ProfileStore {
    // The profile being viewed
    profile: Profile | null = null;
    // Loading state for fetching the profile
    loadingProfile = false;
    // Loading state for uploading a photo
    uploading = false;
    // Loading state for various operations
    loading = false;
    // List of profiles that the current user is following or followers
    followings: Profile[] = [];
    // Loading state for fetching followings or followers
    loadingFollowings = false;
    // Active tab index, indicating which tab is currently selected
    activeTab = 0;
    // List of activities for the current user
    userActivities: UserActivity[] = [];
    // Loading state for fetching user activities
    loadingActivities = false;

    constructor() {
        makeAutoObservable(this);

        // Reaction to changes in the active tab
        reaction(
            () => this.activeTab,
            activeTab => {
                if (activeTab === 3 || activeTab === 4) {
                    // If on the 'Followers' or 'Following' tab, load the corresponding profiles
                    const predicate = activeTab === 3 ? 'followers' : 'following';
                    this.loadFollowings(predicate);
                } else {
                    // Reset the followings list if on other tabs
                    this.followings = [];
                }
            }
        );
    }

    // Set the active tab
    setActiveTab = (activeTab: number) => {
        this.activeTab = activeTab;
    }

    // Check if the current user is the owner of the profile being viewed
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
            // Fetch the profile from the API
            const profile = await agent.Profiles.get(username);
            runInAction(() => {
                this.profile = profile;
                this.loadingProfile = false;
            });
        } catch (error) {
            console.log(error);
            runInAction(() => this.loadingProfile = false);
        }
    }

    // Upload a photo
    uploadPhoto = async (file: Blob) => {
        this.uploading = true;
        try {
            // Upload the photo to the API
            const response = await agent.Profiles.uploadPhoto(file);
            const photo = response.data;
            runInAction(() => {
                // Update the profile's photo list and set the new main photo
                if (this.profile) {
                    this.profile.photos?.push(photo);
                    if (photo.isMain && store.userStore.user) {
                        store.userStore.setImage(photo.url);
                        this.profile.image = photo.url;
                    }
                }
                this.uploading = false;
            });
        } catch (error) {
            console.log(error);
            runInAction(() => this.uploading = false);
        }
    }

    // Set the main photo
    setMainPhoto = async (photo: Photo) => {
        this.loading = true;
        try {
            // Set the photo as the main photo in the API
            await agent.Profiles.setMainPhoto(photo.id);

            // Set the photo as the main photo in the state
            store.userStore.setImage(photo.url);

            runInAction(() => {
                // If the profile exists, update the main photo in the state
                if (this.profile && this.profile.photos) {
                    // Set the current main photo to false
                    this.profile.photos.find(p => p.isMain)!.isMain = false;
                    // Set the new main photo to true
                    this.profile.photos.find(p => p.id === photo.id)!.isMain = true;
                    // Set the new main photo as the profile image
                    this.profile.image = photo.url;
                    // Set the loading state to false
                    this.loading = false;
                }
            });
        } catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    // Delete the photo from the profile
    deletePhoto = async (photo: Photo) => {
        this.loading = true;
        try {
            await agent.Profiles.deletePhoto(photo.id); // Delete the photo from the API
            runInAction(() => {
                if (this.profile && this.profile.photos) {
                    // Filter out the deleted photo from the profile's photo list
                    this.profile.photos = this.profile.photos.filter(p => p.id !== photo.id);
                    this.loading = false; // Set the loading state to false
                }
            });
        } catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    // Update the profile information
    updateProfile = async (profile: Partial<Profile>) => {
        this.loading = true;
        try {
            await agent.Profiles.updateProfile(profile); // Update the profile in the API
            runInAction(() => {
                if (profile.displayName !== store.userStore.user?.displayName) {
                    // Update the user's display name in the state if it has changed
                    store.userStore.setDisplayName(profile.displayName!);
                }
                // Update the profile in the state with the new information
                this.profile = { ...this.profile, ...profile as Profile };
                this.loading = false; // Set the loading state to false
            });
        } catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    // Update the following status for a user
    updateFollowing = async (username: string, following: boolean) => {
        this.loading = true;
        try {
            await agent.Profiles.updateFollowing(username); // Update the following status in the API
            store.activityStore.updateAttendeeFollowing(username); // Update the following status in the state
            runInAction(() => {
                if (this.profile && this.profile.username !== store.userStore.user?.userName && this.profile.username === username) {
                    // Update the followers count and following status in the profile being viewed
                    following ? this.profile.followersCount++ : this.profile.followersCount--;
                    this.profile.following = !this.profile.following;
                }
                if (this.profile && this.profile.username === store.userStore.user?.userName) {
                    // Update the following count in the user's own profile
                    following ? this.profile.followingCount++ : this.profile.followingCount--;
                }
                this.followings.forEach(profile => {
                    // Update the followers count and following status in the list of followings
                    if (profile.username === username) {
                        profile.following ? profile.followersCount-- : profile.followersCount++;
                        profile.following = !profile.following;
                    }
                });
                this.loading = false; // Set the loading state to false
            });
        } catch (error) {
            console.log(error);
            runInAction(() => this.loading = false);
        }
    }

    // Load the list of followings for the current user
    loadFollowings = async (predicate: string) => {
        this.loadingFollowings = true;
        try {
            const followings = await agent.Profiles.listFollowings(this.profile!.username, predicate);
            runInAction(() => {
                // Set the list of followings in the state
                this.followings = followings;
                this.loadingFollowings = false; // Set the loading state to false
            });
        } catch (error) {
            console.log(error);
            runInAction(() => this.loadingFollowings = false);
        }
    }

    // Load the list of activities for the current user
    loadUserActivities = async (username: string, predicate?: string) => {
        this.loadingActivities = true;
        try {
            const activities = await agent.Profiles.listActivities(username, predicate!);
            runInAction(() => {
                // Set the list of user activities in the state
                this.userActivities = activities;
                this.loadingActivities = false; // Set the loading state to false
            });
        } catch (error) {
            console.log(error);
            runInAction(() => this.loadingActivities = false);
        }
    }
}