import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { searchUsers } from '../services/UserService'
import Layout from '../components/Layout'
import { Search } from 'lucide-react'

function SearchPage() {
    const { token } = useAuth()
    const [query, setQuery] = useState('')
    const [results, setResults] = useState([])
    const [isLoading, setIsLoading] = useState(false)
    const [error, setError] = useState('')

    useEffect(() => {
        if (!query.trim()) {
            setResults([])
            return
        }

        setIsLoading(true)
        setError('')

        const timeoutId = setTimeout(async () => {
            try {
                const result = await searchUsers(token, query)
                setResults(result)
            } catch (err) {
                setError(err.message)
            } finally {
                setIsLoading(false)
            }
        }, 300)

        return () => clearTimeout(timeoutId)
    }, [query, token])

    return (
        <Layout>
            <div className="w-full max-w-[600px] mx-auto px-4 py-8">
                <h1 className="text-xl font-semibold text-white mb-6">Kullanıcı Ara</h1>

                <div className="flex items-center gap-2 mb-6 px-4 py-2.5 rounded-lg bg-[#171c24] border border-[#1f2633] focus-within:border-[#6366f1] transition">
                    <Search className="w-4 h-4 text-slate-500 shrink-0" />
                    <input
                        type="text"
                        value={query}
                        onChange={(e) => setQuery(e.target.value)}
                        placeholder="Kullanıcı adı veya isim ara..."
                        className="flex-1 bg-transparent text-white placeholder-slate-500 focus:outline-none"
                        autoFocus
                    />
                </div>

                {isLoading && <p className="text-slate-400">Aranıyor...</p>}
                {error && <p className="text-red-500">{error}</p>}
                {!isLoading && query && results.length === 0 && (
                    <p className="text-slate-400">Sonuç bulunamadı.</p>
                )}

                <div className="space-y-1">
                    {results.map((user) => (
                        <Link
                            key={user.userID}
                            to={`/profile/${user.userID}`}
                            className="flex items-center gap-3 p-3 rounded-lg hover:bg-[#171c24] transition"
                        >
                            <div className="w-11 h-11 rounded-full overflow-hidden bg-gradient-to-tr from-[#6366f1] via-[#8b5cf6] to-[#06b6d4] flex items-center justify-center text-sm font-semibold text-white shrink-0">
                                {user.ppUrl ? (
                                    <img src={user.ppUrl} alt={user.username} className="w-full h-full object-cover" />
                                ) : (
                                    user.username?.[0]?.toUpperCase()
                                )}
                            </div>
                            <div>
                                <p className="font-semibold text-white text-sm">{user.username}</p>
                                <p className="text-xs text-slate-400">{user.firstName} {user.lastName}</p>
                            </div>
                        </Link>
                    ))}
                </div>
            </div>
        </Layout>
    )
}

export default SearchPage