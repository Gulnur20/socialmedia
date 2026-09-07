import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import Layout from '../components/Layout'
import { getMyProfile, updateProfile, updateSettings, freezeAccount, deleteAccount, changePassword } from '../services/UserService'

function EditProfilePage() {
    const { token, logoutUser } = useAuth()
    const navigate = useNavigate()
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        biography: '',
        ppUrl: '',
        birthDate: '',
    })
    const [isPrivate, setIsPrivate] = useState(false)
    const [error, setError] = useState('')
    const [successMessage, setSuccessMessage] = useState('')
    const [isLoading, setIsLoading] = useState(false)
    const [currentPassword, setCurrentPassword] = useState('')
    const [newPassword, setNewPassword] = useState('')
    const [confirmPassword, setConfirmPassword] = useState('')
    const [passwordError, setPasswordError] = useState('')
    const [passwordSuccess, setPasswordSuccess] = useState('')
    const [isChangingPassword, setIsChangingPassword] = useState(false)
    useEffect(() => {
        async function fetchProfile() {
            try {
                const result = await getMyProfile(token)
                setFormData({
                    firstName: result.firstName || '',
                    lastName: result.lastName || '',
                    biography: result.biography || '',
                    ppUrl: result.ppUrl || '',
                    birthDate: result.birthDate ? result.birthDate.split('T')[0] : '',
                })
                setIsPrivate(result.isPrivate)
            } catch (err) {
                setError(err.message)
            }
        }
        fetchProfile()
    }, [token])

    function handleChange(e) {
        const { name, value } = e.target
        setFormData({ ...formData, [name]: value })
    }

    async function handleSubmit(e) {
        e.preventDefault()
        setError('')
        setSuccessMessage('')
        setIsLoading(true)

        try {
            await updateProfile(token, formData)
            setSuccessMessage('Profil güncellendi.')
        } catch (err) {
            setError(err.message)
        } finally {
            setIsLoading(false)
        }
    }

    async function handlePrivacyToggle() {
        setError('')
        try {
            await updateSettings(token, { isPrivate: !isPrivate })
            setIsPrivate(!isPrivate)
        } catch (err) {
            setError(err.message)
        }
    }

    async function handleFreeze() {
        if (!window.confirm('Hesabını dondurmak istediğine emin misin? Tekrar giriş yaparak aktifleştirebilirsin.')) return
        try {
            await freezeAccount(token)
            logoutUser()
            navigate('/login')
        } catch (err) {
            setError(err.message)
        }
    }

    async function handleDelete() {
        if (!window.confirm('Hesabını kalıcı olarak silmek istediğine emin misin? Bu işlem geri alınamaz.')) return
        try {
            await deleteAccount(token)
            logoutUser()
            navigate('/login')
        } catch (err) {
            setError(err.message)
        }
    }

    async function handlePasswordSubmit(e) {
        e.preventDefault()
        setPasswordError('')
        setPasswordSuccess('')

        if (newPassword !== confirmPassword) {
            setPasswordError('Yeni şifreler eşleşmiyor.')
            return
        }

        setIsChangingPassword(true)
        try {
            await changePassword(token, currentPassword, newPassword)
            setPasswordSuccess('Şifre başarıyla değiştirildi.')
            setCurrentPassword('')
            setNewPassword('')
            setConfirmPassword('')
        } catch (err) {
            setPasswordError(err.message)
        } finally {
            setIsChangingPassword(false)
        }
    }

    const inputClass = "w-full px-3.5 py-2.5 rounded-lg bg-[#0f141c] border border-[#1f2633] text-white placeholder-slate-500 focus:outline-none focus:border-[#6366f1] transition"
    const labelClass = "block text-xs font-medium text-slate-400 mb-1.5"

    return (
        <Layout>
            <div className="w-full max-w-[600px] mx-auto px-4 py-8 space-y-8">
                <h1 className="text-xl font-semibold text-white">Ayarlar</h1>

                {error && (
                    <p className="text-red-400 text-sm bg-red-500/10 border border-red-500/20 rounded-lg px-3 py-2">
                        {error}
                    </p>
                )}
                {successMessage && (
                    <p className="text-emerald-400 text-sm bg-emerald-500/10 border border-emerald-500/20 rounded-lg px-3 py-2">
                        {successMessage}
                    </p>
                )}

                {/* Profil Bilgileri */}
                <div className="bg-[#171c24] border border-[#1f2633] rounded-2xl p-6">
                    <h2 className="text-sm font-semibold text-white mb-4">Profil Bilgileri</h2>
                    <form onSubmit={handleSubmit} className="space-y-4">
                        <div className="grid grid-cols-2 gap-3">
                            <div>
                                <label className={labelClass}>Ad</label>
                                <input
                                    type="text"
                                    name="firstName"
                                    value={formData.firstName}
                                    onChange={handleChange}
                                    className={inputClass}
                                />
                            </div>
                            <div>
                                <label className={labelClass}>Soyad</label>
                                <input
                                    type="text"
                                    name="lastName"
                                    value={formData.lastName}
                                    onChange={handleChange}
                                    className={inputClass}
                                />
                            </div>
                        </div>

                        <div>
                            <label className={labelClass}>Biyografi</label>
                            <textarea
                                name="biography"
                                value={formData.biography}
                                onChange={handleChange}
                                rows={3}
                                className={`${inputClass} resize-none`}
                            />
                        </div>

                        <div>
                            <label className={labelClass}>Profil Fotoğrafı URL</label>
                            <input
                                type="text"
                                name="ppUrl"
                                value={formData.ppUrl}
                                onChange={handleChange}
                                className={inputClass}
                            />
                        </div>

                        <div>
                            <label className={labelClass}>Doğum Tarihi</label>
                            <input
                                type="date"
                                name="birthDate"
                                value={formData.birthDate}
                                onChange={handleChange}
                                className={`${inputClass} [color-scheme:dark]`}
                            />
                        </div>

                        <button
                            type="submit"
                            disabled={isLoading}
                            className="px-4 py-2.5 rounded-lg bg-[#6366f1] hover:bg-[#4f46e5] disabled:opacity-50 text-white text-sm font-medium transition"
                        >
                            {isLoading ? 'Kaydediliyor...' : 'Kaydet'}
                        </button>
                    </form>
                </div>

                {/* Gizlilik */}
                <div className="bg-[#171c24] border border-[#1f2633] rounded-2xl p-6">
                    <h2 className="text-sm font-semibold text-white mb-4">Gizlilik</h2>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-white font-medium">Gizli Hesap</p>
                            <p className="text-xs text-slate-400 mt-0.5">
                                Açıkken, seni takip etmeyenler gönderilerini göremez.
                            </p>
                        </div>
                        <button
                            onClick={handlePrivacyToggle}
                            className={`w-11 h-6 rounded-full transition relative shrink-0 ${isPrivate ? 'bg-[#6366f1]' : 'bg-[#1f2633]'
                                }`}
                        >
                            <span
                                className={`absolute top-0.5 w-5 h-5 rounded-full bg-white transition-transform ${isPrivate ? 'translate-x-5' : 'translate-x-0.5'
                                    }`}
                            />
                        </button>
                    </div>
                </div>

                {/* Şifre Değiştirme */}
                <div className="bg-[#171c24] border border-[#1f2633] rounded-2xl p-6">
                    <h2 className="text-sm font-semibold text-white mb-4">Şifre Değiştir</h2>

                    {passwordError && (
                        <p className="text-red-400 text-sm bg-red-500/10 border border-red-500/20 rounded-lg px-3 py-2 mb-4">
                            {passwordError}
                        </p>
                    )}
                    {passwordSuccess && (
                        <p className="text-emerald-400 text-sm bg-emerald-500/10 border border-emerald-500/20 rounded-lg px-3 py-2 mb-4">
                            {passwordSuccess}
                        </p>
                    )}

                    <form onSubmit={handlePasswordSubmit} className="space-y-4">
                        <div>
                            <label className={labelClass}>Mevcut Şifre</label>
                            <input
                                type="password"
                                value={currentPassword}
                                onChange={(e) => setCurrentPassword(e.target.value)}
                                className={inputClass}
                            />
                        </div>

                        <div>
                            <label className={labelClass}>Yeni Şifre</label>
                            <input
                                type="password"
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                                className={inputClass}
                            />
                        </div>

                        <div>
                            <label className={labelClass}>Yeni Şifre (Tekrar)</label>
                            <input
                                type="password"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                className={inputClass}
                            />
                        </div>

                        <button
                            type="submit"
                            disabled={isChangingPassword}
                            className="px-4 py-2.5 rounded-lg bg-[#6366f1] hover:bg-[#4f46e5] disabled:opacity-50 text-white text-sm font-medium transition"
                        >
                            {isChangingPassword ? 'Değiştiriliyor...' : 'Şifreyi Değiştir'}
                        </button>
                    </form>
                </div>

                {/* Tehlikeli Alan */}
                <div className="bg-[#171c24] border border-red-500/20 rounded-2xl p-6 space-y-4">
                    <h2 className="text-sm font-semibold text-red-400">Tehlikeli Alan</h2>

                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-white font-medium">Hesabı Dondur</p>
                            <p className="text-xs text-slate-400 mt-0.5">
                                Hesabın gizlenir, tekrar giriş yaparak geri getirebilirsin.
                            </p>
                        </div>
                        <button
                            onClick={handleFreeze}
                            className="px-3 py-1.5 rounded-lg bg-[#1e2533] hover:bg-[#283245] text-white text-xs font-medium transition"
                        >
                            Dondur
                        </button>
                    </div>

                    <div className="flex items-center justify-between pt-4 border-t border-[#1f2633]">
                        <div>
                            <p className="text-sm text-white font-medium">Hesabı Sil</p>
                            <p className="text-xs text-slate-400 mt-0.5">
                                Bu işlem kalıcıdır ve geri alınamaz.
                            </p>
                        </div>
                        <button
                            onClick={handleDelete}
                            className="px-3 py-1.5 rounded-lg bg-red-500/10 hover:bg-red-500/20 text-red-400 text-xs font-medium transition"
                        >
                            Sil
                        </button>
                    </div>
                </div>
            </div>
        </Layout>
    )
}

export default EditProfilePage