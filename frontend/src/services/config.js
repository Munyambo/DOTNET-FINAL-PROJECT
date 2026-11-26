// The address of our C# API (from launchSettings.json)
export const BASE_URL = "/api";

// Headers for every request (adds the login token once we have JWT)
export function getHeaders() {
  const token = localStorage.getItem("token");
  return {
    "Content-Type": "application/json",
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

// Throws a clear error if the API said no
export async function check(res) {
  if (!res.ok) throw new Error(`API error ${res.status}`);
  return res;
}