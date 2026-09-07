import { createContext, useContext, useState, useEffect } from 'react'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null)
    const [token, setToken] = useState(null)
    const [isLoading, setIsLoading] = useState(true)

    useEffect(() => {
        const savedToken = localStorage.getItem('token')
        const savedUser = localStorage.getItem('user')

        if (savedToken && savedUser) {
            setToken(savedToken)
            setUser(JSON.parse(savedUser))
        }
        setIsLoading(false)
    }, [])

    function loginUser(authResponse) {
        setToken(authResponse.token)
        setUser(authResponse.user)
        localStorage.setItem('token', authResponse.token)
        localStorage.setItem('user', JSON.stringify(authResponse.user))
    }

    function logoutUser() {
        setToken(null)
        setUser(null)
        localStorage.removeItem('token')
        localStorage.removeItem('user')
    }

    const value = {
        user,
        token,
        isAuthenticated: !!token,
        isLoading,
        loginUser,
        logoutUser,
    }

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth() {
    return useContext(AuthContext)
}