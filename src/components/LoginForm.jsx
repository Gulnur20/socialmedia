import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { login } from '../services/AuthService'
import { useAuth } from '../context/AuthContext'

function LoginForm() {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(false)

  const { loginUser } = useAuth()
  const navigate = useNavigate()

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setIsLoading(true)

    try {
      const result = await login(username, password)
      loginUser(result)
      navigate('/timeline')
    } catch (err) {
      setError(err.message)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="w-full max-w-sm mx-auto">
      <div className="bg-[#171c24] border border-[#1f2633] rounded-2xl p-8">
        <h2 className="text-xl font-semibold text-white mb-6 text-center">Giriş Yap</h2>

        {error && (
          <p className="text-red-400 text-sm bg-red-500/10 border border-red-500/20 rounded-lg px-3 py-2 mb-4">
            {error}
          </p>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-medium text-slate-400 mb-1.5">Kullanıcı Adı</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              className="w-full px-3.5 py-2.5 rounded-lg bg-[#0f141c] border border-[#1f2633] text-white placeholder-slate-500 focus:outline-none focus:border-[#6366f1] transition"
            />
          </div>

          <div>
            <label className="block text-xs font-medium text-slate-400 mb-1.5">Şifre</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full px-3.5 py-2.5 rounded-lg bg-[#0f141c] border border-[#1f2633] text-white placeholder-slate-500 focus:outline-none focus:border-[#6366f1] transition"
            />
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="w-full py-2.5 rounded-lg bg-[#6366f1] hover:bg-[#4f46e5] disabled:opacity-50 disabled:cursor-not-allowed text-white text-sm font-medium transition"
          >
            {isLoading ? 'Giriş yapılıyor...' : 'Giriş Yap'}
          </button>
        </form>
      </div>

      <p className="text-center text-sm text-slate-400 mt-6">
        Hesabın yok mu?{' '}
        <Link to="/register" className="text-[#818cf8] hover:text-indigo-400 font-medium">
          Kayıt Ol
        </Link>
      </p>
    </div>
  )
}

export default LoginForm