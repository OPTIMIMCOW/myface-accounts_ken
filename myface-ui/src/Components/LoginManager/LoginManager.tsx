import React, { createContext, ReactNode, useState } from "react";

export const UserDetailsContext = createContext({
    password: "",
    userName: "",
});

export const LoginContext = createContext({
    isLoggedIn: false,
    isAdmin: false,
    logIn: (user: string, pass: string) => { },
    logOut: () => { },
    userName: "",
    password: "",
});

interface LoginManagerProps {
    children: ReactNode
}

export function LoginManager(props: LoginManagerProps): JSX.Element {
    const [loggedIn, setLoggedIn] = useState(false);
    const [password, setPassword] = useState("");
    const [username, setUserName] = useState("");

    function logIn(username: string, password: string) {
        setLoggedIn(true);
        setPassword(password);
        setUserName(username);
    }

    function logOut() {
        setLoggedIn(false);
    }

    const context = {
        isLoggedIn: loggedIn,
        isAdmin: loggedIn,
        logIn: logIn,
        logOut: logOut,
        userName: username,
        password: password,
    };

    return (
        <LoginContext.Provider value={context}>
            {props.children}
        </LoginContext.Provider>
    );
}