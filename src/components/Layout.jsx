import { Link } from 'react-router-dom'
import { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import { getUserProfile } from '../services/UserService'
import { Home, User, Settings, LogOut, Search, UserPlus } from 'lucide-react'

function Layout({ children }) {
    const { user, token, logoutUser } = useAuth()
    const [ppUrl, setPpUrl] = useState(null)

    useEffect(() => {
        async function fetchAvatar() {
            if (!user || !token) return
            try {
                const result = await getUserProfile(token, user.userID)
                setPpUrl(result.ppUrl)
            } catch (err) {
                console.error(err)
            }
        }
        fetchAvatar()
    }, [user, token])

    if (!user) {
        return <div>Kullanıcı bilgileri yükleniyor...</div>;
    }


    return (
        <div className="min-h-screen bg-[#0f141c] text-[#e2e8f0] flex">
            {/* Sol Sidebar */}
            <nav className="w-64 shrink-0 border-r border-[#1f2633] flex flex-col justify-between py-6 px-4 h-screen sticky top-0">
                <div className="space-y-1">
                    <div className="px-3 mb-6">
                        <span className="text-lg font-semibold text-white tracking-tight">Nexus</span>
                    </div>



                    <Link
                        to="/timeline"
                        className="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-slate-300 hover:bg-[#1e2533] hover:text-white transition"
                    >
                        <Home className="w-4.5 h-4.5" />
                        Ana Akış
                    </Link>

                    <Link
                        to="/search"
                        className="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-slate-300 hover:bg-[#1e2533] hover:text-white transition"
                    >
                        <Search className="w-4.5 h-4.5" />
                        Ara
                    </Link>

                    <Link
                        to="/follow-requests"
                        className="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-slate-300 hover:bg-[#1e2533] hover:text-white transition"
                    >
                        <UserPlus className="w-4.5 h-4.5" />
                        Takip İstekleri
                    </Link>

                    <Link
                        to={`/profile/${user?.userID}`}
                        className="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-slate-300 hover:bg-[#1e2533] hover:text-white transition"
                    >
                        <User className="w-4.5 h-4.5" />
                        Profilim
                    </Link>

                    <Link
                        to="/profile/edit"
                        className="flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-slate-300 hover:bg-[#1e2533] hover:text-white transition"
                    >
                        <Settings className="w-4.5 h-4.5" />
                        Profili Düzenle
                    </Link>
                </div>

                <div className="space-y-3 px-3">
                    <Link
                        to={`/profile/${user.userID}`}
                        className="flex items-center gap-3 pb-3 border-t border-[#1f2633] pt-4 hover:opacity-80 transition"
                    >
                        <div className="w-9 h-9 rounded-full overflow-hidden bg-gradient-to-tr from-[#6366f1] via-[#8b5cf6] to-[#06b6d4] flex items-center justify-center text-sm font-semibold text-white shrink-0">
                            {ppUrl ? (
                                <img src={ppUrl} alt={user.username} className="w-full h-full object-cover" />
                            ) : (
                                user.username?.[0]?.toUpperCase()
                            )}
                        </div>
                        <span className="text-sm font-medium text-white truncate">{user.username}</span>
                    </Link>
                    <button
                        onClick={logoutUser}
                        className="w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium text-slate-300 hover:bg-[#1e2533] hover:text-white transition"
                    >
                        <LogOut className="w-4.5 h-4.5" />
                        Çıkış Yap
                    </button>
                </div>
            </nav>

            {/* Ana İçerik */}
            <main className="flex-1 bg-[#0f141c]">
                {children}
            </main>
        </div>
    )
}

export default Layout