import { useState } from "react";
import { Link, useNavigate } from "react-router";

export default function SignIn() {
	const navigate = useNavigate();
	const [formData, setFormData] = useState({
		emailOrUsername: "",
		password: "",
	});
	const [errors, setErrors] = useState({});
	const [isLoading, setIsLoading] = useState(false);

	const handleChange = (e) => {
		const { name, value } = e.target;
		setFormData((prev) => ({
			...prev,
			[name]: value,
		}));

		// Clear error when user starts typing
		if (errors[name]) {
			setErrors((prev) => ({
				...prev,
				[name]: "",
			}));
		}
	};

	const validateForm = () => {
		const newErrors = {};

		if (!formData.emailOrUsername.trim()) {
			newErrors.emailOrUsername = "Email or username is required";
		}

		if (!formData.password) {
			newErrors.password = "Password is required";
		}

		return newErrors;
	};

	const handleSubmit = async (e) => {
		e.preventDefault();

		const formErrors = validateForm();
		if (Object.keys(formErrors).length > 0) {
			setErrors(formErrors);
			return;
		}

		setIsLoading(true);

		// Simulate API call
		setTimeout(() => {
			setIsLoading(false);
			navigate("/dashboard");
		}, 1500);
	};

	const isFormValid = formData.emailOrUsername.trim() && formData.password;

	return (
		<div className="flex items-center justify-center min-h-screen p-4">
			<div className="w-full max-w-md">
				<div className="bg-white rounded-2xl shadow-xl p-8">
					<div className="text-center mb-8">
						<h1 className="text-3xl font-bold text-gray-900 mb-2">
							Welcome Back
						</h1>
						<p className="text-gray-600">Sign in to your account</p>
					</div>

					<form onSubmit={handleSubmit} className="space-y-6">
						<div>
							<label
								htmlFor="emailOrUsername"
								className="block text-sm font-medium text-gray-700 mb-2"
							>
								Email or Username
							</label>
							<input
								type="text"
								id="emailOrUsername"
								name="emailOrUsername"
								value={formData.emailOrUsername}
								onChange={handleChange}
								className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ${
									errors.emailOrUsername
										? "border-red-500 bg-red-50"
										: "border-gray-300 hover:border-gray-400"
								}`}
								placeholder="Enter your email or username"
							/>
							{errors.emailOrUsername && (
								<p className="mt-1 text-sm text-red-600">
									{errors.emailOrUsername}
								</p>
							)}
						</div>

						<div>
							<label
								htmlFor="password"
								className="block text-sm font-medium text-gray-700 mb-2"
							>
								Password
							</label>
							<input
								type="password"
								id="password"
								name="password"
								value={formData.password}
								onChange={handleChange}
								className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ${
									errors.password
										? "border-red-500 bg-red-50"
										: "border-gray-300 hover:border-gray-400"
								}`}
								placeholder="Enter your password"
							/>
							{errors.password && (
								<p className="mt-1 text-sm text-red-600">{errors.password}</p>
							)}
						</div>

						<div className="flex items-center justify-between">
							<label className="flex items-center">
								<input
									type="checkbox"
									className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
								/>
								<span className="ml-2 text-sm text-gray-600">Remember me</span>
							</label>
							<a
								href="#"
								className="text-sm text-blue-600 hover:text-blue-800 transition-colors"
							>
								Forgot password?
							</a>
						</div>

						<button
							type="submit"
							disabled={!isFormValid || isLoading}
							className={`w-full py-3 px-4 rounded-lg font-medium transition-all duration-200 ${
								isFormValid && !isLoading
									? "bg-blue-600 hover:bg-blue-700 text-white shadow-lg hover:shadow-xl transform hover:-translate-y-0.5"
									: "bg-gray-300 text-gray-500 cursor-not-allowed"
							}`}
						>
							{isLoading ? (
								<div className="flex items-center justify-center">
									<div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></div>
									Signing In...
								</div>
							) : (
								"Sign In"
							)}
						</button>
					</form>

					<div className="mt-8 text-center">
						<p className="text-gray-600">
							Don't have an account?{" "}
							<Link
								to="/signup"
								className="text-gray-900 hover:text-black font-medium transition-colors"
							>
								Sign up
							</Link>
						</p>
					</div>
				</div>
			</div>
		</div>
	);
}
