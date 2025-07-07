import { createBrowserRouter } from "react-router";
import SignUp from "../pages/SignUp";
import SignIn from "../pages/SignIn";

export const router = createBrowserRouter([
	{
		path: "/",
		Component: SignIn,
	},
	{
		path: "/signup",
		Component: SignUp,
	},
]);
