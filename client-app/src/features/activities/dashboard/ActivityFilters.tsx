import React from "react";
import {Header, Menu} from "semantic-ui-react";
import {Calendar} from "react-calendar";
import {observer} from "mobx-react-lite";
import {useStore} from "../../../app/stores/store";

export default observer(function ActivityFilters() {
    // Destructuring assignment to get the 'predicate' and 'setPredicate' from the 'activityStore'
    const {activityStore: {predicate, setPredicate}} = useStore();

    return (
        <>
            {/* Vertical menu for activity filters */}
            <Menu vertical size='large' style={{width: '100%', marginTop: 28}}>
                {/* Header for the filter section */}
                <Header icon='filter' attached color='teal' content='Filters'/>

                {/* Menu item for displaying all activities */}
                <Menu.Item
                    content='All Activities'
                    active={predicate.has('all')}
                    onClick={() => setPredicate('all', 'true')}
                />

                {/* Menu item for displaying activities the user is going to */}
                <Menu.Item
                    content="I'm going"
                    active={predicate.has('isGoing')}
                    onClick={() => setPredicate('isGoing', 'true')}
                />

                {/* Menu item for displaying activities the user is hosting */}
                <Menu.Item
                    content="I'm hosting"
                    active={predicate.has('isHost')}
                    onClick={() => setPredicate('isHost', 'true')}
                />
            </Menu>

            {/* Empty Header component for spacing */}
            <Header/>

            {/* Calendar component for selecting start date filter */}
            <Calendar
                onChange={(date) => setPredicate('startDate', date as Date)}
                value={predicate.get('startDate') || new Date()}
            />
        </>
    )
})
