import { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import { createPost } from '../services/PostService'
import { getUserProfile } from '../services/UserService'
import { uploadFile } from '../services/UploadService'
import { Image as ImageIcon, X } from 'lucide-react'

function CreatePostForm({ onPostCreated }) {
    const { token, user } = useAuth()
    const [caption, setCaption] = useState('')
    const [mediaUrls, setMediaUrls] = useState([])
    const [error, setError] = useState('')
    const [isLoading, setIsLoading] = useState(false)
    const [isUploading, setIsUploading] = useState(false)
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

    async function handleFileChange(e) {
        const files = Array.from(e.target.files)
        if (files.length === 0) return

        if (mediaUrls.length + files.length > 10) {
            setError('Bir gönderiye en fazla 10 medya ekleyebilirsin.')
            return
        }

        setIsUploading(true)
        setError('')
        try {
            const uploadedUrls = []
            for (const file of files) {
                const url = await uploadFile(token, file, 'post')
                uploadedUrls.push(url)
            }
            setMediaUrls([...mediaUrls, ...uploadedUrls])
        } catch (err) {
            setError(err.message)
        } finally {
            setIsUploading(false)
            e.target.value = ''
        }
    }

    function handleRemoveMedia(index) {
        setMediaUrls(mediaUrls.filter((_, i) => i !== index))
    }

    async function handleSubmit(e) {
        e.preventDefault()
        setError('')

        if (mediaUrls.length === 0) {
            setError('En az bir medya eklemelisin.')
            return
        }

        setIsLoading(true)

        try {
            const postData = {
                caption,
                media: mediaUrls.map((url) => ({ url, mediaType: 'Image' })),
            }
            const newPost = await createPost(token, postData)

            setCaption('')
            setMediaUrls([])

            onPostCreated(newPost)
        } catch (err) {
            setError(err.message)
        } finally {
            setIsLoading(false)
        }
    }

    return (
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
                        type="file"
                        accept="image/*"
                        multiple
                        onChange={handleFileChange}
                        className="flex-1 bg-transparent text-white text-sm focus:outline-none file:mr-2 file:py-1 file:px-2 file:rounded file:border-0 file:bg-[#1e2533] file:text-white file:text-xs file:cursor-pointer cursor-pointer"
                    />
                </div>
            </div>

            {isUploading && <p className="text-xs text-slate-400 ml-13">Yükleniyor...</p>}

            {mediaUrls.length > 0 && (
                <div className="ml-13 grid grid-cols-3 gap-2">
                    {mediaUrls.map((url, index) => (
                        <div key={index} className="relative aspect-square rounded-lg overflow-hidden border border-[#1f2633]">
                            <img
                                src={url}
                                alt={`Medya ${index + 1}`}
                                className="w-full h-full object-cover"
                            />
                            <button
                                type="button"
                                onClick={() => handleRemoveMedia(index)}
                                className="absolute top-1 right-1 p-1 rounded-full bg-black/70 hover:bg-black/90 text-white transition"
                            >
                                <X className="w-3 h-3" />
                            </button>
                        </div>
                    ))}
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
                    disabled={isLoading || mediaUrls.length === 0}
                    className="px-5 py-2 rounded-lg bg-[#6366f1] hover:bg-[#4f46e5] disabled:opacity-40 disabled:cursor-not-allowed text-white text-sm font-medium transition"
                >
                    {isLoading ? 'Paylaşılıyor...' : 'Paylaş'}
                </button>
            </div>
        </form>
    )
}

export default CreatePostForm