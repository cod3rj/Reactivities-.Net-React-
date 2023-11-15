import {makeAutoObservable, reaction, runInAction} from "mobx";
import { Activity, ActivityFormValues } from "../models/activity";
import agent from "../api/agent";
import { format } from "date-fns";
import { store } from "./store";
import { Profile } from "../models/profile";
import {Pagination, PagingParams} from "../models/pagination";

export default class ActivityStore {
    // An in-memory storage of activities
    activityRegistry = new Map<string, Activity>();
    // Represents the currently selected activity
    selectedActivity: Activity | undefined = undefined;
    // Tracks whether the edit mode is on or off
    editMode = false;
    // Represents the loading state
    loading = false;
    // Represents the initial loading state
    loadingInitial = false;
    // Represents pagination information, initially set to null
    pagination: Pagination | null = null;
    // Represents paging parameters, initialized with default values
    pagingParams = new PagingParams();
    // Represents the predicate used for filtering activities, initially set to include 'all'
    predicate = new Map().set('all', true);

    // Constructor to make all properties observable
    constructor() {
        // Automatically makes all properties observable
        makeAutoObservable(this);

        // Reaction to changes in the predicate keys
        reaction(
            // Observe changes in the keys of the predicate Map
            () => this.predicate.keys(),
            // Reaction to changes in predicate keys
            () => {
                // Reset paging parameters when the predicate changes
                this.pagingParams = new PagingParams();
                // Clear the activityRegistry and reload activities
                this.activityRegistry.clear();
                this.loadActivities();
            }
        );
    }

    // Set the paging parameters
    setPagingParams = (pagingParams: PagingParams) => {
        this.pagingParams = pagingParams;
    }

    // Method to set the predicate used for filtering activities
    setPredicate = (predicate: string, value: string | Date) => {
        // Function to reset all predicate keys except 'startDate'
        const resetPredicate = () => {
            this.predicate.forEach((value, key) => {
                if (key !== 'startDate') this.predicate.delete(key);
            });
        }

        // Switch statement to handle different predicate cases
        switch (predicate) {
            // When the predicate is 'all'
            case 'all':
                // Reset all predicate keys and set 'all' to true
                resetPredicate();
                this.predicate.set('all', true);
                break;

            // When the predicate is 'isGoing'
            case 'isGoing':
                // Reset all predicate keys and set 'isGoing' to true
                resetPredicate();
                this.predicate.set('isGoing', true);
                break;

            // When the predicate is 'isHost'
            case 'isHost':
                // Reset all predicate keys and set 'isHost' to true
                resetPredicate();
                this.predicate.set('isHost', true);
                break;

            // When the predicate is 'startDate'
            case 'startDate':
                // Delete the existing 'startDate' key and set it to the provided value
                this.predicate.delete('startDate');
                this.predicate.set('startDate', value);
                break;
        }
    }

    // Computed property to generate parameters for Axios requests
    get axiosParams() {
        // Create a new instance of URLSearchParams
        const params = new URLSearchParams();
        // Append 'pageNumber' parameter to the URLSearchParams
        params.append('pageNumber', this.pagingParams.pageNumber.toString());
        // Append 'pageSize' parameter to the URLSearchParams
        params.append('pageSize', this.pagingParams.pageNumber.toString());
        // Append 'startDate' parameter to the URLSearchParams
        this.predicate.forEach((value, key) => {
            // If the key is 'startDate', convert the value to ISO string
            if (key === 'startDate') {
                params.append(key, (value as Date).toISOString())
            } else {
                params.append(key, value);
            }
        })
        // Return the generated URLSearchParams
        return params;
    }

    // A computed property that returns activities sorted by date
    get activitiesByDate() {
        // Convert the activityRegistry values to an array and sort them by date
        return Array.from(this.activityRegistry.values())
            .sort((a, b) => a.date!.getTime() - b.date!.getTime());
    }

    // A computed property that returns activities grouped by date
    get groupedActivities() {
        return Object.entries(
            // Reduce the activitiesByDate array to a grouped object by date
            this.activitiesByDate.reduce((activities, activity) => {
                const date = format(activity.date!, 'dd MMM yyyy'); // Format the date
                // If the date key exists, add the activity to the existing array, else create a new array
                activities[date] = activities[date] ? [...activities[date], activity] : [activity];
                return activities;
            }, {} as { [key: string]: Activity[] })
        )
    }

