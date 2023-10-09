import { FontIcon, getTheme, IconButton, IIconProps, IStackStyles, mergeStyles, Stack, Text } from '@fluentui/react';
import React, { FC, ReactElement } from 'react';
import { useAuth0 } from "@auth0/auth0-react";

const theme = getTheme();

const logoStyles: IStackStyles = {
    root: {
        width: '300px',
        background: theme.palette.themePrimary,
        alignItems: 'center',
        padding: '0 20px'
    }
}

const logoIconClass = mergeStyles({
    fontSize: 20,
    paddingRight: 10
});

const toolStackClass: IStackStyles = {
    root: {
        alignItems: 'center',
        height: 48,
        paddingRight: 10
    }
}

const iconProps: IIconProps = {
    styles: {
        root: {
            fontSize: 16,
            color: theme.palette.white
        }
    }
}

const Header: FC = (): ReactElement => {
    const { loginWithRedirect, logout, user, isAuthenticated, isLoading } = useAuth0();
    return (
        <Stack horizontal>
            <Stack horizontal styles={logoStyles}>
                <FontIcon aria-label="Check" iconName="SkypeCircleCheck" className={logoIconClass} />
                <Text variant="xLarge">ToDo</Text>
            </Stack>
            <Stack.Item grow={1}>
                <div></div>
            </Stack.Item>
            <Stack.Item>
                <Stack horizontal styles={toolStackClass} grow={1}>
                    <IconButton aria-label="Add" iconProps={{ iconName: "Settings", ...iconProps }} />
                    <IconButton aria-label="Add" iconProps={{ iconName: "Help", ...iconProps }} />
                    
                    {isAuthenticated && user && (
                    <Stack horizontal>
                        <img src={user.picture} alt={user.name} />
                        <h2>{user.name}</h2>
                        <p>{user.email}</p>
                        <button onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}> Log Out </button>
                    </Stack>)
                    
                    }

                    {isLoading && (<div>Loading...</div>) }

                    {!isLoading && !isAuthenticated && 
                        (<button onClick={() => loginWithRedirect()}>Log In</button>)}

                    {/* <Persona size={PersonaSize.size24} text="Sample User" /> */}
                    {/* <Toggle label="Dark Mode" inlineLabel styles={{ root: { marginBottom: 0 } }} onChange={changeTheme} /> */}
                </Stack>
            </Stack.Item>
        </Stack>
    );
}

export default Header;