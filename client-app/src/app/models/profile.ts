import {User} from "./user";

// Interface representing the structure of a user profile
export interface Profile {
    username: string;          // User's username
    displayName: string;       // User's display name
    image?: string;            // User's profile image URL (optional)
    bio?: string;              // User's biography or description (optional)
    followersCount: number;    // Number of followers the user has
    followingCount: number;    // Number of users the user is following
    following: boolean;        // Indicates whether the current user is following this profile
    photos?: Photo[];          // Array of photos associated with the user's profile (optional)
}

// Class implementation of the Profile interface
export class Profile implements Profile {
    constructor(user: User) {
        // Initializing the Profile properties based on the provided User object
        this.username = user.userName;         // Set username from the User object
        this.displayName = user.displayName;   // Set display name from the User object
        this.image = user.image;               // Set profile image URL from the User object
        this.followingCount = 0;               // Initialize following count to 0
        this.followersCount = 0;               // Initialize followers count to 0
        this.following = false;                // Initialize following status to false
    }
}

// Interface representing the structure of a photo associated with a user profile
export interface Photo {
    id: string;         // Unique identifier for the photo
    url: string;        // URL of the photo
    isMain: boolean;    // Indicates whether the photo is the main/profile photo
}