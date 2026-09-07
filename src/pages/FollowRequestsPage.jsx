import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { getFollowRequests, acceptFollowRequest, rejectFollowRequest } from '../services/FollowService'
import Layout from '../components/Layout'

function FollowRequestsPage() {
    const { token } = useAuth()
    const [requests, setRequests] = useState([])
    const [isLoading, setIsLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        async function fetchRequests() {
            try {
                const result = await getFollowRequests(token)
                setRequests(result)
            } catch (err) {
                setError(err.message)
            } finally {
                setIsLoading(false)
            }
        }
        fetchRequests()
    }, [token])

    async function handleAccept(requestId) {
        try {
            await acceptFollowRequest(token, requestId)
            setRequests(requests.filter((r) => r.requestID !== requestId))
        } catch (err) {
            setError(err.message)
        }
    }

    async function handleReject(requestId) {
        try {
            await rejectFollowRequest(token, requestId)
            setRequests(requests.filter((r) => r.requestID !== requestId))
        } catch (err) {
            setError(err.message)
        }
    }

    return (
        <Layout>
            <div className="w-full max-w-[600px] mx-auto px-4 py-8">
                <h1 className="text-xl font-semibold text-white mb-6">Takip İstekleri</h1>

                {error && <p className="text-red-400 text-sm mb-4">{error}</p>}
                {isLoading && <p className="text-slate-400">Yükleniyor...</p>}
                {!isLoading && requests.length === 0 && (
                    <p className="text-slate-400">Bekleyen takip isteği yok.</p>
                )}

                <div className="space-y-2">
                    {requests.map((r) => (
                        <div
                            key={r.requestID}
                            className="flex items-center justify-between gap-3 p-3 rounded-lg bg-[#171c24] border border-[#1f2633]"
                        >
                            <Link to={`/profile/${r.requestingUser.userID}`} className="flex items-center gap-3">
                                <div className="w-11 h-11 rounded-full overflow-hidden bg-gradient-to-tr from-[#6366f1] via-[#8b5cf6] to-[#06b6d4] flex items-center justify-center text-sm font-semibold text-white shrink-0">
                                    {r.requestingUser.ppUrl ? (
                                        <img src={r.requestingUser.ppUrl} alt={r.requestingUser.username} className="w-full h-full object-cover" />
                                    ) : (
                                        r.requestingUser.username?.[0]?.toUpperCase()
                                    )}
                                </div>
                                <span className="text-sm font-medium text-white">{r.requestingUser.username}</span>
                            </Link>

                            <div className="flex items-center gap-2 shrink-0">
                                <button
                                    onClick={() => handleAccept(r.requestID)}
                                    className="px-3 py-1.5 rounded-lg bg-[#6366f1] hover:bg-[#4f46e5] text-white text-xs font-medium transition"
                                >
                                    Kabul Et
                                </button>
                                <button
                                    onClick={() => handleReject(r.requestID)}
                                    className="px-3 py-1.5 rounded-lg bg-[#1e2533] hover:bg-[#283245] text-white text-xs font-medium transition"
                                >
                                    Reddet
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </Layout>
    )
}

export default FollowRequestsPage