import { useState } from 'react'
import { ChevronLeft, ChevronRight } from 'lucide-react'

function MediaGallery({ media, caption }) {
    const [currentIndex, setCurrentIndex] = useState(0)

    function goPrev() {
        setCurrentIndex((prev) => (prev === 0 ? media.length - 1 : prev - 1))
    }

    function goNext() {
        setCurrentIndex((prev) => (prev === media.length - 1 ? 0 : prev + 1))
    }

    return (
        <div className="relative bg-black">
            <img
                src={media[currentIndex].mediaURL}
                alt={caption || 'post'}
                className="w-full max-h-[600px] object-contain"
            />

            {media.length > 1 && (
                <>
                    <button
                        onClick={goPrev}
                        className="absolute left-2 top-1/2 -translate-y-1/2 p-1.5 rounded-full bg-black/60 hover:bg-black/80 text-white transition"
                    >
                        <ChevronLeft className="w-4 h-4" />
                    </button>
                    <button
                        onClick={goNext}
                        className="absolute right-2 top-1/2 -translate-y-1/2 p-1.5 rounded-full bg-black/60 hover:bg-black/80 text-white transition"
                    >
                        <ChevronRight className="w-4 h-4" />
                    </button>

                    <div className="absolute top-3 right-3 px-2 py-0.5 rounded-full bg-black/60 text-white text-xs">
                        {currentIndex + 1}/{media.length}
                    </div>

                    <div className="absolute bottom-3 left-1/2 -translate-x-1/2 flex gap-1.5">
                        {media.map((_, index) => (
                            <div
                                key={index}
                                className={`w-1.5 h-1.5 rounded-full transition ${index === currentIndex ? 'bg-white' : 'bg-white/40'
                                    }`}
                            />
                        ))}
                    </div>
                </>
            )}
        </div>
    )
}

export default MediaGallery