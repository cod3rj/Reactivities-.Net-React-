import { Tab } from "semantic-ui-react";
import ProfilePhotos from "./ProfilePhotos";
import { Profile } from "../../app/models/profile";
import { observer } from "mobx-react-lite";
import ProfileAbout from "./ProfileAbout";
import ProfileFollowings from "./ProfileFollowings";
import { useStore } from "../../app/stores/store";

interface Props {
    profile: Profile;
}

// Using MobX observer to automatically re-render the component on store changes
export default observer(function ProfileContent({ profile }: Props) {
    // Access the profileStore from the global MobX store
    const { profileStore } = useStore();

    // Define panes for the Tab component, each with a menuItem and corresponding render function
    const panes = [
        { menuItem: 'About', render: () => <ProfileAbout /> },
        { menuItem: 'Photos', render: () => <ProfilePhotos profile={profile} /> },
        { menuItem: 'Events', render: () => <Tab.Pane>Events Content</Tab.Pane> },
        { menuItem: 'Followers', render: () => <ProfileFollowings /> },
        { menuItem: 'Following', render: () => <ProfileFollowings /> },
    ];

    // Render a Tab component with vertical menu, panes, and an event handler for tab changes
    return (
        <Tab
            menu={{ fluid: true, vertical: true }} // Set menu properties
            menuPosition='right' // Set menu position
            panes={panes} // Set panes for the Tab
            onTabChange={(_, data) => profileStore.setActiveTab(data.activeIndex as number)} // Handle tab change
        />
    );
});
