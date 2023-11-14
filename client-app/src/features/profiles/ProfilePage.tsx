import { Grid } from "semantic-ui-react";
import ProfileHeader from "./ProfileHeader";
import ProfileContent from "./ProfileContent";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import { useStore } from "../../app/stores/store";
import { useEffect } from "react";
import LoadingComponent from "../../app/layout/LoadingComponent";

// Using MobX observer to automatically re-render the component on store changes
export default observer(function ProfilePage() {
    // Extract the 'username' parameter from the URL using the 'useParams' hook
    const { username } = useParams<{ username: string }>();

    // Access the profileStore from the global MobX store
    const { profileStore } = useStore();

    // Destructure properties from the profileStore
    const { loadingProfile, loadProfile, profile, setActiveTab } = profileStore;

    // Use the 'useEffect' hook to load the profile when the component mounts or when the 'username' changes
    useEffect(() => {
        // Check if 'username' is defined and load the profile
        if (username) loadProfile(username);

        // Clean up by setting the active tab to 0 when the component is unmounted
        return () => {
            setActiveTab(0);
        };
    }, [loadProfile, username, setActiveTab]);

    // If the profile is still loading, display a loading component
    if (loadingProfile) return <LoadingComponent content='Loading profile...' />;

    // Render the profile page with header and content components
    return (
        <Grid>
            <Grid.Column width={16}>
                {/* Check if the profile is defined before rendering */}
                {profile &&
                    <>
                        {/* Render the ProfileHeader component with the profile */}
                        <ProfileHeader profile={profile} />
                        {/* Render the ProfileContent component with the profile */}
                        <ProfileContent profile={profile} />
                    </>
                }
            </Grid.Column>
        </Grid>
    );
});
