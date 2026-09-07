import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { register } from '../services/AuthService'

function RegisterForm() {
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        birthDate: '',
    })
    const [error, setError] = useState('')
    const [successMessage, setSuccessMessage] = useState('')
    const [isLoading, setIsLoading] = useState(false)
    const navigate = useNavigate()

    function handleChange(event) {
        const { name, value } = event.target
        setFormData({ ...formData, [name]: value })
    }

    async function handleSubmit(event) {
        event.preventDefault()
        setError('')
        setSuccessMessage('')
        setIsLoading(true)
        try {
            const result = await register(formData)
            setSuccessMessage(`Kayıt başarılı! Hoş geldin, ${result.username}!`)
            setTimeout(() => navigate('/login'), 1500)
        } catch (error) {
            setError(error.message)
        } finally {
            setIsLoading(false)
        }
    }

    const inputClass = "w-full px-3.5 py-2.5 rounded-lg bg-[#0f141c] border border-[#1f2633] text-white placeholder-slate-500 focus:outline-none focus:border-[#6366f1] transition"
    const labelClass = "block text-xs font-medium text-slate-400 mb-1.5"

    return (
        <div className="w-full max-w-sm mx-auto">
            <div className="bg-[#171c24] border border-[#1f2633] rounded-2xl p-8">
                <h2 className="text-xl font-semibold text-white mb-6 text-center">Kayıt Ol</h2>

                {error && (
                    <p className="text-red-400 text-sm bg-red-500/10 border border-red-500/20 rounded-lg px-3 py-2 mb-4">
                        {error}
                    </p>
                )}
                {successMessage && (
                    <p className="text-emerald-400 text-sm bg-emerald-500/10 border border-emerald-500/20 rounded-lg px-3 py-2 mb-4">
                        {successMessage}
                    </p>
                )}

                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label className={labelClass}>Kullanıcı Adı</label>
                        <input
                            type="text"
                            name="username"
                            value={formData.username}
                            onChange={handleChange}
                            className={inputClass}
                        />
                    </div>

                    <div>
                        <label className={labelClass}>E-posta</label>
                        <input
                            type="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            className={inputClass}
                        />
                    </div>

                    <div>
                        <label className={labelClass}>Şifre</label>
                        <input
                            type="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            className={inputClass}
                        />
                    </div>

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
                        className="w-full py-2.5 rounded-lg bg-[#6366f1] hover:bg-[#4f46e5] disabled:opacity-50 disabled:cursor-not-allowed text-white text-sm font-medium transition"
                    >
                        {isLoading ? 'Kayıt olunuyor...' : 'Kayıt Ol'}
                    </button>
                </form>
            </div>

            <p className="text-center text-sm text-slate-400 mt-6">
                Zaten hesabın var mı?{' '}
                <Link to="/login" className="text-[#818cf8] hover:text-indigo-400 font-medium">
                    Giriş Yap
                </Link>
            </p>
        </div>
    )
}

export default RegisterForm