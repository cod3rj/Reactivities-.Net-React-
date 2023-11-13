import { ChatComment } from "../models/comment";
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { makeAutoObservable, runInAction } from "mobx";
import { store } from "./store";

export default class CommentStore {
    // An array to store chat comments
    comments: ChatComment[] = [];

    // Connection to the SignalR hub
    hubConnection: HubConnection | null = null;

    // Constructor to make the class observable
    constructor() {
        makeAutoObservable(this);
    }

    // Create a connection to the SignalR hub for a specific activity
    createHubConnection = (activityId: string) => {
        // Check if the user is selected
        if (store.activityStore.selectedActivity) {
            // Create a new hub connection with necessary configurations
            this.hubConnection = new HubConnectionBuilder()
                .withUrl('http://localhost:5000/chat?activityId=' + activityId, {
                    accessTokenFactory: () => store.userStore.user?.token!
                })
                // Enable automatic reconnection in case of connection loss
                .withAutomaticReconnect()
                // Set the logging level to information for debugging purposes
                .configureLogging(LogLevel.Information)
                .build();

            // Start the hub connection
            this.hubConnection.start().catch(error => console.log('Error establishing connection: ', error));

            // Listen for the 'LoadComments' event from the hub
            this.hubConnection.on('LoadComments', (comments: ChatComment[]) => {
                runInAction(() => {
                    comments.forEach(comment => {
                        comment.createdAt = new Date(comment.createdAt + 'Z');
                    })
                    this.comments = comments
                });
            });

            // Listen for the 'ReceiveComment' event from the hub
            this.hubConnection.on('ReceiveComment', (comment: ChatComment) => {
                runInAction(() => {
                    comment.createdAt = new Date(comment.createdAt);
                    this.comments.unshift(comment)
                });
            });
        }
    }

    // Stop the SignalR hub connection
    stopHubConnection = () => {
        this.hubConnection?.stop().catch(error => console.log('Error stopping connection: ', error));
    }

    // Clear the stored comments and stop the hub connection
    clearComments = () => {
        this.comments = [];
        this.stopHubConnection();
    }

    // Add a comment to the SignalR hub
    addComment = async (values: { body: string, activityId?: string }) => {
        // Set the activityId in the comment values
        values.activityId = store.activityStore.selectedActivity?.id;
        try {
            // Invoke the 'SendComment' method on the hub with the comment values
            await this.hubConnection?.invoke('SendComment', values);
        } catch (error) {
            console.log(error);
        }
    }

}
