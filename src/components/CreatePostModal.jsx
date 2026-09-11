import { X } from 'lucide-react'
import CreatePostForm from './CreatePostForm'

function CreatePostModal({ onClose, onPostCreated }) {
    function handlePostCreated(newPost) {
        onPostCreated(newPost)
        onClose()
    }
    function handlePostCreated(newPost) {
        onPostCreated(newPost)
        onClose()
    }
    return (
        <div
            className="fixed inset-0 bg-black/70 flex items-center justify-center z-50 px-4"
            onClick={onClose}
        >
            <div
                className="bg-[#171c24] w-full max-w-md rounded-2xl border border-[#1f2633] relative"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="flex items-center justify-between px-5 py-4 border-b border-[#1f2633]">
                    <h2 className="text-sm font-semibold text-white">Yeni Gönderi Oluştur</h2>
                    <button
                        onClick={onClose}
                        className="p-1 rounded-lg hover:bg-[#1e2533] text-slate-400 hover:text-white transition"
                    >
                        <X className="w-4 h-4" />
                    </button>
                </div>

                <div className="p-5">
                    <CreatePostForm onPostCreated={handlePostCreated} />
                </div>
            </div>
        </div>
    )
}

export default CreatePostModal