import { Profile } from "../../app/models/profile";
import { observer } from "mobx-react-lite";
import { Button, Reveal } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { SyntheticEvent } from "react";

interface Props {
    profile: Profile;
}

// Using MobX observer to automatically re-render the component on store changes
export default observer(function FollowButton({ profile }: Props) {
    // Access the profileStore and userStore from the global MobX store
    const { profileStore, userStore } = useStore();
    // Destructure properties from the profileStore
    const { updateFollowing, loading } = profileStore;

    // If the logged-in user is viewing their own profile, hide the FollowButton
    if (userStore.user?.userName === profile.username) return null;

    // Handle the follow/unfollow button click
    function handleFollow(e: SyntheticEvent, username: string) {
        e.preventDefault();
        // Toggle the following status based on the current state
        profile.following ? updateFollowing(username, false) : updateFollowing(username, true);
    }

    // Render a button with a reveal effect to show follow/unfollow status
    return (
        <Reveal animated='move'>
            {/* Visible content */}
            <Reveal.Content visible style={{ width: '100%' }}>
                <Button
                    fluid
                    color='teal'
                    content={profile.following ? 'Following' : 'Not following'}
                />
            </Reveal.Content>
            {/* Hidden content */}
            <Reveal.Content hidden style={{ width: '100%' }}>
                <Button
                    basic
                    fluid
                    // Change button color based on follow status
                    color={profile.following ? 'red' : 'green'}
                    // Change button label based on follow status
                    content={profile.following ? 'Unfollow' : 'Follow'}
                    // Show loading indicator while the follow/unfollow operation is in progress
                    loading={loading}
                    // Handle button click
                    onClick={(e) => handleFollow(e, profile.username)}
                />
            </Reveal.Content>
        </Reveal>
    );
});
