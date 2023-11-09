import {observer} from "mobx-react-lite";
import {useStore} from "../../app/stores/store";
import {ErrorMessage, Formik} from "formik";
import {Button, Form, Header} from "semantic-ui-react";
import MyTextInput from "../../app/common/form/MyTextInput";
import * as Yup from 'yup';
import ValidationError from "../errors/ValidationError";

export default observer(function RegisterForm() {
    const {userStore} = useStore();

    // Used to initialize the form
    const initialValues = {
        displayName: '',
        userName: '',
        email: '',
        password: '',
        error: null
    }

    // Used to validate the form
    const validationSchema = Yup.object({
        displayName: Yup.string().required(),
        userName: Yup.string().required(),
        email: Yup.string().required(),
        password: Yup.string().required(),
    })

    return (
        <Formik initialValues={initialValues} validationSchema={validationSchema}
                onSubmit={(values, {setErrors}) => userStore.register(values)
                    .catch(error => setErrors({error}))}
        >
            {({handleSubmit, isSubmitting, errors, isValid, dirty}) => (
                <Form className='ui form error' onSubmit={handleSubmit} autoComplete='off'>
                    <Header as='h2' content='Sign Up to Reactivities' color='teal' textAlign='center'/>
                    <MyTextInput placeholder='Display Name' name='displayName'/>
                    <MyTextInput placeholder='Username' name='userName'/>
                    <MyTextInput placeholder='Email' name='email'/>
                    <MyTextInput placeholder='Password' name='password' type='password'/>
                    <ErrorMessage name='error' render={() =>
                        <ValidationError errors={errors.error as unknown as string[]}/>}/>
                    <Button
                        disabled={!isValid || !dirty || isSubmitting}
                        loading={isSubmitting}
                        content='Sign Up'
                        ype='submit'
                        fluid/>
                </Form>
            )}

        </Formik>
    )
})