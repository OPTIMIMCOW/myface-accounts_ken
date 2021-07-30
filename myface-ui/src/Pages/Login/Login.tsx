import React, { FormEvent, useContext, useState } from 'react';
import { Page } from "../Page/Page";
import { LoginContext } from "../../Components/LoginManager/LoginManager";
import { basicAuth } from "../../Api/apiClient";
import { Redirect } from "react-router-dom";
import "./Login.scss";

function prepareBasicAuth(userName: string, password: string): string {
    const encodedAuth = Buffer.from(`${userName}:${password}`).toString('base64');
    return `Basic ${encodedAuth}`;
}

export function Login(): JSX.Element {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const loginContext = useContext(LoginContext);

    function tryLogin(event: FormEvent) {
        event.preventDefault();
        const encodedUserPass = prepareBasicAuth(username, password);
        basicAuth(encodedUserPass).catch(() => { return <Login/>} );
        
        loginContext.logIn(encodedUserPass);
    }

    return (
        <Page containerClassName="login">
            <h1 className="title">Log In</h1>
            <form className="login-form" onSubmit={tryLogin}>
                <label className="form-label">
                    Username
                    <input className="form-input" type={"text"} value={username} onChange={event => setUsername(event.target.value)} />
                </label>

                <label className="form-label">
                    Password
                    <input className="form-input" type={"password"} value={password} onChange={event => setPassword(event.target.value)} />
                </label>

                <button className="submit-button" type="submit">Log In</button>
            </form>
        </Page>
    );
}