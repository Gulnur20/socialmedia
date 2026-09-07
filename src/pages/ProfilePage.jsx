import { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import { getUserProfile } from '../services/UserService'
import { followUser, unfollowUser } from '../services/FollowService'
import { getPostsByUserId } from '../services/PostService'
import Layout from '../components/Layout'
import { Settings, Share2, UserPlus, UserCheck, Lock } from 'lucide-react'
import { getPostById } from '../services/PostService'
import PostModal from '../components/PostModal'
import FollowListModal from '../components/FollowListModal'
import { useParams, useNavigate } from 'react-router-dom'

function ProfilePage() {
    const { id } = useParams()
    const { token, user } = useAuth()
    const [profile, setProfile] = useState(null)
    const [error, setError] = useState('')
    const [posts, setPosts] = useState([])
    const [selectedPost, setSelectedPost] = useState(null)
    const [followListType, setFollowListType] = useState(null)
    const [linkCopied, setLinkCopied] = useState(false)
    const navigate = useNavigate()

    useEffect(() => {
        async function fetchProfile() {
            try {
                const result = await getUserProfile(token, id)
                setProfile(result)
                return result
            } catch (err) {
                setError(err.message)
                return null
            }
        }

        async function fetchPosts() {
            try {
                const result = await getPostsByUserId(token, id)
                setPosts(result)
            } catch (err) {
                setError(err.message)
            }
        }

        async function load() {
            const profileResult = await fetchProfile()
            const isOwnProfile = profileResult && String(profileResult.userID) === String(user?.userID)
            const canSeePosts = profileResult && (!profileResult.isPrivate || profileResult.isFollowedByCurrentUser || isOwnProfile)
            if (canSeePosts) {
                fetchPosts()
            }
        }

        load()
    }, [id, token, user])

    async function handleFollowToggle() {
        try {
            if (profile.isFollowedByCurrentUser) {
                await unfollowUser(token, id)
                setProfile({
                    ...profile,
                    isFollowedByCurrentUser: false,
                    followerCount: profile.followerCount - 1,
                })
            } else {
                await followUser(token, id)
                setProfile({
                    ...profile,
                    isFollowedByCurrentUser: true,
                    followerCount: profile.followerCount + 1,
                })
            }
        } catch (err) {
            setError(err.message)
        }
    }

    async function handleShareClick() {
        const profileUrl = `${window.location.origin}/profile/${id}`
        try {
            await navigator.clipboard.writeText(profileUrl)
            setLinkCopied(true)
            setTimeout(() => setLinkCopied(false), 2000)
        } catch (err) {
            console.error('Link kopyalanamadı.')
        }
    }

    async function handlePostClick(postId) {
        try {
            const result = await getPostById(token, postId)
            setSelectedPost(result)
        } catch (err) {
            setError(err.message)
        }
    }

    return (
        <Layout>
            <div className="w-full max-w-[860px] mx-auto px-4 py-8">

                {error && <p className="text-red-500 mb-4">{error}</p>}
                {!profile && !error && <p className="text-slate-400">Yükleniyor...</p>}

                {profile && (
                    <header className="flex flex-col sm:flex-row items-center sm:items-start gap-8 md:gap-14 pb-10 border-b border-[#1f2633]">
                        {/* Avatar */}
                        <div className="relative shrink-0">
                            <div className="w-32 h-32 md:w-36 md:h-36 rounded-full p-[3px] bg-gradient-to-tr from-[#6366f1] via-[#8b5cf6] to-[#06b6d4]">
                                <div className="w-full h-full rounded-full overflow-hidden border-2 border-[#0f141c]">
                                    {profile.ppUrl ? (
                                        <img
                                            src={profile.ppUrl}
                                            alt={profile.username}
                                            className="w-full h-full object-cover"
                                        />
                                    ) : (
                                        <div className="w-full h-full bg-[#171c24] flex items-center justify-center text-3xl font-semibold text-white">
                                            {profile.username?.[0]?.toUpperCase()}
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>

                        <div className="flex-1 space-y-5 text-center sm:text-left">
                            <div className="flex flex-wrap items-center justify-center sm:justify-start gap-3">
                                <h1 className="text-xl md:text-2xl font-semibold tracking-tight text-white">
                                    {profile.username}
                                </h1>
                                <div className="flex items-center gap-2">
                                    <button
                                        onClick={handleFollowToggle}
                                        className={`flex items-center gap-1.5 px-5 py-2 rounded-full text-sm font-semibold whitespace-nowrap shrink-0 transition-all duration-200 ${profile.isFollowedByCurrentUser
                                            ? 'bg-[#171c24] border border-[#1f2633] text-slate-300 hover:border-red-500/40 hover:text-red-400 hover:bg-red-500/5'
                                            : 'bg-[#6366f1] text-white hover:bg-[#4f46e5] shadow-md shadow-[#6366f1]/20'
                                            }`}
                                    >
                                        {profile.isFollowedByCurrentUser ? (
                                            <>
                                                <UserCheck className="w-4 h-4" />
                                                Takip Ediliyor
                                            </>
                                        ) : (
                                            <>
                                                <UserPlus className="w-4 h-4" />
                                                Takip Et
                                            </>
                                        )}
                                    </button>

                                    <button
                                        onClick={() => navigate('/profile/edit')}
                                        className="p-2 rounded-lg bg-[#1e2533] hover:bg-[#283245] text-slate-300 transition duration-150"
                                    >
                                        <Settings className="w-4 h-4" />
                                    </button>
                                    <button
                                        onClick={handleShareClick}
                                        className="relative p-2 rounded-lg bg-[#1e2533] hover:bg-[#283245] text-slate-300 transition duration-150"
                                    >
                                        <Share2 className="w-4 h-4" />
                                        {linkCopied && (
                                            <span className="absolute -bottom-8 left-1/2 -translate-x-1/2 px-2 py-1 rounded-md bg-[#171c24] border border-[#1f2633] text-xs text-white whitespace-nowrap">
                                                Link kopyalandı
                                            </span>
                                        )}
                                    </button>
                                </div>
                            </div>

                            <div className="flex justify-center sm:justify-start gap-8 text-sm">
                                <div className="flex gap-1.5">
                                    <span className="font-semibold text-white">{profile.postCount}</span>
                                    <span className="text-slate-400">gönderi</span>
                                </div>

                                <button onClick={() => setFollowListType('followers')} className="flex gap-1.5 hover:opacity-80 transition">
                                    <span className="font-semibold text-white">{profile.followerCount}</span>
                                    <span className="text-slate-400">takipçi</span>
                                </button>
                                <button onClick={() => setFollowListType('following')} className="flex gap-1.5 hover:opacity-80 transition">
                                    <span className="font-semibold text-white">{profile.followingCount}</span>
                                    <span className="text-slate-400">takip</span>
                                </button>
                            </div>

                            <div className="space-y-2 text-sm leading-relaxed">
                                <p className="font-semibold text-white">
                                    {profile.firstName} {profile.lastName}
                                </p>
                                {profile.biography && (
                                    <p className="text-slate-300 max-w-xl">
                                        {profile.biography}
                                    </p>
                                )}
                            </div>
                        </div>
                    </header>
                )}

                {profile && profile.isPrivate && !profile.isFollowedByCurrentUser && profile.userID !== user?.userID ? (
                    <div className="flex flex-col items-center justify-center py-16 text-center">
                        <div className="w-16 h-16 rounded-full border-2 border-[#1f2633] flex items-center justify-center mb-4">
                            <Lock className="w-7 h-7 text-slate-400" />
                        </div>
                        <p className="text-white font-semibold text-lg">Bu Hesap Gizli</p>
                        <p className="text-slate-400 text-sm mt-1">
                            Fotoğraflarını ve videolarını görmek için bu hesabı takip et.
                        </p>
                    </div>
                ) : (
                    posts.length > 0 && (
                        <div className="grid grid-cols-3 gap-2 md:gap-3 py-6">
                            {posts.map((post) => (
                                <div
                                    key={post.postID}
                                    onClick={() => handlePostClick(post.postID)}
                                    className="group relative aspect-square bg-[#171c24] rounded-lg overflow-hidden cursor-pointer border border-[#1f2633]/60"
                                >
                                    {post.media && post.media.length > 0 ? (
                                        <img
                                            src={post.media[0].mediaURL}
                                            alt={post.caption || 'post'}
                                            className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
                                        />
                                    ) : (
                                        <div className="w-full h-full flex items-center justify-center text-slate-500 text-sm">
                                            Medya yok
                                        </div>
                                    )}
                                </div>
                            ))}
                        </div>
                    )
                )}
            </div>

            <PostModal
                key={selectedPost?.postID}
                post={selectedPost}
                onClose={() => setSelectedPost(null)}
                onPostDeleted={(id) => {
                    setPosts(posts.filter((p) => p.postID !== id))
                    setSelectedPost(null)
                }}
                onPostUpdated={(updated) => {
                    setPosts(posts.map((p) => p.postID === updated.postID ? { ...p, caption: updated.caption } : p))
                }}
            />
            {followListType && (
                <FollowListModal
                    userId={id}
                    type={followListType}
                    onClose={() => setFollowListType(null)}
                />
            )}
        </Layout>
    )
}

export default ProfilePage