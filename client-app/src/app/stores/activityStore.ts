import { makeAutoObservable, runInAction } from "mobx";
import { Activity } from "../models/activity";
import agent from "../api/agent";
import { v4 as uuid } from 'uuid';

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
    loadingInitial = true;

    // Represents the submitting state
    submitting = false;

    // Constructor to make all properties observable
    constructor() {
        makeAutoObservable(this);
    }

    // A computed property that returns activities sorted by date
    get activitiesByDate() {
        return Array.from(this.activityRegistry.values())
            .sort((a, b) => Date.parse(a.date) - Date.parse(b.date));
    }

    // Load the list of activities from the server
    loadActivities = async () => {
        try {
            const activities = await agent.Activities.list();
            activities.forEach(activity => {
                activity.date = activity.date.split('T')[0];
                this.activityRegistry.set(activity.id, activity);
            })
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    }

    // Set the loading initial state
    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    // Select an activity based on the provided ID
    selectActivity = (id: string) => {
        this.selectedActivity = this.activityRegistry.get(id);
    }

    // Clear the currently selected activity
    cancelSelectedActivity = () => {
        this.selectedActivity = undefined;
    }

    // Open the form for a new activity or to edit an existing one
    openForm = (id?: string) => {
        id ? this.selectActivity(id) : this.cancelSelectedActivity();
        this.setEditMode(true);
    }

    // Close the form
    closeForm = () => {
        this.setEditMode(false);
    }

    // Set the edit mode
    setEditMode = (state: boolean) => {
        this.editMode = state;
    }

    // Create a new activity
    createActivity = async (activity: Activity) => {
        this.loading = true;
        activity.id = uuid();
        try {
            await agent.Activities.create(activity);
            runInAction(() => {
                this.activityRegistry.set(activity.id, activity);
                this.selectedActivity = activity;
                this.setEditMode(false);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    // Update an existing activity
    updateActivity = async (activity: Activity) => {
        this.loading = true;
        try {
            await agent.Activities.update(activity);
            runInAction(() => {
                this.activityRegistry.set(activity.id, activity);
                this.selectedActivity = activity;
                this.setEditMode(false);
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }

    // Delete an activity by ID
    deleteActivity = async (id: string) => {
        this.loading = true;
        try {
            await agent.Activities.delete(id);
            runInAction(() => {
                this.activityRegistry.delete(id);
                if (this.selectedActivity?.id === id) this.cancelSelectedActivity();
                this.loading = false;
            })
        } catch (error) {
            console.log(error);
            runInAction(() => {
                this.loading = false;
            })
        }
    }
}
