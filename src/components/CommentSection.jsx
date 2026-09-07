import { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import { getComments, addComment, toggleCommentLike } from '../services/CommentService'

function CommentItem({ comment, postId, token, onReplyAdded, depth = 0 }) {
    const [isReplying, setIsReplying] = useState(false)
    const [replyText, setReplyText] = useState('')
    const [showReplies, setShowReplies] = useState(false)
    const [isLiked, setIsLiked] = useState(comment.isLikedByCurrentUser)
    const [likeCount, setLikeCount] = useState(comment.likeCount)

    async function handleReplySubmit(e) {
        e.preventDefault()
        if (!replyText.trim()) return

        try {
            const created = await addComment(token, postId, replyText, comment.commentID)
            onReplyAdded(comment.commentID, created)
            setReplyText('')
            setIsReplying(false)
            setShowReplies(true)
        } catch (err) {
            console.error(err.message)
        }
    }

    async function handleLikeClick() {
        try {
            const result = await toggleCommentLike(token, comment.commentID)
            setIsLiked(result.isLiked)
            setLikeCount(isLiked ? likeCount - 1 : likeCount + 1)
        } catch (err) {
            console.error(err.message)
        }
    }

    const hasReplies = comment.replies && comment.replies.length > 0

    return (
        <div className={depth > 0 ? 'mt-3 ml-10' : 'mt-3'}>
            <p className="text-sm text-slate-200">
                <span className="font-semibold text-white mr-1.5">{comment.author.username}</span>
                {comment.commentText}
            </p>

            <div className="flex items-center gap-3 mt-1">
                <button
                    onClick={handleLikeClick}
                    className="flex items-center gap-1 text-xs text-slate-500 hover:text-slate-300"
                >
                    {isLiked ? '❤️' : '🤍'} {likeCount > 0 && likeCount}
                </button>

                <button
                    onClick={() => setIsReplying(!isReplying)}
                    className="text-xs text-slate-500 hover:text-slate-300"
                >
                    Yanıtla
                </button>

                {hasReplies && (
                    <button
                        onClick={() => setShowReplies(!showReplies)}
                        className="text-xs text-slate-500 hover:text-slate-300 font-medium"
                    >
                        {showReplies
                            ? 'Yanıtları gizle'
                            : `${comment.replies.length} yanıtı görüntüle`}
                    </button>
                )}
            </div>

            {isReplying && (
                <form onSubmit={handleReplySubmit} className="flex gap-2 mt-2 ml-10">
                    <input
                        type="text"
                        placeholder={`${comment.author.username} kullanıcısına yanıt yaz...`}
                        value={replyText}
                        onChange={(e) => setReplyText(e.target.value)}
                        className="flex-1 bg-transparent border-b border-slate-600 text-sm text-white placeholder-slate-500 focus:outline-none focus:border-indigo-400 py-1"
                    />
                    <button type="submit" className="text-xs text-indigo-400 font-medium shrink-0">
                        Gönder
                    </button>
                </form>
            )}

            {hasReplies && showReplies && (
                <div>
                    {comment.replies.map((reply) => (
                        <CommentItem
                            key={reply.commentID}
                            comment={reply}
                            postId={postId}
                            token={token}
                            onReplyAdded={onReplyAdded}
                            depth={depth + 1}
                        />
                    ))}
                </div>
            )}
        </div>
    )
}

function CommentSection({ postId }) {
    const { token } = useAuth()
    const [comments, setComments] = useState([])
    const [newComment, setNewComment] = useState('')
    const [error, setError] = useState('')

    useEffect(() => {
        async function fetchComments() {
            try {
                const result = await getComments(token, postId)
                setComments(result)
            } catch (err) {
                setError(err.message)
            }
        }
        fetchComments()
    }, [postId, token])

    async function handleSubmit(e) {
        e.preventDefault()
        if (!newComment.trim()) return

        try {
            const created = await addComment(token, postId, newComment)
            setComments([...comments, created])
            setNewComment('')
        } catch (err) {
            setError(err.message)
        }
    }

    function handleReplyAdded(parentId, newReply) {
        function addReplyRecursive(list) {
            return list.map((c) => {
                if (c.commentID === parentId) {
                    return { ...c, replies: [...(c.replies || []), newReply] }
                }
                if (c.replies && c.replies.length > 0) {
                    return { ...c, replies: addReplyRecursive(c.replies) }
                }
                return c
            })
        }
        setComments(addReplyRecursive(comments))
    }

    return (
        <div className="mt-2 pt-2 border-t border-slate-800">
            {error && <p className="text-red-400 text-sm">{error}</p>}

            {comments.map((c) => (
                <CommentItem
                    key={c.commentID}
                    comment={c}
                    postId={postId}
                    token={token}
                    onReplyAdded={handleReplyAdded}
                />
            ))}

            <form onSubmit={handleSubmit} className="flex gap-2 mt-3">
                <input
                    type="text"
                    placeholder="Yorum yaz..."
                    value={newComment}
                    onChange={(e) => setNewComment(e.target.value)}
                    className="flex-1 bg-transparent border-b border-slate-600 text-sm text-white placeholder-slate-500 focus:outline-none focus:border-indigo-400 py-1"
                />
                <button type="submit" className="text-xs text-indigo-400 font-medium shrink-0">
                    Gönder
                </button>
            </form>
        </div>
    )
}

export default CommentSection