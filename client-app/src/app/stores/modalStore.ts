import {makeAutoObservable} from "mobx";

interface Modal {
    open: boolean;
    body: JSX.Element | null;
}
export default class ModalStore {
    // This is where we define the Global States of our application
    modal: Modal = {
        open: false,
        body: null,
    }

    // This is the constructor that will make all properties observable
    constructor() {
        makeAutoObservable(this);
    }

    // This method will open the modal and set the body of the modal
    openModal = (content: JSX.Element) => {
        this.modal.open = true;
        this.modal.body = content;
    }

    // This method will close the modal and set the body of the modal to null
    closeModal = () => {
        this.modal.open = false;
        this.modal.body = null;
    }
}