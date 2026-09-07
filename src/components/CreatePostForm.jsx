import { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import { createPost } from '../services/PostService'
import { getUserProfile } from '../services/UserService'
import { Image as ImageIcon } from 'lucide-react'

function CreatePostForm({ onPostCreated }) {
    const { token, user } = useAuth()
    const [caption, setCaption] = useState('')
    const [mediaUrl, setMediaUrl] = useState('')
    const [error, setError] = useState('')
    const [isLoading, setIsLoading] = useState(false)
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

    async function handleSubmit(e) {
        e.preventDefault()
        setError('')
        setIsLoading(true)

        try {
            const postData = {
                caption,
                media: [{ url: mediaUrl, mediaType: 'Image' }],
            }
            const newPost = await createPost(token, postData)

            setCaption('')
            setMediaUrl('')

            onPostCreated(newPost)
        } catch (err) {
            setError(err.message)
        } finally {
            setIsLoading(false)
        }
    }

    return (
        <div className="bg-[#171c24] border border-[#1f2633] rounded-2xl p-5 mb-6">
            <form onSubmit={handleSubmit} className="space-y-3">
                <div className="flex gap-3">
                    <div className="w-10 h-10 rounded-full overflow-hidden bg-gradient-to-tr from-[#6366f1] via-[#8b5cf6] to-[#06b6d4] flex items-center justify-center text-sm font-semibold text-white shrink-0">
                        {ppUrl ? (
                            <img src={ppUrl} alt={user?.username} className="w-full h-full object-cover" />
                        ) : (
                            user?.username?.[0]?.toUpperCase()
                        )}
                    </div>

                    <textarea
                        placeholder="Ne düşünüyorsun?"
                        value={caption}
                        onChange={(e) => setCaption(e.target.value)}
                        rows={3}
                        className="flex-1 bg-transparent text-white placeholder-slate-500 text-sm resize-none focus:outline-none"
                    />
                </div>

                <div className="flex items-center gap-2 pl-13">
                    <div className="flex-1 flex items-center gap-2 px-3 py-2 rounded-lg bg-[#0f141c] border border-[#1f2633] focus-within:border-[#6366f1] transition">
                        <ImageIcon className="w-4 h-4 text-slate-500 shrink-0" />
                        <input
                            type="text"
                            placeholder="Resim URL'si"
                            value={mediaUrl}
                            onChange={(e) => setMediaUrl(e.target.value)}
                            className="flex-1 bg-transparent text-white placeholder-slate-500 text-sm focus:outline-none"
                        />
                    </div>
                </div>

                {mediaUrl && (
                    <div className="ml-13 rounded-lg overflow-hidden border border-[#1f2633] max-h-64">
                        <img
                            src={mediaUrl}
                            alt="Önizleme"
                            className="w-full h-full object-cover"
                            onError={(e) => { e.target.style.display = 'none' }}
                        />
                    </div>
                )}

                {error && (
                    <p className="text-red-400 text-sm bg-red-500/10 border border-red-500/20 rounded-lg px-3 py-2">
                        {error}
                    </p>
                )}

                <div className="flex justify-end pt-1">
                    <button
                        type="submit"
                        disabled={isLoading || !mediaUrl}
                        className="px-5 py-2 rounded-lg bg-[#6366f1] hover:bg-[#4f46e5] disabled:opacity-40 disabled:cursor-not-allowed text-white text-sm font-medium transition"
                    >
                        {isLoading ? 'Paylaşılıyor...' : 'Paylaş'}
                    </button>
                </div>
            </form>
        </div>
    )
}

export default CreatePostForm