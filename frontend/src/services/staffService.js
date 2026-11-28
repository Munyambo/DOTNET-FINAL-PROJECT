import { BASE_URL, getHeaders, check } from "./config";

const API_URL = `${BASE_URL}/Staff`;

// VIEW ALL (each staff comes with its branch)
export async function getStaff() {
  const res = await fetch(`${API_URL}/GetAll`, { headers: getHeaders() });
  await check(res);
  return res.json();
}

// VIEW ONE
export async function getStaffMember(id) {
  const res = await fetch(`${API_URL}/Get/${id}`, { headers: getHeaders() });
  await check(res);
  return res.json();
}

// CREATE (also creates the login: needs email and password)
export async function createStaff(staff) {
  const res = await fetch(`${API_URL}/Create`, {
    method: "POST",
    headers: getHeaders(),
    body: JSON.stringify(staff),
  });
  await check(res);
  return res.json();
}

// UPDATE (send the userId too, or the staff loses their login link)
export async function updateStaff(staff) {
  const res = await fetch(`${API_URL}/Update`, {
    method: "PUT",
    headers: getHeaders(),
    body: JSON.stringify(staff),
  });
  await check(res);
  return res.json();
}

// DELETE (also deletes the login)
export async function deleteStaff(id) {
  const res = await fetch(`${API_URL}/Delete/${id}`, {
    method: "DELETE",
    headers: getHeaders(),
  });
  await check(res);
  return res.json();
}