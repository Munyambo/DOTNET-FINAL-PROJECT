import { BASE_URL, getHeaders, check } from "./config";

const API_URL = `${BASE_URL}/Auth`;

// LOGIN: sends email + password, returns { token, role, name, userId }
export async function login(email, password) {
  const res = await fetch(`${API_URL}/Login`, {
    method: "POST",
    headers: getHeaders(),
    body: JSON.stringify({ email, password }),
  });
  await check(res);
  return res.json();
}

// REGISTER: sends the new customer's details
export async function register(data) {
  const res = await fetch(`${API_URL}/Register`, {
    method: "POST",
    headers: getHeaders(),
    body: JSON.stringify(data),
  });
  await check(res);
  return res.json();
}