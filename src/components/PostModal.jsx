import { useState } from 'react'
import { Link } from 'react-router-dom'
import { X, Pencil } from 'lucide-react'
import { useAuth } from '../context/AuthContext'
import { toggleLike, deletePost, updatePost } from '../services/PostService'
import CommentSection from './CommentSection'
import LikesModal from './LikesModal'
import MediaGallery from './MediaGallery'

function PostModal({ post, onClose, onPostDeleted, onPostUpdated }) {
    const { token, user } = useAuth()
    const [isLiked, setIsLiked] = useState(post?.isLikedByCurrentUser)
    const [likeCount, setLikeCount] = useState(post?.likeCount)
    const [showLikes, setShowLikes] = useState(false)
    const [isEditing, setIsEditing] = useState(false)
    const [editCaption, setEditCaption] = useState(post?.caption || '')
    const [caption, setCaption] = useState(post?.caption || '')
    const [error, setError] = useState('')

    if (!post) return null

    const isOwnPost = post.author.userID === user.userID

    async function handleLikeClick() {
        try {
            const result = await toggleLike(token, post.postID)
            setIsLiked(result.isLiked)
            setLikeCount(isLiked ? likeCount - 1 : likeCount + 1)
        } catch (err) {
            console.error(err.message)
        }
    }

    async function handleDeleteClick() {
        if (!window.confirm('Bu postu silmek istediğine emin misin?')) return
        try {
            await deletePost(token, post.postID)
            onPostDeleted(post.postID)
        } catch (err) {
            console.error(err.message)
        }
    }

    async function handleEditSubmit(e) {
        e.preventDefault()
        setError('')
        try {
            const updated = await updatePost(token, post.postID, editCaption)
            setCaption(updated.caption)
            setIsEditing(false)
            if (onPostUpdated) onPostUpdated(updated)
        } catch (err) {
            setError(err.message)
        }
    }

    return (
        <div
            className="fixed inset-0 bg-black/70 flex items-center justify-center z-50 px-4"
            onClick={onClose}
        >
            <div
                className="bg-[#0f141c] w-full max-w-4xl max-h-[85vh] rounded-xl border border-[#1f2633] flex overflow-hidden"
                onClick={(e) => e.stopPropagation()}
            >
                {/* Sol: Medya */}
                <div className="hidden sm:flex flex-1 bg-black items-center justify-center min-w-0">
                    {post.media && post.media.length > 0 ? (
                        <MediaGallery media={post.media} caption={caption} />
                    ) : (
                        <p className="text-slate-500 text-sm">Medya yok</p>
                    )}
                </div>

                {/* Sağ: Bilgi + Yorumlar */}
                <div className="w-full sm:w-[380px] shrink-0 flex flex-col">
                    {/* Başlık */}
                    <div className="flex items-center justify-between px-4 py-3 border-b border-[#1f2633] gap-3">
                        <Link to={`/profile/${post.author.userID}`} onClick={onClose} className="flex items-center gap-2.5 min-w-0">
                            <div className="w-8 h-8 rounded-full overflow-hidden bg-gradient-to-tr from-[#6366f1] via-[#8b5cf6] to-[#06b6d4] flex items-center justify-center text-xs font-semibold text-white shrink-0">
                                {post.author.ppUrl ? (
                                    <img src={post.author.ppUrl} alt={post.author.username} className="w-full h-full object-cover" />
                                ) : (
                                    post.author.username?.[0]?.toUpperCase()
                                )}
                            </div>
                            <span className="text-sm font-semibold text-white truncate">{post.author.username}</span>
                        </Link>

                        <div className="flex items-center gap-3 shrink-0">
                            {isOwnPost && (
                                <button
                                    onClick={() => {
                                        setEditCaption(caption)
                                        setIsEditing(!isEditing)
                                    }}
                                    className="text-xs text-slate-400 hover:text-white font-medium"
                                >
                                    Düzenle
                                </button>
                            )}
                            <button
                                onClick={onClose}
                                className="p-1 rounded-lg hover:bg-[#1e2533] text-slate-400 hover:text-white transition"
                            >
                                <X className="w-4 h-4" />
                            </button>
                        </div>
                    </div>

                    {/* Sadece mobilde küçük görsel (sm altı ekranlar için) */}
                    <div className="sm:hidden">
                        {post.media && post.media.length > 0 && (
                            <img
                                src={post.media[0].mediaURL}
                                alt={caption || 'post'}
                                className="w-full aspect-square object-cover"
                            />
                        )}
                    </div>

                    {/* Caption + Yorumlar (kaydırılabilir alan) */}
                    <div className="flex-1 overflow-y-auto px-4 py-3 space-y-3">
                        {isEditing ? (
                            <form onSubmit={handleEditSubmit} className="space-y-2">
                                <textarea
                                    value={editCaption}
                                    onChange={(e) => setEditCaption(e.target.value)}
                                    rows={3}
                                    className="w-full bg-[#171c24] border border-[#1f2633] rounded-lg p-2 text-sm text-white resize-none focus:outline-none focus:border-[#6366f1]"
                                />
                                {error && <p className="text-red-400 text-xs">{error}</p>}
                                <div className="flex gap-2">
                                    <button
                                        type="submit"
                                        className="px-3 py-1.5 rounded-lg bg-[#6366f1] hover:bg-[#4f46e5] text-white text-xs font-medium transition"
                                    >
                                        Kaydet
                                    </button>
                                    <button
                                        type="button"
                                        onClick={() => setIsEditing(false)}
                                        className="px-3 py-1.5 rounded-lg bg-[#1e2533] hover:bg-[#283245] text-white text-xs font-medium transition"
                                    >
                                        Vazgeç
                                    </button>
                                </div>
                            </form>
                        ) : (
                            caption && (
                                <p className="text-sm text-slate-200">
                                    <span className="font-semibold text-white mr-1.5">{post.author.username}</span>
                                    {caption}
                                </p>
                            )
                        )}

                        <CommentSection postId={post.postID} />
                    </div>

                    {/* Alt: Beğeni */}
                    <div className="border-t border-[#1f2633] px-2 py-2 flex items-center justify-between">
                        <div className="flex items-center gap-3">
                            <button
                                onClick={handleLikeClick}
                                className="text-sm text-white"
                            >
                                {isLiked ? '❤️' : '🤍'}
                            </button>
                            <button
                                onClick={() => setShowLikes(true)}
                                className="text-sm text-white hover:underline"
                            >
                                {likeCount} beğenme
                            </button>
                        </div>
                        {isOwnPost && (
                            <button
                                onClick={handleDeleteClick}
                                className="text-xs text-red-400 hover:text-red-300 font-medium"
                            >
                                Postu Sil
                            </button>
                        )}
                    </div>
                </div>
            </div>

            {showLikes && (
                <LikesModal postId={post.postID} onClose={() => setShowLikes(false)} />
            )}
        </div>
    )
}

export default PostModal