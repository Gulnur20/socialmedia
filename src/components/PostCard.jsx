import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Heart, MessageCircle, Trash2 } from 'lucide-react'
import { useAuth } from '../context/AuthContext'
import { toggleLike, deletePost } from '../services/PostService'
import CommentSection from './CommentSection'
import LikesModal from './LikesModal'
import MediaGallery from './MediaGallery'

function PostCard({ post, onPostDeleted }) {
    const { token, user } = useAuth()
    const [isLiked, setIsLiked] = useState(post.isLikedByCurrentUser)
    const [likeCount, setLikeCount] = useState(post.likeCount)
    const [showComments, setShowComments] = useState(false)
    const [showLikes, setShowLikes] = useState(false)

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

    return (
        <div className="bg-[#171c24] border border-[#1f2633] rounded-2xl overflow-hidden mb-6">
            {/* Başlık */}
            <div className="flex items-center justify-between px-4 py-3">
                <Link to={`/profile/${post.author.userID}`} className="flex items-center gap-2.5">
                    <div className="w-10  h-10 rounded-full overflow-hidden bg-gradient-to-tr from-[#6366f1] via-[#8b5cf6] to-[#06b6d4] flex items-center justify-center text-xs font-semibold text-white shrink-0">
                        {post.author.ppUrl ? (
                            <img src={post.author.ppUrl} alt={post.author.username} className="w-full h-full object-cover" />
                        ) : (
                            post.author.username?.[0]?.toUpperCase()
                        )}
                    </div>
                    <span className="text-sm font-semibold text-white">{post.author.username}</span>
                </Link>

                {isOwnPost && (
                    <button
                        onClick={handleDeleteClick}
                        className="btn btn-danger"
                        type="button"
                        title="Sil"
                    >
                        <Trash2 className="w-4 h-4" />
                    </button>
                )}
            </div>

            {/* Medya */}
            {post.media && post.media.length > 0 && (
                <MediaGallery media={post.media} caption={post.caption} />
            )}

            {/* Aksiyonlar */}
            <div className="px-5 pt-3 flex items-center gap-4">
                <button onClick={handleLikeClick} className="text-white hover:opacity-70 transition">
                    <Heart className={`w-6 h-6 ${isLiked ? 'fill-red-500 text-red-500' : ''}`} />
                </button>
                <button onClick={() => setShowComments(!showComments)} className="text-white hover:opacity-70 transition">
                    <MessageCircle className="w-5 h-5" />
                </button>
            </div>

            {/* Beğeni / yorum sayacı */}
            <div className="px-5 pt3 flex items-center gap-3 text-sm">
                <button
                    onClick={() => setShowLikes(true)}
                    className="text-white font-semibold hover:underline"
                >
                    {likeCount} beğenme
                </button>
                {post.commentCount > 0 && (
                    <button
                        onClick={() => setShowComments(!showComments)}
                        className="text-slate-400 hover:text-slate-300"
                    >
                        {post.commentCount} yorum
                    </button>
                )}
            </div>

            {/* Caption */}
            {post.caption && (
                <p className="px-5 pt-2 text-sm text-slate-200">
                    <span className="font-semibold text-white mr-1.5">{post.author.username}</span>
                    {post.caption}
                </p>
            )}

            {/* Yorumlar */}
            <div className="px-5 pb-5">
                {showComments && <CommentSection postId={post.postID} />}
            </div>

            {showLikes && (
                <LikesModal postId={post.postID} onClose={() => setShowLikes(false)} />
            )}
        </div>
    )
}

export default PostCard