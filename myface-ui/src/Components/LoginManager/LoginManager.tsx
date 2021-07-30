import React, { createContext, ReactNode, useState } from "react";

export const LoginContext = createContext({
    isLoggedIn: false,
    isAdmin: false,
    logIn: (encodedUserPass:string) => { },
    logOut: () => { },
    encodedUserPass: "",
});

interface LoginManagerProps {
    children: ReactNode
}

export function LoginManager(props: LoginManagerProps): JSX.Element {
    const [loggedIn, setLoggedIn] = useState(false);
    const [encodedUserPass, setEncodedUserPass] = useState("");

    function logIn(encodedUserPass: string) {
        setLoggedIn(true);
        setEncodedUserPass(encodedUserPass);
    }

    function logOut() {
        setLoggedIn(false);
        setEncodedUserPass("");
    }

    const context = {
        isLoggedIn: loggedIn,
        isAdmin: loggedIn,
        logIn: logIn,
        logOut: logOut,
        encodedUserPass: encodedUserPass,
    };

    return (
        <LoginContext.Provider value={context}>
            {props.children}
        </LoginContext.Provider>
    );
}