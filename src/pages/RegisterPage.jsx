import RegisterForm from '../components/RegisterForm'

function RegisterPage() {
    return (
        <div className="min-h-screen bg-[#0f141c] flex flex-col items-center justify-center px-4 py-12">
            <h1 className="text-2xl font-semibold text-white mb-8 tracking-tight">Sosyal Medya Projesi</h1>
            <RegisterForm />
        </div>
    )
}

export default RegisterPage