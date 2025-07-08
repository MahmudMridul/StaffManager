import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { urls } from "./apiConfig";

const initialState = {
	value: 0,
};

export const signUp = createAsyncThunk(
	"auth/signUp",
	async (payload, { rejectWithValue }) => {
		try {
			const url = urls.signUp;
			const response = await fetch(url, {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
					Accept: "application/json",
				},
				body: JSON.stringify(payload),
			});

			if (!response.ok) {
				const errorData = await response.json();
				return rejectWithValue(errorData.message || "Signup failed");
			}

			const data = await response.json();
			return data;
		} catch (error) {
			return rejectWithValue(error.message || "Network error");
		}
	}
);

export const appSlice = createSlice({
	name: "counter",
	initialState,
	reducers: {
		increment: (state) => {
			// Redux Toolkit allows us to write "mutating" logic in reducers. It
			// doesn't actually mutate the state because it uses the Immer library,
			// which detects changes to a "draft state" and produces a brand new
			// immutable state based off those changes
			state.value += 1;
		},
		decrement: (state) => {
			state.value -= 1;
		},
		incrementByAmount: (state, action) => {
			state.value += action.payload;
		},
	},
});

// Action creators are generated for each case reducer function
export const { increment, decrement, incrementByAmount } = appSlice.actions;

export default appSlice.reducer;
