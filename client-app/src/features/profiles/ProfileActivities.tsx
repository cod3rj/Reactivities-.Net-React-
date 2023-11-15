import {observer} from "mobx-react-lite";
import {Button, Card, Grid, Header, Image, Tab, TabProps} from "semantic-ui-react";
import {useStore} from "../../app/stores/store";
import {SyntheticEvent} from "react";
import {UserActivity} from "../../app/models/profile";
import {Link} from "react-router-dom";
import {format} from "date-fns";

// Observer function for the ProfileActivities component
export default observer(function ProfileActivities() {
    // Access the profileStore from the MobX store
    const {profileStore} = useStore();
    const {loadUserActivities, profile, loadingActivities, userActivities} = profileStore;

    // Define panes for different activity views
    const panes = [
        {menuItem: 'Future Events', pane: {key: 'future'}},
        {menuItem: 'Past Events', pane: {key: 'past'}},
        {menuItem: 'Hosting', pane: {key: 'hosting'}},
    ];

    // Handle tab change event
    const handleTabChange = (e: SyntheticEvent, data: TabProps) => {
        // Load user activities based on the selected tab
        loadUserActivities(profile!.username, panes[data.activeIndex as number].pane.key);
    };

    // Render the ProfileActivities component
    return (
        <Tab.Pane>
            <Grid>
                <Grid.Column width={16}>
                    <Header floated='left' icon='calendar' content='Activities'/>
                </Grid.Column>
                <Grid.Column width={16}>
                    {/* Display tabs for different activity views */}
                    <Tab
                        panes={panes}
                        menu={{secondary:true, pointing: true}}
                        onTabChange={(e, data) => handleTabChange(e, data)}
                    />

                    {/* Display user activities in Card.Group */}
                    <Card.Group itemsPerRow={4}>
                        {userActivities.map((activity: UserActivity) => (
                            <Card
                                as={Link}
                                to={`/activities/${activity.id}`}
                                key={activity.id}
                            >
                                {/* Display activity image with category-specific placeholder */}
                                <Image
                                    src={`/assets/categoryImages/${activity.category}.jpg`}
                                    style={{minHeight: 100, objectFit: 'cover'}}
                                />
                                <Card.Content>
                                    {/* Display activity details */}
                                    <Card.Header textAlign='center'>{activity.title}</Card.Header>
                                    <Card.Meta textAlign='center'>
                                        <div>{format(new Date(activity.date), 'do LLL')}</div>
                                        <div>{format(new Date(activity.date), 'h:mm:a')}</div>
                                    </Card.Meta>
                                </Card.Content>
                            </Card>
                        ))}
                    </Card.Group>
                </Grid.Column>
            </Grid>
        </Tab.Pane>
    );
});
