import { observer } from "mobx-react-lite";
import { useStore } from "../../app/stores/store";
import { Card, Grid, Header, Tab } from "semantic-ui-react";
import ProfileCard from "./ProfileCard";

// Using MobX observer to automatically re-render the component on store changes
export default observer(function ProfileFollowings() {
    // Access the profileStore from the global MobX store
    const { profileStore } = useStore();
    // Destructure properties from the profileStore
    const { profile, loadingFollowings, followings, activeTab } = profileStore;

    // Render a Tab.Pane with followings information
    return (
        <Tab.Pane loading={loadingFollowings}>
            <Grid>
                {/* Header section */}
                <Grid.Column width={16}>
                    {/* Display header based on the activeTab */}
                    <Header
                        floated='left'
                        icon='user'
                        content={activeTab === 3 ? `People Following ${profile?.displayName}` : `People ${profile?.displayName} Following`}
                    />
                </Grid.Column>
                {/* Content section */}
                <Grid.Column width={16}>
                    {/* Display a Card.Group with ProfileCards for each following profile */}
                    <Card.Group itemsPerRow={4}>
                        {followings.map(profile => (
                            <ProfileCard key={profile.username} profile={profile} />
                        ))}
                    </Card.Group>
                </Grid.Column>
            </Grid>
        </Tab.Pane>
    );
});
