import { BASE_URL, getHeaders, check } from "./config";

const API_URL = `${BASE_URL}/Branch`;

// VIEW ALL
export async function getBranches() {
  const res = await fetch(`${API_URL}/GetAll`, { headers: getHeaders() });
  await check(res);
  return res.json();
}

// VIEW ONE
export async function getBranch(id) {
  const res = await fetch(`${API_URL}/Get/${id}`, { headers: getHeaders() });
  await check(res);
  return res.json();
}

// CREATE
export async function createBranch(branch) {
  const res = await fetch(`${API_URL}/Create`, {
    method: "POST",
    headers: getHeaders(),
    body: JSON.stringify(branch),
  });
  await check(res);
  return res.json();
}

// UPDATE
export async function updateBranch(branch) {
  const res = await fetch(`${API_URL}/Update`, {
    method: "PUT",
    headers: getHeaders(),
    body: JSON.stringify(branch),
  });
  await check(res);
  return res.json();
}

// DELETE
export async function deleteBranch(id) {
  const res = await fetch(`${API_URL}/Delete/${id}`, {
    method: "DELETE",
    headers: getHeaders(),
  });
  await check(res);
  return res.json();
}