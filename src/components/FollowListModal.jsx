import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { X } from 'lucide-react'
import { useAuth } from '../context/AuthContext'
import { getFollowers, getFollowing } from '../services/FollowService'

function FollowListModal({ userId, type, onClose }) {
    const { token } = useAuth()
    const [users, setUsers] = useState([])
    const [isLoading, setIsLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        async function fetchList() {
            setIsLoading(true)
            try {
                const result = type === 'followers'
                    ? await getFollowers(token, userId)
                    : await getFollowing(token, userId)
                setUsers(result)
            } catch (err) {
                setError(err.message)
            } finally {
                setIsLoading(false)
            }
        }
        fetchList()
    }, [userId, type, token])

    if (!type) return null

    return (
        <div
            className="fixed inset-0 bg-black/70 flex items-center justify-center z-50 px-4"
            onClick={onClose}
        >
            <div
                className="bg-[#0f141c] w-full max-w-sm max-h-[70vh] rounded-xl border border-[#1f2633] flex flex-col"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between px-4 py-3 border-b border-[#1f2633] shrink-0">
                    <h2 className="text-sm font-semibold text-white">
                        {type === 'followers' ? 'Takipçiler' : 'Takip Edilenler'}
                    </h2>
                    <button
                        onClick={onClose}
                        className="p-1.5 rounded-lg hover:bg-[#1e2533] text-slate-300"
                    >
                        <X className="w-4 h-4" />
                    </button>
                </div>

                <div className="overflow-y-auto p-2">
                    {isLoading && <p className="text-slate-400 text-sm px-3 py-2">Yükleniyor...</p>}
                    {error && <p className="text-red-400 text-sm px-3 py-2">{error}</p>}
                    {!isLoading && users.length === 0 && (
                        <p className="text-slate-400 text-sm px-3 py-2">
                            {type === 'followers' ? 'Henüz takipçi yok.' : 'Henüz kimseyi takip etmiyor.'}
                        </p>
                    )}

                    {users.map((u) => (
                        <Link
                            key={u.userID}
                            to={`/profile/${u.userID}`}
                            onClick={onClose}
                            className="flex items-center gap-3 p-2.5 rounded-lg hover:bg-[#171c24] transition"
                        >
                            <div className="w-10 h-10 rounded-full overflow-hidden bg-gradient-to-tr from-[#6366f1] via-[#8b5cf6] to-[#06b6d4] flex items-center justify-center text-sm font-semibold text-white shrink-0">
                                {u.ppUrl ? (
                                    <img src={u.ppUrl} alt={u.username} className="w-full h-full object-cover" />
                                ) : (
                                    u.username?.[0]?.toUpperCase()
                                )}
                            </div>
                            <span className="text-sm font-medium text-white">{u.username}</span>
                        </Link>
                    ))}
                </div>
            </div>
        </div>
    )
}

export default FollowListModal