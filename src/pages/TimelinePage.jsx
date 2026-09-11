import { useState, useEffect, useRef, useCallback } from 'react'
import { useAuth } from '../context/AuthContext'
import { getTimeline } from '../services/PostService'
import PostCard from '../components/PostCard'
import CreatePostForm from '../components/CreatePostForm'
import Layout from '../components/Layout'
import CreatePostModal from '../components/CreatePostModal'
import { Plus } from 'lucide-react'

function TimelinePage() {
    const { token } = useAuth()
    const [posts, setPosts] = useState([])
    const [page, setPage] = useState(1)
    const [hasMore, setHasMore] = useState(true)
    const [isLoading, setIsLoading] = useState(true)
    const [isLoadingMore, setIsLoadingMore] = useState(false)
    const [error, setError] = useState('')
    const observerRef = useRef(null)
    const loadMoreRef = useRef(null)

    useEffect(() => {
        async function fetchFirstPage() {
            setIsLoading(true)
            try {
                const result = await getTimeline(token, 1, 10)
                setPosts(result.items)
                setHasMore(result.hasMore)
                setPage(1)
            } catch (err) {
                setError(err.message)
            } finally {
                setIsLoading(false)
            }
        }

        fetchFirstPage()
    }, [token])

    const loadMore = useCallback(async () => {
        if (isLoadingMore || !hasMore) return

        setIsLoadingMore(true)
        try {
            const nextPage = page + 1
            const result = await getTimeline(token, nextPage, 10)
            setPosts((prev) => [...prev, ...result.items])
            setHasMore(result.hasMore)
            setPage(nextPage)
        } catch (err) {
            setError(err.message)
        } finally {
            setIsLoadingMore(false)
        }
    }, [token, page, hasMore, isLoadingMore])

    useEffect(() => {
        if (observerRef.current) observerRef.current.disconnect()

        observerRef.current = new IntersectionObserver((entries) => {
            if (entries[0].isIntersecting) {
                loadMore()
            }
        })

        if (loadMoreRef.current) {
            observerRef.current.observe(loadMoreRef.current)
        }

        return () => observerRef.current?.disconnect()
    }, [loadMore])

    function handlePostCreated(newPost) {
        setPosts([newPost, ...posts])
    }

    function handlePostDeleted(deletedPostId) {
        setPosts(posts.filter((p) => p.postID !== deletedPostId))
    }

    return (
        <Layout>
            <div className="w-full max-w-[600px] mx-auto px-4 py-8">


                {isLoading && <p className="text-slate-400">Yükleniyor...</p>}
                {error && <p className="text-red-500">{error}</p>}
                {!isLoading && posts.length === 0 && (
                    <p className="text-slate-400">Henüz gösterilecek post yok.</p>
                )}

                {posts.map((post) => (
                    <PostCard key={post.postID} post={post} onPostDeleted={handlePostDeleted} />
                ))}

                <div ref={loadMoreRef} className="h-10 flex items-center justify-center">
                    {isLoadingMore && <p className="text-slate-400 text-sm">Daha fazla yükleniyor...</p>}
                    {!hasMore && posts.length > 0 && (
                        <p className="text-slate-500 text-sm">Gösterilecek başka post yok.</p>
                    )}
                </div>
            </div>
        </Layout>
    )
}

export default TimelinePage