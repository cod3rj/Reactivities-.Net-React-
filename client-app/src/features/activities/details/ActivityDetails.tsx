import React, {useEffect} from "react";
import {Button, Card, Grid, Image} from "semantic-ui-react";
import {useStore} from "../../../app/stores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import {Link, useParams} from "react-router-dom";
import {Simulate} from "react-dom/test-utils";
import {observer} from "mobx-react-lite";
import ActivityDetailedHeader from "./ActivityDetailedHeader";
import ActivityDetailedInfo from "./ActivityDetailedInfo";
import ActivityDetailedChat from "./ActivityDetailedChat";
import ActivityDetailedSidebar from "./ActivityDetailedSidebar";


export default observer(function ActivityDetails() {
    const {activityStore} = useStore();
    const {selectedActivity: activity, loadActivity,
        loadingInitial, clearSelectedActivity} = activityStore;
    const {id} = useParams();

    useEffect(() => {
        if (id) loadActivity(id);
        return () => clearSelectedActivity();
    }, [id, loadActivity, clearSelectedActivity]);

    if (loadingInitial || !activity) return <LoadingComponent/>;

    return (
        <Grid>
            <Grid.Column width={10}>
                <ActivityDetailedHeader activity={activity}/>
                <ActivityDetailedInfo activity={activity}/>
                <ActivityDetailedChat activityId={activity.id}/>
            </Grid.Column>
            <Grid.Column width={6}>
                <ActivityDetailedSidebar activity={activity}/>
            </Grid.Column>
        </Grid>
    )
})