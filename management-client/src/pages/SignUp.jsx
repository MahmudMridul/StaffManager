import { useState } from "react";
import { Link, useNavigate } from "react-router";

export default function SignUp() {
	const navigate = useNavigate();
	const [formData, setFormData] = useState({
		firstName: "",
		lastName: "",
		username: "",
		email: "",
		password: "",
		confirmPassword: "",
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

		if (!formData.firstName.trim()) {
			newErrors.firstName = "First name is required";
		}

		if (!formData.lastName.trim()) {
			newErrors.lastName = "Last name is required";
		}

		if (!formData.username.trim()) {
			newErrors.username = "Username is required";
		} else if (formData.username.length < 3) {
			newErrors.username = "Username must be at least 3 characters";
		}

		if (!formData.email.trim()) {
			newErrors.email = "Email is required";
		} else if (!/\S+@\S+\.\S+/.test(formData.email)) {
			newErrors.email = "Please enter a valid email address";
		}

		if (!formData.password) {
			newErrors.password = "Password is required";
		} else if (formData.password.length < 6) {
			newErrors.password = "Password must be at least 6 characters";
		}

		if (!formData.confirmPassword) {
			newErrors.confirmPassword = "Please confirm your password";
		} else if (formData.password !== formData.confirmPassword) {
			newErrors.confirmPassword = "Passwords do not match";
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
		}, 2000);
	};

	const isFormValid = Object.values(formData).every(
		(value) => value.trim() !== ""
	);

	return (
		<div className="flex items-center justify-center min-h-screen p-4">
			<div className="w-full max-w-md">
				<div className="bg-white rounded-2xl shadow-xl p-8">
					<div className="text-center mb-8">
						<h1 className="text-3xl font-bold text-gray-900 mb-2">
							Create Account
						</h1>
						<p className="text-gray-600">Join us today</p>
					</div>

					<form onSubmit={handleSubmit} className="space-y-5">
						<div className="grid grid-cols-2 gap-4">
							<div>
								<label
									htmlFor="firstName"
									className="block text-sm font-medium text-gray-700 mb-2"
								>
									First Name
								</label>
								<input
									type="text"
									id="firstName"
									name="firstName"
									value={formData.firstName}
									onChange={handleChange}
									className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ${
										errors.firstName
											? "border-red-500 bg-red-50"
											: "border-gray-300 hover:border-gray-400"
									}`}
									placeholder="John"
								/>
								{errors.firstName && (
									<p className="mt-1 text-xs text-red-600">
										{errors.firstName}
									</p>
								)}
							</div>

							<div>
								<label
									htmlFor="lastName"
									className="block text-sm font-medium text-gray-700 mb-2"
								>
									Last Name
								</label>
								<input
									type="text"
									id="lastName"
									name="lastName"
									value={formData.lastName}
									onChange={handleChange}
									className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ${
										errors.lastName
											? "border-red-500 bg-red-50"
											: "border-gray-300 hover:border-gray-400"
									}`}
									placeholder="Doe"
								/>
								{errors.lastName && (
									<p className="mt-1 text-xs text-red-600">{errors.lastName}</p>
								)}
							</div>
						</div>

						<div>
							<label
								htmlFor="username"
								className="block text-sm font-medium text-gray-700 mb-2"
							>
								Username
							</label>
							<input
								type="text"
								id="username"
								name="username"
								value={formData.username}
								onChange={handleChange}
								className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ${
									errors.username
										? "border-red-500 bg-red-50"
										: "border-gray-300 hover:border-gray-400"
								}`}
								placeholder="johndoe"
							/>
							{errors.username && (
								<p className="mt-1 text-sm text-red-600">{errors.username}</p>
							)}
						</div>

						<div>
							<label
								htmlFor="email"
								className="block text-sm font-medium text-gray-700 mb-2"
							>
								Email
							</label>
							<input
								type="email"
								id="email"
								name="email"
								value={formData.email}
								onChange={handleChange}
								className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ${
									errors.email
										? "border-red-500 bg-red-50"
										: "border-gray-300 hover:border-gray-400"
								}`}
								placeholder="john@example.com"
							/>
							{errors.email && (
								<p className="mt-1 text-sm text-red-600">{errors.email}</p>
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

						<div>
							<label
								htmlFor="confirmPassword"
								className="block text-sm font-medium text-gray-700 mb-2"
							>
								Confirm Password
							</label>
							<input
								type="password"
								id="confirmPassword"
								name="confirmPassword"
								value={formData.confirmPassword}
								onChange={handleChange}
								className={`w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ${
									errors.confirmPassword
										? "border-red-500 bg-red-50"
										: "border-gray-300 hover:border-gray-400"
								}`}
								placeholder="Confirm your password"
							/>
							{errors.confirmPassword && (
								<p className="mt-1 text-sm text-red-600">
									{errors.confirmPassword}
								</p>
							)}
						</div>

						<div className="flex items-center">
							<input
								type="checkbox"
								id="terms"
								className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
							/>
							<label htmlFor="terms" className="ml-2 text-sm text-gray-600">
								I agree to the{" "}
								<a
									href="#"
									className="text-blue-600 hover:text-blue-800 transition-colors"
								>
									Terms of Service
								</a>{" "}
								and{" "}
								<a
									href="#"
									className="text-blue-600 hover:text-blue-800 transition-colors"
								>
									Privacy Policy
								</a>
							</label>
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
									Creating Account...
								</div>
							) : (
								"Sign Up"
							)}
						</button>
					</form>

					<div className="mt-8 text-center">
						<p className="text-gray-600">
							Already have an account?{" "}
							<Link
								to="/signin"
								className="text-gray-900 hover:text-black font-medium transition-colors"
							>
								Sign in
							</Link>
						</p>
					</div>
				</div>
			</div>
		</div>
	);
}
