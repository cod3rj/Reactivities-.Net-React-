import {observer} from "mobx-react-lite";
import {useStore} from "../../stores/store";
import {Modal} from "semantic-ui-react";

export default observer(function ModalContainer() {
    const {modalStore} = useStore();
    const {modal, closeModal} = modalStore;

    return(
        <Modal open={modal.open} onClose={closeModal} size='mini'>
            <Modal.Content>
                {modal.body}
            </Modal.Content>
        </Modal>
    )
})