    // Load the list of activities from the server
    loadActivities = async () => {
        this.setLoadingInitial(true);
        try {
            // Call the API to get a paginated list of activities
            const result = await agent.Activities.list(this.axiosParams);
            // Iterate over each activity in the data property of the result
            result.data.forEach(activity => {
                this.setActivity(activity); // Set each activity in the activityRegistry
            })
            this.setPagination(result.pagination); // Set the pagination details
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    }

    // Set the pagination
    setPagination = (pagination: Pagination) => {
        this.pagination = pagination;
    }

    // Load Individual activity from the server
    loadActivity = async (id: string) => {
        let activity = this.getActivity(id);
        if (activity) {
            this.selectedActivity = activity;
            return activity;
        } else {
            this.setLoadingInitial(true);
            try {
                activity = await agent.Activities.details(id);
                this.setActivity(activity);
                runInAction(() => this.selectedActivity = activity);
                this.setLoadingInitial(false);
                return activity;
            } catch (error) {
                console.log(error);
                this.setLoadingInitial(false);
            }
        }
    }

    // Set the Activity
    private setActivity = (activity: Activity) => {
        const user = store.userStore.user; // Get the current user
        /*
            This will check if the current user is in the list of attendees
            If the user is in the list of attendees then isGoing will be set to true
            then we will set the isHost property to true if the current user is the host
            and finally we will set the host property to the host of the activity
        */
        if (user) {
            activity.isGoing = activity.attendees!.some(
                a => a.username === user.userName
            )
            activity.isHost = activity.hostUsername === user.userName;
            activity.host = activity.attendees?.find(x => x.username === activity.hostUsername);
        }
        activity.date = new Date(activity.date!);
        this.activityRegistry.set(activity.id, activity);
    }

    // Gets the Id: Value | Undefined
    private getActivity = (id: string) => {
        return this.activityRegistry.get(id);
    }

    // Set the loading initial state
    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    // Set the edit mode
    setEditMode = (state: boolean) => {
        this.editMode = state;
    }

    // Create a new activity
    createActivity = async (activity: ActivityFormValues) => {
        const user = store.userStore.user;
        const attendee = new Profile(user!);
        try {
            await agent.Activities.create(activity);
            const newActivity = new Activity(activity);
            newActivity.hostUsername = user!.userName;
            newActivity.attendees = [ attendee ];
            this.setActivity(newActivity);
            runInAction(() => {
                this.selectedActivity = newActivity;
            })
        } catch (error) {
            console.log(error);
        }
    }

    // Update an existing activity
    updateActivity = async (activity: ActivityFormValues) => {
        try {
            await agent.Activities.update(activity);
            runInAction(() => {
                if (activity.id) {
                    const updatedActivity = { ...this.getActivity(activity.id), ...activity };
                    this.activityRegistry.set(activity.id, updatedActivity as Activity);
                    this.selectedActivity = updatedActivity as Activity;
                }
            })
        } catch (error) {
            console.log(error);
        }
    }

    // Delete an activity by ID
    deleteActivity = async (id: string) => {
        this.loading = true;
        try {
            await agent.Activities.delete(id);
            runInAction(() => {
                this.activityRegistry.delete(id);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    // Update attendance
    updateAttendance = async () => {
        const user = store.userStore.user;
        this.loading = true;
        try {
            await agent.Activities.attend(this.selectedActivity!.id);
            runInAction(() => {
                if (this.selectedActivity?.isGoing) {
                    this.selectedActivity.attendees = this.selectedActivity.attendees?.filter(
                        a => a.username !== user?.userName);
                    this.selectedActivity.isGoing = false;
                } else {
                    const attendee = new Profile(user!);
                    this.selectedActivity?.attendees?.push(attendee);
                    this.selectedActivity!.isGoing = true;
                }
                this.activityRegistry.set(this.selectedActivity!.id, this.selectedActivity!);
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    // Cancel Activity
    cancelActivityToggle = async () => {
        this.loading = true;
        try {
            await agent.Activities.attend(this.selectedActivity!.id);
            runInAction(() => {
                this.selectedActivity!.isCancelled = !this.selectedActivity?.isCancelled;
                this.activityRegistry.set(this.selectedActivity!.id, this.selectedActivity!);
            })
        } catch (error) {
            console.log(error);
        } finally {
            runInAction(() => this.loading = false);
        }
    }

    // Clear the selected activity from the store
    clearSelectedActivity = () => {
        this.selectedActivity = undefined;
    }

    // Update the attendees following
    updateAttendeeFollowing = (username: string) => {
        this.activityRegistry.forEach(activity => {
            activity.attendees.forEach(attendee => {
                if (attendee.username === username) {
                    attendee.following ? attendee.followersCount-- : attendee.followersCount++;
                    attendee.following = !attendee.following;
                }
            })
        })
    }
}
